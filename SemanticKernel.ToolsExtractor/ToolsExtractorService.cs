using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Reflection;

namespace SemanticKernel.ToolsExtractor;

public class ToolsExtractorService
{
    public static (MethodInfo Method, string Description)[] GetKernelFunctionMethods(Type type)
    {
        return type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                   .Where(m => m.GetCustomAttributes(typeof(KernelFunctionAttribute), false).Length > 0)
                   .Select(m =>
                   {
                       var descriptionAttribute = m.GetCustomAttribute<DescriptionAttribute>();
                       string description = descriptionAttribute != null ? descriptionAttribute.Description : string.Empty;
                       return (Method: m, Description: description);
                   })
                   .ToArray();
    }
}
