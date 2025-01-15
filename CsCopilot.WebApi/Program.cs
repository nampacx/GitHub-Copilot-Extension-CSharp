using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextToImage;
using Nampacx.Copilot.WebApi.Services;
using Shared.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(b => b.AddConsole());

builder.Services.AddSingleton<GitHubLLMClient>();
builder.Services.AddSingleton<FunctionCallingService>();

#pragma warning disable SKEXP0001, SKEXP0010
builder.Services.AddSingleton(b =>
{
    var config = b.GetRequiredService<IConfiguration>();
    var builder = Kernel.CreateBuilder();
    builder.AddAzureOpenAITextToImage(
        config["TextToImageModel"],
        config["OpenAIEndpoint"],
        config["OpenAIKey"]);

    var kernel = builder.Build();

    return kernel.GetRequiredService<ITextToImageService>();
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
