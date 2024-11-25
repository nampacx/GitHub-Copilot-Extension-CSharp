using FluentAssertions;
using Shared.DTOs;
using Shared.Helpers;

namespace Shared.Tests;

public class GitHubLLmClientTests
{
    [Fact]
    public void ParseData()
    {
        var data = File.ReadAllText("./Files/data.txt");

        var gitHubLLmClient = new GitHubLLMClient();

        var responses = gitHubLLmClient.ParesStringToResponses(data);

        responses.Count().Should().Be(3);
    }


    [Fact]
    public void GetFunctionsToCall()
    {
        var data = File.ReadAllText("./Files/data.txt");
        var function = new CopilotFunction()
        {
            arguments = null,
            name = "ListFilesInCurrentDirectory"
        };

        var gitHubLLmClient = new GitHubLLMClient();

        var responses = gitHubLLmClient.GetFunctionsToCall(data).ToList();

        responses.Last().name.Should().Be(function.name);
    }
}