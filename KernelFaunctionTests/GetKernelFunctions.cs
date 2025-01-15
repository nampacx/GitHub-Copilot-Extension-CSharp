using FluentAssertions;
using SemanticKernel.ToolsExtractor;
using SemanticKernelPlugins;
using System.Text.Json;

namespace KernelFunctionTests;

public class GetKernelFunctions
{
    [Fact]
    public void GetKernelFunctionMethodsTest()
    {
        var methods = ToolsExtractor.GetKernelFunctionMethods(typeof(FilesPlugin));

        methods.Should().HaveCount(4);
    }

    [Fact]
    public void GetAllFunctionToolsFromType()
    {
        var methods = ToolsExtractor.GetAllFunctionTools(typeof(FilesPlugin));

        methods.Should().HaveCount(4);
    }

    [Fact]
    public void GetFunctionWithDescriptionOfMethodInfo()
    {
        var method = ToolsExtractor.GetKernelFunctionMethods(typeof(FilesPlugin)).FirstOrDefault();

        var functionTool = method.ToFunctionTool();

        functionTool.Function.Description.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GetFunctionWithParametersOfMethodInfo()
    {
        var method = ToolsExtractor.GetKernelFunctionMethods(typeof(FilesPlugin)).FirstOrDefault();

        var functionTool = method.ToFunctionTool();

        functionTool.Function.Parameters.Properties.Count.Should().Be(2);
    }

    [Fact]
    public void GetFunctionToolJson()
    {
        var method = ToolsExtractor.GetKernelFunctionMethods(typeof(FilesPlugin)).FirstOrDefault();

        var functionTool = method.ToFunctionTool();

        var json = JsonSerializer.Serialize(functionTool);

        json.Should().NotContain("Type");
    }
}