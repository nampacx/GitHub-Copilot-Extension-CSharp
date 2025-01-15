using System.Text.Json.Serialization;

namespace Shared.DTOs;

public class CopilotChoice
{
    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("delta")]
    public CopilotDelta Delta { get; set; }

    [JsonPropertyName("logprobs")]
    public object Logprobs { get; set; }

    [JsonPropertyName("finish_reason")]
    public object FinishReason { get; set; }
}

public class CopilotDelta
{
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("tool_calls")]
    public List<CopilotToolCall> Tools { get; set; }
}

public class CopilotToolCall
{
    [JsonPropertyName("function")]
    public CopilotFunction Function { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
}

public class CopilotFunction
{
    [JsonPropertyName("arguments")]
    public string Arguments { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonIgnore]
    public Dictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>();
}


public class CopilotResponse
{
    [JsonPropertyName("Id")]
    public string Id { get; set; } = "chatcmpl-123";


    public string @object { get; set; }

    [JsonPropertyName("created")]
    public long Created { get; set; } = DateTime.Now.Ticks;
    [JsonPropertyName("model")]
    public string Model { get; set; } = "gpt-3.5-turbo-0125";
    [JsonPropertyName("system_fingerprint")]
    public string SystemFingerprint { get; set; } = "fp_44709d6fcb";

    [JsonPropertyName("choices")]
    public List<CopilotChoice> Choices { get; set; }
}

