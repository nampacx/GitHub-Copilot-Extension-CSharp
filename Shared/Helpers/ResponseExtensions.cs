using Microsoft.AspNetCore.Http;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shared.Helpers
{
    public static class ResponseExtensions
    {
        public static async Task SendGitHubLLMResponseAsync(this HttpResponse httpResponse, string message, bool streamed = true)
        {
            if (streamed)
            {
                await httpResponse.WriteAsync(message);
            }
            else
            {
                await httpResponse.WriteAsync($"data: {message}\n\n");
            }
        }

        public static async Task SendAsSSEResponseAsync(this HttpResponse httpResponse, string message)
        {
            var data = new { choices = new[] { new { index = 0, delta = new { role = "assistant", content = message } } } };

            await httpResponse.WriteAsync($"data: {JsonSerializer.Serialize(data)}\n\n");
            await httpResponse.Body.FlushAsync();
        }
    }
}
    
