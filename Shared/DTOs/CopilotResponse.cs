using System.Text.Json.Serialization;

namespace Shared.DTOs;

public class CopilotChoice
{
    public int index { get; set; }
    public CopilotDelta delta { get; set; }
    public object logprobs { get; set; }
    public object finish_reason { get; set; }
}

public class CopilotDelta
{
    public string role { get; set; }

    [JsonPropertyName("tool_calls")]
    public List<CopilotToolCall> toolCalls { get; set; }
}

public class CopilotToolCall
{
    public CopilotFunction function { get; set; }
    public string id { get; set; }
    public int index { get; set; }
    public string type { get; set; }
}

public class CopilotFunction
{
    public string arguments { get; set; }
    public string name { get; set; }
}


public class CopilotResponse
{
    public string id { get; set; } = "chatcmpl-123";
    public string @object { get; set; }
    public long created { get; set; } = DateTime.Now.Ticks;
    public string model { get; set; } = "gpt-3.5-turbo-0125";
    public string system_fingerprint { get; set; } = "fp_44709d6fcb";
    public List<CopilotChoice> choices { get; set; }
}

