// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Choice
{
    public int index { get; set; }
    public Delta delta { get; set; }
    public object logprobs { get; set; }
    public object finish_reason { get; set; }
}

public class Delta
{
    public string role { get; set; }
    public string content { get; set; }
}

public class CopilotResponse
{
    public string id { get; set; } = "chatcmpl-123";
    public string @object { get; set; }
    public long created { get; set; }
    public string model { get; set; } = "gpt-3.5-turbo-0125";
    public string system_fingerprint { get; set; } = "fp_44709d6fcb";
    public List<Choice> choices { get; set; }
}

