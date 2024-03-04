using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDNLPAzureFunction.Services.Interface
{
    public class KernelBase : IKernelBase
    {
        public Kernel CreateKernel()
        {
            const string azureOpenAIDeploymentName = "";
            const string azureOpenAIEndpoint = "";
            const string azureOpenAIAPIKey = "";

            var builder = Kernel.CreateBuilder();

            builder.Plugins.AddFromType<WeatherService>();
            builder.Services.AddAzureOpenAIChatCompletion(azureOpenAIDeploymentName, azureOpenAIEndpoint, azureOpenAIAPIKey);
            return builder.Build();
        }
    }
}
