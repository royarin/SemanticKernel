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

//var kernelBuilder = Kernel.CreateBuilder()
//.AddAzureOpenAIChatCompletion(deploymentName,endpoint,apiKey);

var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromMinutes(10),
};

var kernelBuilder = Kernel.CreateBuilder()
.AddOpenAIChatCompletion(                        // We use Semantic Kernel OpenAI API
        modelId: "phi3",
        apiKey: null,
        endpoint: new Uri("http://localhost:11434"),
        httpClient: httpClient); // With Ollama OpenAI API endpoint


var kernel = kernelBuilder.Build();

// Example 1. Invoke the kernel with a prompt and display the result
var prompt = "What are the things to do in Utrecht?";
var response = await kernel.InvokePromptAsync(prompt);

// Example 2. Invoke the kernel with a templated prompt and display the result
//KernelArguments arguments = new()
//{
//    {
//        "city", "Utrecht"
//    }
//};

//var prompt = "What are the things to do in {{$city}}?";
////var response = await kernel.InvokePromptAsync(prompt, arguments);

//// Example 3. Invoke the kernel with a templated prompt and stream the results to the display
//await foreach (var update in kernel.InvokePromptStreamingAsync(prompt, arguments))
//{
//    Console.Write(update);
//}


Console.WriteLine(response);

//// Create chat history
//var history = new ChatHistory();

//// Get chat completion service
//var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

//// Start the conversation
//Console.Write("User > ");
//string? userInput;
//while ((userInput = Console.ReadLine()) != null)
//{
//    // Add user input
//    history.AddUserMessage(userInput);

//    //// Enable auto function calling
//    //OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
//    //{
//    //    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
//    //};

//    // Get the response from the AI
//    var result = await chatCompletionService.GetChatMessageContentAsync(
//        history,
//        //executionSettings: openAIPromptExecutionSettings,
//        kernel: kernel);

//    // Print the results
//    Console.WriteLine("Assistant > " + result);

//    // Add the message from the agent to the chat history
//    history.AddMessage(result.Role, result.Content ?? string.Empty);

//    // Get user input again
//    Console.Write("User > ");
//}