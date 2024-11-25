using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Helpers.Extensions;
using Shared.Helpers;
using SemanticKernel.ToolsExtractor;
using SemanticKernelPlugins;
using System.Text.Json;

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

        _tools = ToolsExtractor.GetAllFunctionTools(typeof(FilesPlugin)).ToList();
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

        var ser = JsonSerializer.Serialize(chatCompletionsRequest);

        var response = await _gitHubLLMClient.ChatCompletionsAsync(tokenForUser, chatCompletionsRequest);
        var responseString = await response.ReadAsStringAsync();

        var copilotResponses = _gitHubLLMClient.ParesStringToResponses(responseString);
        var responsesWithFunctionCall = copilotResponses.Where(cpR => cpR.choices.Any(c => c.delta.toolCalls))


        await Response.SendGitHubLLMResponseAsync(responseString, chatCompletionsRequest.Stream);

        await Response.Body.FlushAsync();
    }


}   
