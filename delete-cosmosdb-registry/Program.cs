using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace delete_cosmosdb_registry
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddSingleton<IAuthService, AuthService>();
            serviceCollection.AddSingleton<ICosmosDbService, CosmosDbService>();
            serviceCollection.AddSingleton<IDeleteCosmosRegister, DeleteCosmosRegister>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var deleteCosmosRegister = serviceProvider.GetService<IDeleteCosmosRegister>();
            await deleteCosmosRegister.DeleteRegisterAsync();
        }
    }
}