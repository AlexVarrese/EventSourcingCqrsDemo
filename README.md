# EventSourcingCqrsDemo
A sample implementation of the Event Sourcing and CQRS pattern using ASP.NET Core, Azure Functions and Azure Cosmos DB

## Getting Started

1. Install Visual Studio 2019
2. Install Azure Cosmos Db Emulator (https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator)
3. Start Azure Cosmos Db Emulator
4. Configure multiple startup projects in Visual Visual:
   - Accounting.Api
   - Accounting.EventsProcessingFunction
5. Start debug
6. A browser instance with Swagger should pop up enabling you to test the application

