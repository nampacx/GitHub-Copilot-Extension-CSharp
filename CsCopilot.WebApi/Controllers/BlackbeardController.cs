using Microsoft.AspNetCore.Mvc;
using Octokit;
using Shared.DTOs;
using Shared.Helpers;
using System.Linq;
using System.IO;
using Shared.Helpers.Extensions;

namespace CsCopilot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlackbeardController : ControllerBase
{

    private readonly ILogger<BlackbeardController> _logger;
    private readonly GitHubLLMClient _gitHubLLMClient;

    public BlackbeardController(ILogger<BlackbeardController> logger, GitHubLLMClient gitHubLLMClient)
    {
        _logger = logger;
        _gitHubLLMClient = gitHubLLMClient;
    }

    [HttpPost("stream")]
    public async Task GetStream([FromHeader(Name = "X-GitHub-Token")] string tokenForUser, [FromBody] CopilotData copilotData)
    {
        _logger.LogInformation($"{nameof(GetStream)} called.");

        var user = await tokenForUser.GetUser();
        _logger.LogInformation($"User: {user.Login}");

        Response.ContentType = "text/event-stream";
        var chatCompletionsRequest = new ChatCompletionsRequest
        {
            Messages = copilotData.messages.Select(m => new ChatMessage { Content = m.content, Role = m.role }).ToList(),
            Stream = false
        };

        chatCompletionsRequest.Messages.Insert(0, new ChatMessage { Role = "system", Content = "You are a helpful assistant that replies to user messages as if you were the Blackbeard Pirate." });
        chatCompletionsRequest.Messages.Insert(0, new ChatMessage { Role = "system", Content = $"Start every response with the user's name, which is @{user.Login}" });

        var response = await _gitHubLLMClient.ChatCompletionsAsync(tokenForUser, chatCompletionsRequest);
        var responseString = await response.ReadAsStringAsync();

        //await Response.SendStreamedResponseAsync(responseString);
        await Response.SendGitHubLLMResponseAsync(responseString, chatCompletionsRequest.Stream);

        await Response.Body.FlushAsync();
    }
}
