using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Planning.Handlebars;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace CRUDNLPAzureFunction
{
    public class AIController
    {
        [FunctionName("NLPChat")]
        public async Task<string> NLPChat([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            string result = "";

            try
            {
                string azureOpenAIDeploymentName = "";
                string azureOpenAIEndpoint = "";
                string azureOpenAIAPIKey = "";

                var builder = Kernel.CreateBuilder();

            //builder.Plugins.AddFromPromptDirectory("./Plugins/CreateToDoPlugin");
            builder.Plugins.AddFromType<WeatherService>();
            //builder.Plugins.AddFromType<IdentifyToDoObjectPlugin>();
            builder.Services.AddAzureOpenAIChatCompletion(azureOpenAIDeploymentName,
                azureOpenAIEndpoint
                , azureOpenAIAPIKey);

            var kernel = builder.Build();

            var arguments = new KernelArguments();

            //7. Enable auto function calling
            OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };

#pragma warning disable SKEXP0003, SKEXP0011, SKEXP0052, SKEXP0060
           
                string input = req.Query["input"];

                var planner = new HandlebarsPlanner(new HandlebarsPlannerOptions(allowLoops: true));

                arguments["input"] = input;

                var originalPlan = await planner.CreatePlanAsync(kernel, input);

                Console.WriteLine(originalPlan);

                result = await originalPlan.InvokeAsync(kernel, new KernelArguments(openAIPromptExecutionSettings));

                Console.WriteLine(originalPlan);
            }
            catch (JsonReaderException ex)
            {

            }
            catch (Exception ex)
            {
            }

            return result;

        }
    }
}
