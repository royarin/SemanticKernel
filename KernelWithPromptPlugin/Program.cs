
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var endpoint = configuration["AzureOpenAI:Endpoint"];
var apiKey = configuration["AzureOpenAI:ApiKey"];
var deploymentName = configuration["AzureOpenAI:DeploymentName"];

var kernelBuilder = Kernel.CreateBuilder()
.AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);

kernelBuilder.Plugins.AddFromPromptDirectory("Plugins/Prompts");

Kernel kernel = kernelBuilder.Build();

// Enable auto function calling
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

var prompt = "What are the things to do in Utrecht?";
var response = await kernel.InvokePromptAsync(prompt, new(openAIPromptExecutionSettings));

Console.WriteLine(response);