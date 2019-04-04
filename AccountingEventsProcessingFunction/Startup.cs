using Accounting.EventsProcessingFunction;
using AccountingApi.Services;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(Startup))]
namespace Accounting.EventsProcessingFunction
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.AddScoped<DocumentClient>(_ => new DocumentClient(new System.Uri("https://escqrs-demo-store.documents.azure.com:443"), "hbMYQAtMGDLHZsLIoWew5S1nnMjZLlL2OgRAv6MpR8vFkIqKYSzNaSteM6KBGNt9gs7C9BT0A43CMPokkkFYqA=="));
            builder.Services.AddScoped<IAccountQuerys, AccountQuerys>();
        }
    }
}
