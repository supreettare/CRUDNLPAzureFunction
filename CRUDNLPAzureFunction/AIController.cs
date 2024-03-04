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
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using CRUDNLPAzureFunction.Services.Interface;
using System.Runtime.CompilerServices;

namespace CRUDNLPAzureFunction
{
    public class AIController
    {
        private readonly IKernelBase _kernelBase;

        public AIController(IKernelBase kernelBase)
        {
            _kernelBase = kernelBase;
        }

        [FunctionName("NLPChat")]
        public async Task<string> NLPChat([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            string result = "";

            try
            {
                var kernel = _kernelBase.CreateKernel();

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
