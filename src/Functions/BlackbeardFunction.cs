using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Octokit;
using Shared.DTOs;
using Shared.Helpers;

namespace Nampacx.Copilot.Function
{
    public class BlackbeardFunction
    {
        private readonly ILogger<BlackbeardFunction> _logger;
        private readonly GitHubLLMClient _gitHubLLMClient;
        private readonly HttpClient _httpClient;

        public BlackbeardFunction(ILogger<BlackbeardFunction> logger)
        {
            _logger = logger;
            _gitHubLLMClient = new GitHubLLMClient();
        }

        [Function("BlackbeardFunction")]
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
            HttpResponseMessage copilotLLMResponse = await GetLLMResponse(req, tokenForUser, user);

            _logger.LogInformation($"Copilot LLM response: {copilotLLMResponse.StatusCode}");
            var response = await copilotLLMResponse.Content.ReadAsStringAsync();
            if (!copilotLLMResponse.IsSuccessStatusCode)
            {
                _logger.LogError($"Copilot LLM response: {copilotLLMResponse.StatusCode} - {response}");
            }

            return new OkObjectResult(response);
        }

        private async Task<HttpResponseMessage> GetLLMResponse(HttpRequest req, string tokenForUser, User user)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var copilotData = JsonSerializer.Deserialize<CopilotData>(requestBody);

            // Insert a special pirate-y system message in our message list.
            var messages = new List<Message>(copilotData.messages);
            messages.Insert(0, new Message { role = "system", content = "You are a helpful assistant that replies to user messages as if you were the Blackbeard Pirate." });
            messages.Insert(0, new Message { role = "system", content = $"Start every response with the user's name, which is @{user.Login}" });

            // Use Copilot's LLM to generate a response to the user's messages, with our extra system messages attached.
            var copilotLLMResponse = await _gitHubLLMClient.PostAsync(tokenForUser, messages);
            return copilotLLMResponse;
        }

    }
}
