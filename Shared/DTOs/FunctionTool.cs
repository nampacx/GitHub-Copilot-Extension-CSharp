using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;

namespace Shared.DTOs;

public record FunctionTool
{
    public FunctionTool(Function Function)
    {
        this.Function = Function;
    }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "function";

    [JsonPropertyName("function")]
    public Function Function { get; }
}

public record Function
{
    public Function(string name, string description, FunctionParametersDefinition parameters, MethodInfo methodInfo)
    {
        Name = name;
        Description = description;
        Parameters = parameters;
        MethodInfo = methodInfo;
    }

    [JsonIgnore]
    public MethodInfo MethodInfo{ get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("parameters")]
    public FunctionParametersDefinition Parameters { get; set; }
}

public record FunctionParametersDefinition
{
    public FunctionParametersDefinition(Dictionary<string, FunctionParameter> properties, List<string> required)
    {
        Properties = properties;
        Required = required;
    }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";

    [JsonPropertyName("properties")]
    public Dictionary<string, FunctionParameter> Properties { get; set; }

    [JsonPropertyName("required")]
    public List<string> Required { get; set; } = new List<string>();
}

public record FunctionParameter
{
    public FunctionParameter(string type, string description)
    {
        Type = type;
        Description = description;
    }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}