using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var endpoint = configuration["AzureOpenAI:Endpoint"];
var apiKey = configuration["AzureOpenAI:ApiKey"];
var deploymentName = configuration["AzureOpenAI:DeploymentName"];
var azureAISearchEndpoint = configuration["AzureAISearch:Endpoint"];
var azureAISearchApiKey = configuration["AzureAISearch:ApiKey"];
var azureAISearchIndex = configuration["AzureAISearch:Index"];

var kernelBuilder = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);


Kernel kernel = kernelBuilder.Build();

var memoryBuilder = new MemoryBuilder()
    .WithAzureOpenAITextEmbeddingGeneration("text-embedding-ada-002", endpoint, apiKey)
    .WithMemoryStore(new AzureAISearchMemoryStore(azureAISearchEndpoint, azureAISearchApiKey));

var memory = memoryBuilder.Build();

await memory.SaveInformationAsync(collection: azureAISearchIndex, id: "1", text: "Welcome to AI Community Day, where all the AI developers come together to geek out and share what they know about artificial intelligence. It's a chance for us to connect, learn from each other, and get inspired by the latest and greatest in AI. Whether you're a pro at AI or just getting started, this event is all about building our community and shaping the future of AI development. Get ready for a day packed with awesome discussions, session, and networking opportunities. Let's dive into the exciting world of AI together!");
await memory.SaveInformationAsync(collection: azureAISearchIndex, id: "2", text: "AI Community Day will be held on 14 May 2024 between 15:00 to 21:00 at De Fabrique in Utrecht");
await memory.SaveInformationAsync(collection: azureAISearchIndex, id: "3", text: "Full address of the location is: WESTKANAALDIJK 7 3542 DA UTRECHT");
await memory.SaveInformationAsync(collection: azureAISearchIndex, id: "4", text: "This free event is made possible with the help of Microsoft");


//var prompt = "Tell me about ai community day";
//var prompt = "When will Ai Community day be held?";
//var prompt = "What is the address?";
var prompt = "How is this event free?";


var arguments = new KernelArguments()
{
    [TextMemoryPlugin.InputParam] = prompt,
    [TextMemoryPlugin.CollectionParam] = azureAISearchIndex,
    [TextMemoryPlugin.LimitParam] = "2",
    [TextMemoryPlugin.RelevanceParam] = "0.8"
};

// Import the TextMemoryPlugin into the Kernel for other functions
var memoryPlugin = kernel.ImportPluginFromObject(new TextMemoryPlugin(memory));

var searchResponse = await kernel.InvokeAsync(memoryPlugin["Recall"],arguments);

Console.WriteLine(searchResponse);

var RecallFunctionDefinition = @"
You are a chatbot that can have a conversations about any topic including the infromation in the provided context.

provided context: {{$db_record}}

Question: {{$input}}

Answer:
";

OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    Temperature = 0.7
};

KernelArguments RecallFunctionArguments = new(openAIPromptExecutionSettings)
{
    ["input"] = prompt,
    ["db_record"] = searchResponse.GetValue<string>()
};

var aboutMeOracle = kernel.CreateFunctionFromPrompt(RecallFunctionDefinition, openAIPromptExecutionSettings);


var response = await kernel.InvokeAsync(aboutMeOracle, RecallFunctionArguments);

Console.WriteLine(response);



