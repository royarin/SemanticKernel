using KernelWithPlugin.Plugins;
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

kernelBuilder.Plugins.AddFromType<WeatherPlugin>();

Kernel kernel= kernelBuilder.Build();

//var temperature = await kernel.InvokeAsync("WeatherPlugin", "GetWeather", new() { { "city", "Utrecht" } });

//Console.WriteLine(temperature);


// Enable auto function calling
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

//KernelArguments arguments = new(openAIPromptExecutionSettings)
//{
//    {
//        "city", "Utrecht"
//    }
//};

var input = "What is the weather in Utrecht?";

//var temp=await kernel.InvokeAsync("WeatherPlugin", "GetWeather", new() {{ "city","Utrecht" }});

var response = await kernel.InvokePromptAsync(input, new(openAIPromptExecutionSettings));

Console.WriteLine(response);



