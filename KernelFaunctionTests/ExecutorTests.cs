using FluentAssertions;
using SemanticKernel.Connector;
using SemanticKernel.ToolsExtractor;
using SemanticKernelPlugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelFunctionTests;

public class ExecutorTests
{
    [Fact]
    public void ExecuteMethod()
    {
        var methods = ToolsExtractor.GetKernelFunctionMethods(typeof(FilesPlugin));

        var methodInfo = methods.LastOrDefault();

        var response = ToolInvoker.InvokeMethod(methodInfo);

        response.Should().NotBeNull();
    }

    [Fact]
    public void ExecuteMethodFromFunctionTool()
    {
        var tools = ToolsExtractor.GetAllFunctionTools(typeof(FilesPlugin));

        var lastTool = tools.LastOrDefault();

        var response = ToolInvoker.InvokeFunctionTool(lastTool);

        response.Should().NotBeNull();
    }
}
