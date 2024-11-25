using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Helpers.Extensions;
using Shared.Helpers;
using SemanticKernel.ToolsExtractor;
using SemanticKernelPlugins;

namespace Nampacx.Copilot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FunctionCallingController: ControllerBase
{
    private readonly ILogger<FunctionCallingController> _logger;
    private readonly GitHubLLMClient _gitHubLLMClient;
    private readonly List<FunctionTool> _tools;

    public FunctionCallingController(ILogger<FunctionCallingController> logger, GitHubLLMClient gitHubLLMClient)
    {
        _logger = logger;
        _gitHubLLMClient = gitHubLLMClient;

        _tools = ToolsExtractorService.GetAllFunctionTools(typeof(FilesPlugin)).ToList();
    }

    [HttpPost]
    public async Task FileFunctions([FromHeader(Name = "X-GitHub-Token")] string tokenForUser, [FromBody] CopilotData copilotData)
    {
        _logger.LogInformation($"{nameof(FileFunctions)} called.");

        var user = await tokenForUser.GetUser();
        _logger.LogInformation($"User: {user.Login}");

        Response.ContentType = "text/event-stream";
        var chatCompletionsRequest = new ChatCompletionsRequest
        {
            Messages = copilotData.messages.Select(m => new ChatMessage { Content = m.content, Role = m.role }).ToList(),
            Stream = true,
            Tools = _tools
        };

        chatCompletionsRequest.Messages.Insert(0, new ChatMessage { Role = "system", Content = "You are a helpful assistant that helps users manage their files." });

        var response = await _gitHubLLMClient.ChatCompletionsAsync(tokenForUser, chatCompletionsRequest);
        var responseString = await response.ReadAsStringAsync();

        await Response.SendGitHubLLMResponseAsync(responseString, chatCompletionsRequest.Stream);

        await Response.Body.FlushAsync();
    }
}   
