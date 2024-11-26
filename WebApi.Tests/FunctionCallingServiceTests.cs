using FluentAssertions;
using Nampacx.Copilot.WebApi.Services;
using SemanticKernelPlugins;
using Shared.DTOs;
using Shared.Helpers;
using Xunit;

namespace WebApi.Tests;

public class FunctionCallingServiceTests
{
    [Fact]
    public void RegisterTool()
    {
        var fcs = new FunctionCallingService();

        fcs.RegisterTool<FilesPlugin>();

        fcs.Tools.Should().NotBeEmpty();
     }

    [Fact]
    public void Invoke_ListFilesInCurrentDirectory()
    {
        var fcs = new FunctionCallingService();
        fcs.RegisterTool<FilesPlugin>();

        var tool = fcs.Tools.FirstOrDefault(t => t.Function.Name == "ListFilesInCurrentDirectory");
        var response = fcs.Execute(tool);

        response.Should().NotBeNull();
    }

    [Fact]
    public void Invoke_CopilotFunction()
    {
        var fcs = new FunctionCallingService();
        fcs.RegisterTool<FilesPlugin>();

        var function = new CopilotFunction()
        {
            Arguments = null,
            Name = "ListFilesInCurrentDirectory"
        };

        var response = fcs.Execute(function);

        response.Should().NotBeNull();
    }

    [Fact]
    public void Invoke_MethodWithParameters()
    {
        var pathKey = "directoryPath";
        var data = File.ReadAllText("./Files/functionarguments.txt").Replace(Environment.NewLine, "\n"); ;

        var gitHubLLmClient = new GitHubLLMClient();

        var function = gitHubLLmClient.GetFunctionsToCall(data);

        var fcs = new FunctionCallingService();
        fcs.RegisterTool<FilesPlugin>();

        var response = fcs.Execute(function);

        response.Should().NotBeNull();
    }
}