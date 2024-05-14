using KernelWithPlanner.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Planning.Handlebars;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var endpoint = configuration["AzureOpenAI:Endpoint"];
var apiKey = configuration["AzureOpenAI:ApiKey"];
var deploymentName = configuration["AzureOpenAI:DeploymentName"];

var kernelBuilder = Kernel.CreateBuilder()
.AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);

//var httpClient = new HttpClient
//{
//    Timeout = TimeSpan.FromMinutes(10),
//};

//var kernelBuilder = Kernel.CreateBuilder()
//.AddOpenAIChatCompletion(                        // We use Semantic Kernel OpenAI API
//        modelId: "mistral",
//        apiKey: null,
//        endpoint: new Uri("http://localhost:11434"),
//        httpClient: httpClient); // With Ollama OpenAI API endpoint


kernelBuilder.Plugins.AddFromType<WeatherPlugin>();
kernelBuilder.Plugins.AddFromPromptDirectory("Plugins/Prompts");

Kernel kernel = kernelBuilder.Build();

var prompt = "What are the things to do in Utrecht based on the current weather?";

var planner = new HandlebarsPlanner();
var newPlan = await planner.CreatePlanAsync(kernel, prompt);

Console.WriteLine(newPlan);

var planOutput = await newPlan.InvokeAsync(kernel);

Console.WriteLine(planOutput);