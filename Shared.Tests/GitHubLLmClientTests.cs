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

        var function = gitHubLLmClient.GetFunctionsToCall(data);

        function.Should().BeNull();
    }

    [Fact]
    public void Parse_Arguments()
    {
        var pathKey = "directoryPath";
        var data = File.ReadAllText("./Files/functionarguments.txt").Replace(Environment.NewLine, "\n"); ;

        var gitHubLLmClient = new GitHubLLMClient();

        var responses = gitHubLLmClient.GetArguments(data);

        responses.Keys.Should().Contain(pathKey);
    }

    [Fact]
    public void Parse_FunctionWithArguments()
    {
        var pathKey = "directoryPath";
        var data = File.ReadAllText("./Files/functionarguments.txt").Replace(Environment.NewLine, "\n"); ;

        var gitHubLLmClient = new GitHubLLMClient();

        var function = gitHubLLmClient.GetFunctionsToCall(data);
        function.Parameters.Should().NotBeEmpty();
        function.Parameters.Keys.Should().Contain(pathKey);
    }

    [Fact]
    public void Parse_ListFilesInCurrentDirectory()
    {
        var data = File.ReadAllText("./Files/data.txt").Replace(Environment.NewLine, "\n"); ;
        var function = new CopilotFunction()
        {
            Arguments = null,
            Name = "ListFilesInCurrentDirectory"
        };

        var gitHubLLmClient = new GitHubLLMClient();

        var responses = gitHubLLmClient.GetFunctionsToCall(data);

        responses.Name.Should().Be(function.Name);
    }

    [Fact]
    public void Parse_ListFilesInCurrentDirectory_NoArguments()
    {
        var data = File.ReadAllText("./Files/data.txt").Replace(Environment.NewLine, "\n"); ;

        var gitHubLLmClient = new GitHubLLMClient();

        var arguments = gitHubLLmClient.GetArguments(data);

        arguments.Should().BeEmpty();
    }
}