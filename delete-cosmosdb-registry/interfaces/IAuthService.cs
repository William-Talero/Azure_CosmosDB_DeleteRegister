namespace delete_cosmosdb_registry;

public interface IAuthService
{
    Task<string> GetAccessTokenAsync();
}