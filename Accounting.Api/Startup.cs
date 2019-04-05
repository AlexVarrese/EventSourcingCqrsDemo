using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AccountingApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AccountingApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Add Swagger
            services.AddSwaggerGen(c =>
            {
                string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info() { Title = "EventSourcingCqrsDemo Accounting API", Version = version });

                // Set the comments path for the Swagger JSON and UI (name and path configured in project settings)
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "Accounting.Api.xml");
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            var databaseSettings = this.Configuration.GetSection("Database").Get<DatabaseSettings>();

            var documentClient = new DocumentClient(new Uri(databaseSettings.Uri), databaseSettings.AccountKey);

            SetupDatabaseAsync(documentClient).GetAwaiter().GetResult();

            // Register application services
            services.AddSingleton(documentClient);
            services.AddScoped<IAccountQuerys, AccountQuerys>();
            services.AddScoped<IAccountCommands, AccountCommands>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            // Configure Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WuH.IoT API");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private async Task SetupDatabaseAsync(DocumentClient documentClient)
        {
            var database = new Database() { Id = Constants.DatabaseName };

            var databaseResponse = await documentClient.CreateDatabaseIfNotExistsAsync(database);
            await documentClient.CreateDocumentCollectionIfNotExistsAsync(databaseResponse.Resource.SelfLink, 
                new DocumentCollection()
                {
                    Id = Constants.EventStoreCollectionName,
                    PartitionKey = new PartitionKeyDefinition()
                    {
                        Paths = new Collection<string>() { "/AggregateId" }
                    }
                });
            await documentClient.CreateDocumentCollectionIfNotExistsAsync(databaseResponse.Resource.SelfLink,
               new DocumentCollection()
               {
                   Id = Constants.ViewsCollectionName,
                   PartitionKey = new PartitionKeyDefinition()
                   {
                       Paths = new Collection<string>() { "/AggregateId" }
                   }
               });
        }
    }
}
