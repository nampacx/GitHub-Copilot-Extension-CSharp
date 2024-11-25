using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Helpers.Extensions;
using Shared.Helpers;
using SemanticKernel.ToolsExtractor;
using SemanticKernelPlugins;
using System.Text.Json;
using Nampacx.Copilot.WebApi.Services;

namespace Nampacx.Copilot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FunctionCallingController : ControllerBase
{
    private readonly ILogger<FunctionCallingController> _logger;
    private readonly GitHubLLMClient _gitHubLLMClient;
    private readonly FunctionCallingService _functionCallingService;

    public FunctionCallingController(ILogger<FunctionCallingController> logger, GitHubLLMClient gitHubLLMClient, FunctionCallingService functionCallingService)
    {
        _logger = logger;
        _gitHubLLMClient = gitHubLLMClient;
        _functionCallingService = functionCallingService;

        functionCallingService.RegisterTool<FilesPlugin>();
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
            Tools = _functionCallingService.Tools
        };

        chatCompletionsRequest.Messages.Insert(0,
            new ChatMessage
            {
                Role = "system",
                Content = "You are a helpful assistant that helps users manage their files."
            });

        var ser = JsonSerializer.Serialize(chatCompletionsRequest);

        var responseString = string.Empty;

        for (int i = 0; i < 5; i++)
        {
            responseString = await (await _gitHubLLMClient.ChatCompletionsAsync(tokenForUser, chatCompletionsRequest)).ReadAsStringAsync();

            var functionsToCall = _gitHubLLMClient.GetFunctionsToCall(responseString).ToList();
            if(!functionsToCall.Any())
            {
                break;
            }

            foreach (var function in functionsToCall)
            {
                var result = _functionCallingService.Execute(function);

                var newMessage = new ChatMessage
                {
                    Role = "system",
                    Content = $"""
                                The function: {function.name} returned: {result}
                              """
                };

                chatCompletionsRequest.Messages.Add(newMessage);
            }
        }

        await Response.SendGitHubLLMResponseAsync(responseString, chatCompletionsRequest.Stream);

        await Response.Body.FlushAsync();
    }


}
