using FluentAssertions;
using Nampacx.Copilot.WebApi.Services;
using SemanticKernelPlugins;
using Shared.DTOs;
using Xunit;

namespace WebApi.Tests;

public class FunctionCallingServiceTests
{
    [Fact]
    public void TestRegisterTools()
    {
        var fcs = new FunctionCallingService();

        fcs.RegisterTool<FilesPlugin>();

        fcs.Tools.Should().NotBeEmpty();
     }

    [Fact]
    public void TestToolInvoke()
    {
        var fcs = new FunctionCallingService();
        fcs.RegisterTool<FilesPlugin>();

        var tool = fcs.Tools.FirstOrDefault(t => t.Function.Name == "ListFilesInCurrentDirectory");
        var response = fcs.Execute(tool);

        response.Should().NotBeNull();
    }

    [Fact]
    public void TestCopilotFunctionInvoke()
    {
        var fcs = new FunctionCallingService();
        fcs.RegisterTool<FilesPlugin>();

        var function = new CopilotFunction()
        {
            arguments = null,
            name = "ListFilesInCurrentDirectory"
        };

        var response = fcs.Execute(function);

        response.Should().NotBeNull();
    }
}