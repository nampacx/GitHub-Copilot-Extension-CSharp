using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Helpers;
using Shared.Helpers.Extensions;
using System.Text.Json;

namespace CsCopilot.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EraserDiagramController : ControllerBase
{

    private readonly ILogger<EraserDiagramController> _logger;
    private readonly GitHubLLMClient _gitHubLLMClient;
    private readonly IWebHostEnvironment webHostEnvironment;

    public EraserDiagramController(
        ILogger<EraserDiagramController> logger,
        GitHubLLMClient gitHubLLMClient,
        IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _gitHubLLMClient = gitHubLLMClient;
        this.webHostEnvironment = webHostEnvironment;
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

        chatCompletionsRequest.Messages.Insert(0,
            new ChatMessage
            {
                Role = "system",
                Content = GetSystemPrompt()
            });

        var response = await _gitHubLLMClient.ChatCompletionsAsync(tokenForUser, chatCompletionsRequest);
        var responseString = await response.ReadAsStringAsync();



        await Response.SendGitHubLLMResponseAsync(responseString, chatCompletionsRequest.Stream);

        await Response.Body.FlushAsync();
    }

    [HttpGet]
    public string GetSystemPrompt()
    {
        return BuildSystemPrompt();
    }

    private string BuildSystemPrompt()
    {
        var icons = ReadFile("Grounding/eraser/icons.json");

        var samples = GetFiles("Grounding/eraser/samples").Select(f => ReadFile(f));

        var eraserdiagram_template_promnpt =
$"""
You are an expert in creating earaserdiagrams. 
Make sure that all open open curly braces are closed with a closing brace.
Make sure the first line always is cloud-architecture-diagram.
Also try to select the most appropriate icons for each resource based on the provided icons.

For reference you can use the following samples:

## Samples:
{JsonSerializer.Serialize(samples)}

## Icons:
{icons}
""";

        return eraserdiagram_template_promnpt;
    }

    private IEnumerable<string> GetFiles(string directoryPath)
    {
        string physicalPath = Path.Combine(webHostEnvironment.ContentRootPath, directoryPath);

        return Directory.GetFiles(physicalPath, "*", SearchOption.AllDirectories);
    }

    private string ReadFile(string relativePath)
    {
        // Map the relative path to the physical path
        string physicalPath = Path.Combine(webHostEnvironment.ContentRootPath, relativePath);

        // Read the file content
        if (System.IO.File.Exists(physicalPath))
        {
            return System.IO.File.ReadAllText(physicalPath);
        }
        else
        {
            throw new FileNotFoundException("File not found", physicalPath);
        }
    }

}

