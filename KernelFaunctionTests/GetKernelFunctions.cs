using FluentAssertions;
using SemanticKernel.ToolsExtractor;
using SemanticKernelPlugins;

namespace KernelFunctionTests;

public class GetKernelFunctions
{
    [Fact]
    public void GetKernelFunctionMethodsTest()
    {

        var methods = ToolsExtractorService.GetKernelFunctionMethods(typeof(FilesPlugin));

        methods.Should().HaveCount(4);
    }

    [Fact]
    public void GetKernelFunctionAttributeDescriptionTests()
    {
       var methods = ToolsExtractorService.GetKernelFunctionMethods(typeof(FilesPlugin));

        methods.All(m => m.Description != string.Empty);
    }
}