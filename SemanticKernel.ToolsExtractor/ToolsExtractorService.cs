using Microsoft.SemanticKernel;
using Shared.DTOs;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.Json;

namespace SemanticKernel.ToolsExtractor;

public class ToolsExtractorService
{
    public static MethodInfo[] GetKernelFunctionMethods(Type type)
    {
        return type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                   .Where(m => m.GetCustomAttributes(typeof(KernelFunctionAttribute), false).Length > 0)
                   .ToArray();
    }

    public static IEnumerable<FunctionTool> GetAllFunctionTools(Type type)
    {
        var kernelFunctionMethods = GetKernelFunctionMethods(type);

        foreach(var method in kernelFunctionMethods)
        {
            yield return method.ToFunctionTool();
        }
    }
}

public static class MethodInfoExtensions
{
    public static FunctionTool ToFunctionTool(this MethodInfo methodInfo)
    {
        string description = GetFunctionDescription(methodInfo);

        var parameters = GetFunctionParameters(methodInfo);

        return new FunctionTool(new Function(methodInfo.Name, description, new FunctionParametersDefinition(parameters)));
    }


    private static string GetFunctionDescription(MethodInfo methodInfo)
    {
        var descriptionAttribute = methodInfo.GetCustomAttribute<DescriptionAttribute>();
        var description = descriptionAttribute != null ? descriptionAttribute.Description : string.Empty;
        return description;
    }

    private static Dictionary<string, FunctionParameter> GetFunctionParameters(MethodInfo methodInfo) => methodInfo.GetParameters()
            .Select(p =>
            {
                var paramDescriptionAttribute = p.GetCustomAttribute<DescriptionAttribute>();
                var paramDescription = paramDescriptionAttribute != null ? paramDescriptionAttribute.Description : string.Empty;
                return (p.Name, Description: paramDescription, Type: p.ParameterType);
            })
            .ToDictionary(p => p.Name, p => new FunctionParameter(p.Type.Name, p.Description));
}