using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Octokit;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextToImage;
using Microsoft.Extensions.Configuration;
using CsCopilot.DTOs;
using CsCopilot.Helpers;

namespace Nampacx.Copilot.Function
{
#pragma warning disable SKEXP0001, SKEXP0010

    public class DalleFunction
    {
        private readonly ILogger<DalleFunction> _logger;
        private readonly HttpClient _httpClient;
        private ITextToImageService textToImage;
        private BitlyShortUrl bitlyShortUrl;

        public DalleFunction(ILogger<DalleFunction> logger, IConfiguration _configuration)
        {

            _logger = logger;
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(@"https://api.githubcopilot.com")
            };

            var builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAITextToImage(
                _configuration["TextToImageModel"],
                _configuration["OpenAIEndpoint"],
                _configuration["OpenAIKey"]);

            var kernel = builder.Build();

            textToImage = kernel.GetRequiredService<ITextToImageService>();

            bitlyShortUrl = new BitlyShortUrl(_configuration["BitlyToken"]);
        }

        [Function("DalleFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            // Identify the user, using the GitHub API token provided in the request headers.
            string tokenForUser = req.Headers["X-GitHub-Token"];
            var client = new GitHubClient(new ProductHeaderValue("GitHubCopilotFunction"))
            {
                Credentials = new Credentials(tokenForUser)
            };
            var user = await client.User.Current();
            _logger.LogInformation($"User: {user.Login}");

            // Parse the request payload and log it.
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var copilotData = JsonSerializer.Deserialize<CopilotData>(requestBody);

            // Insert a special pirate-y system message in our message list.

            var message = copilotData.messages.Last();

            var url = await textToImage.GenerateImageAsync(message.content, 1024, 1024);

            url =await bitlyShortUrl.ShortenAsync(url);

            var messages = new List<Message>
            {
                new Message
                {
                    role = "system",
                    content =
                """
                You are a helpful assistant who supports users generate image using DallE-3.
                The user will give you a URI and please format it in a ways the user can access it from the chat.
                """
                },
                new Message
                {
                    role = "user",
                    content = $"Image url for '{message.content}' is {url}"
                }
            }; 

            // Use Copilot's LLM to generate a response to the user's messages, with our extra system messages attached.
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenForUser}");
            var copilotLLMResponse = await _httpClient.PostAsync(
                "/chat/completions",
                new StringContent(
                    JsonSerializer.Serialize(
                        new { messages, stream = true }),
                        System.Text.Encoding.UTF8,
                        "application/json")
            );

            _logger.LogInformation($"Copilot LLM response: {copilotLLMResponse.StatusCode}");
            var response = await copilotLLMResponse.Content.ReadAsStringAsync();
            if (!copilotLLMResponse.IsSuccessStatusCode)
            {
                _logger.LogError($"Copilot LLM response: {copilotLLMResponse.StatusCode} - {response}");
            }

            return new OkObjectResult(response);
        }
    }
}
