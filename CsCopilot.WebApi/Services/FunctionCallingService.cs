using SemanticKernel.Connector;
using SemanticKernel.ToolsExtractor;
using SemanticKernelPlugins;
using Shared.DTOs;
using System.Text.Json;

namespace Nampacx.Copilot.WebApi.Services;

public class FunctionCallingService
{
    public List<FunctionTool> Tools { get; private set; }

    public FunctionCallingService()
    {
        Tools = new List<FunctionTool>();
    }

    public void RegisterTool<T>()
    {
        Tools.AddRange(ToolsExtractor.GetAllFunctionTools<T>());
    }

    public string Execute(FunctionTool functionTool)
    {
        return JsonSerializer.Serialize(ToolInvoker.InvokeFunctionTool(functionTool));
    }

    public string Execute(FunctionTool functionTool, Dictionary<string, object> parameters)
    {
        return JsonSerializer.Serialize(ToolInvoker.InvokeFunctionTool(functionTool, parameters));
    }

    public string Execute(CopilotFunction copilotFunction)
    {
        var tool = Tools.FirstOrDefault(t => t.Function.Name == copilotFunction.Name);

        if (copilotFunction.Parameters.Count == 0)
        {
            return Execute(tool);
        }
        else
        {
            return Execute(tool, copilotFunction.Parameters);
        }
    }
}
