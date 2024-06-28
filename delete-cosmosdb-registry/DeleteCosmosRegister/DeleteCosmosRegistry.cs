using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace delete_cosmosdb_registry
{
    public class DeleteCosmosRegister : IDeleteCosmosRegister
    {
        private readonly IAuthService authService;
        private readonly ICosmosDbService cosmosDbService;

        public DeleteCosmosRegister(IAuthService authService, ICosmosDbService cosmosDbService)
        {
            this.authService = authService;
            this.cosmosDbService = cosmosDbService;
        }

        public async Task DeleteRegisterAsync()
        {
            string accessToken = await authService.GetAccessTokenAsync();

            bool keepRunning = true;
            while (keepRunning)
            {
                await CheckPermissionsAndDeleteItem(accessToken);

                Console.WriteLine("¿Desea eliminar otro registro? (si/no):");
                string input = Console.ReadLine().Trim().ToLower();
                if (input != "si")
                {
                    keepRunning = false;
                }
            }
        }

        private async Task CheckPermissionsAndDeleteItem(string accessToken)
        {
            bool hasPermission = await cosmosDbService.CheckUserPermissionsAsync(accessToken);

            if (hasPermission)
            {
                Console.WriteLine("Ingrese el ID del item que desea eliminar:");
                string itemId = Console.ReadLine();
                Console.WriteLine("Ingrese la llave de partición del item que desea eliminar:");
                string partitionKey = Console.ReadLine();

                var itemExists = await cosmosDbService.ItemExistsAsync(itemId, partitionKey);
                if (itemExists)
                {
                    Console.WriteLine($"Esta seguro de eliminar el item con id: {itemId} y llave de partición: {partitionKey} (si/no)?");
                    string confirmation = Console.ReadLine();
                    if (confirmation == "si")
                    {
                        await cosmosDbService.DeleteItemAsync(itemId, partitionKey);
                        Console.WriteLine($"Item con ID {itemId} eliminado.");
                    }
                    else
                    {
                        Console.WriteLine($"Proceso de eliminación abortado.");
                    }
                }
                else
                {
                    Console.WriteLine("El item con el ID y llave de partición proporcionados no existe.");
                }
            }
            else
            {
                Console.WriteLine("No tiene permisos para eliminar items de esta base de datos.");
            }
        }
    }
}