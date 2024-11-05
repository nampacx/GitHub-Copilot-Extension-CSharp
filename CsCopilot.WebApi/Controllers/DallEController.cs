using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextToImage;
using System.Net.Http.Headers;
using Shared.Helpers;
using Shared.DTOs;
using Shared.Helpers.Extensions;

#pragma warning disable SKEXP0001, SKEXP0010

namespace Nampacx.Copilot.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DalleController : ControllerBase
    {
        private readonly ILogger<DalleController> _logger;
        private readonly ITextToImageService _textToImage;

        public DalleController(ILogger<DalleController> logger,ITextToImageService textToImageService)
        {
            _logger = logger;
            _textToImage = textToImageService;
        }

        [HttpPost]
        public async Task GenerateImage([FromHeader(Name = "X-GitHub-Token")] string tokenForUser, [FromBody] CopilotData copilotData)
        {
            Response.ContentType = "text/event-stream";

            var user = await tokenForUser.GetUser();
            _logger.LogInformation($"User: {user.Login}");

            var response = string.Empty;
            // Insert a special pirate-y system message in our message list.
            try
            {
                var message = copilotData.messages.Last();

               await Response.SendAsSSEResponseAsync("Generating image");

                var url = await _textToImage.GenerateImageAsync(message.content, 1024, 1024);
                response = $"""
                    <a href="{url}"> image </a>
                    """;
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error generating image");
                response = "Error generating image!";
            }

            await Response.SendAsSSEResponseAsync(response);
        }
    }
}