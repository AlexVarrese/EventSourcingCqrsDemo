using Accounting.EventsProcessingFunction;
using Accounting.Services;
using AccountingApi.Services;
using MediatR;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;

[assembly: WebJobsStartup(typeof(Startup))]
namespace Accounting.EventsProcessingFunction
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var cosmosDbConnectionString = Environment.GetEnvironmentVariable("CosmosDbConnection", EnvironmentVariableTarget.Process);
            builder.Services.AddSingleton<DocumentClient>(_ => 
                CreateDocumentClientFromConnectionString(cosmosDbConnectionString));
            builder.Services.AddScoped<IAccountQuerys, AccountQuerys>();
            builder.Services.AddScoped<IAccountingEventHandlers, AccountingEventHandlers>();
        }

        /// <summary>
        /// Parses a Cosmos DB Connection String and creates a DocumentClient.
        /// Based on https://github.com/Azure/azure-documentdb-dotnet/issues/203
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private DocumentClient CreateDocumentClientFromConnectionString(string connectionString)
        {
            IDictionary<string, string> settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string[] splitted = connectionString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string nameValue in splitted)
            {
                string[] splittedNameValue = nameValue.Split(new char[] { '=' }, 2);

                if (splittedNameValue.Length != 2)
                {
                    throw new ArgumentException("Settings must be of the form \"name=value\".");
                }

                if (settings.ContainsKey(splittedNameValue[0]))
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Duplicate setting '{0}' found.", splittedNameValue[0]));
                }

                settings.Add(splittedNameValue[0], splittedNameValue[1]);
            }

            var documentClient = new DocumentClient(new Uri(settings["AccountEndpoint"]), settings["AccountKey"]);
            return documentClient;
        }
    }
}
