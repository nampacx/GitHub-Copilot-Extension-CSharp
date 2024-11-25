using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernel.Connector
{
    public class ToolExecution
    {
        public static object ExecuteFunction(MethodInfo methodInfo)
        {
            var instance = Activator.CreateInstance(methodInfo.DeclaringType);

            return methodInfo.Invoke(instance, null);
        }

        public static object ExecuteFunction(FunctionTool functionTool)
        {
            var methodInfo = functionTool.Function.MethodInfo;

            var instance = Activator.CreateInstance(methodInfo.DeclaringType);

            return methodInfo.Invoke(instance, null);
        }
    }
}
