using FluentAssertions;
using Shared.DTOs;
using Shared.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shared.Tests;

public class GitHubLLmClientTests
{
    [Fact]
    public void ParseData()
    {
        var data = File.ReadAllText("./Files/data.txt").Replace(Environment.NewLine, "\n");

        var gitHubLLmClient = new GitHubLLMClient();

        var responses = gitHubLLmClient.ParesStringToResponses(data);

        responses.Count().Should().Be(3);
    }

    [Fact]
    public void NoFunctionCalled()
    {
        var data = File.ReadAllText("./Files/nofunction.txt").Replace(Environment.NewLine, "\n"); ;

        var gitHubLLmClient = new GitHubLLMClient();

        var responses = gitHubLLmClient.GetFunctionsToCall(data);

        responses.Should().BeEmpty();
    }

    [Fact]
    public void GetFunctionsToCall()
    {
        var data = File.ReadAllText("./Files/data.txt").Replace(Environment.NewLine, "\n"); ;
        var function = new CopilotFunction()
        {
            arguments = null,
            name = "ListFilesInCurrentDirectory"
        };

        var gitHubLLmClient = new GitHubLLMClient();

        var responses = gitHubLLmClient.GetFunctionsToCall(data);

        responses.Last().name.Should().Be(function.name);
    }
}