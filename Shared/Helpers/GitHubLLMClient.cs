using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shared.Helpers;

public class GitHubLLMClient
{
    private HttpClient _httpClient;
    public GitHubLLMClient()
    {
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(@"https://api.githubcopilot.com")
        };
    }

    public async Task<HttpResponseMessage> PostAsync(string tokenForUser, List<Message> messages)
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenForUser}");
        var copilotLLMResponse = await _httpClient.PostAsync(
            "/chat/completions",
            new StringContent(
                JsonSerializer.Serialize(
                    new { messages, stream = true }),
                    System.Text.Encoding.UTF8,
                    "application/json")
        );
        return copilotLLMResponse;
    }

    public IEnumerable<CopilotResponse> ParesStringToResponses(string response)
    {
        var jsonStrings = SplitIntoJsonStrings(response).ToList();
        var copilotResponse = new List<CopilotResponse>();
        foreach (var jsonString in jsonStrings.Skip(1).Take(jsonStrings.Count -2))
        {

            var cR = JsonSerializer.Deserialize<CopilotResponse>(jsonString);
            if (cR != null)
            {
                copilotResponse.Add(cR);
            }
        }

        return copilotResponse;
    }

    private static IEnumerable<string> SplitIntoJsonStrings(string input)
    {
        var splits = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        return splits.Select(s => s.TrimStart("data:".ToCharArray()).Trim());
    }

    public async Task<HttpContent> ChatCompletionsAsync(string apiKey, ChatCompletionsRequest request, string integrationID = null)
    {
        var body = JsonSerializer.Serialize(request);
        var content = new StringContent(body, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        if (!string.IsNullOrEmpty(integrationID))
        {
            _httpClient.DefaultRequestHeaders.Add("Copilot-Integration-Id", integrationID);
        }

        var response = await _httpClient.PostAsync("/chat/completions", content);

        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            throw new Exception($"Unexpected status code: {response.StatusCode}, Response: {responseBody}");
        }

        return response.Content;
    }

    public async Task<EmbeddingsResponse> EmbeddingsAsync(string token, EmbeddingsRequest request, string integrationID = null)
    {
        var body = JsonSerializer.Serialize(request);
        var content = new StringContent(body, Encoding.UTF8, "application/json");

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.githubcopilot.com/embeddings")
        {
            Content = content
        };
        httpRequest.Headers.Add("Accept", "application/json");
        httpRequest.Headers.Add("Authorization", $"Bearer {token}");
        if (!string.IsNullOrEmpty(integrationID))
        {
            httpRequest.Headers.Add("Copilot-Integration-Id", integrationID);
        }

        var response = await _httpClient.SendAsync(httpRequest);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            throw new Exception($"Unexpected status code: {response.StatusCode}, Response: {responseBody}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<EmbeddingsResponse>(responseContent);
    }

    public IEnumerable<CopilotFunction> GetFunctionsToCall(string data)
    {
        var responses = ParesStringToResponses(data);

        return responses.Where(r => r.choices.Any(c => c.delta?.role != null))
             .SelectMany(r => r.choices.SelectMany(c => c.delta.toolCalls.Select(tC => tC.function)));
            
    }
}

public class ChatRequest
{
    public List<ChatMessage> Messages { get; set; }
}

public class ChatMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }
}

public enum Model
{
    GPT35Turbo,
    GPT4,
    TextEmbeddingAda002
}

public class ChatCompletionsRequest
{
    [JsonPropertyName("messages")]
    public List<ChatMessage> Messages { get; set; }

    [JsonPropertyName("model")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Model Model { get; set; }

    [JsonPropertyName("stream")]
    public bool Stream { get; set; }

    [JsonPropertyName("tools")]
    public List<FunctionTool> Tools { get; set; }
}

public class EmbeddingsRequest
{
    [JsonPropertyName("model")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Model Model { get; set; }
    public List<string> Input { get; set; }
}

public class EmbeddingsResponse
{
    public List<EmbeddingsResponseData> Data { get; set; }
    public EmbeddingsResponseUsage Usage { get; set; }
}

public class EmbeddingsResponseData
{
    public List<float> Embedding { get; set; }
    public int Index { get; set; }
}

public class EmbeddingsResponseUsage
{
    public int PromptTokens { get; set; }
    public int TotalTokens { get; set; }
}