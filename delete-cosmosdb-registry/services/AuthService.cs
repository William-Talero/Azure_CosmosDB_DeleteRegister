using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

namespace delete_cosmosdb_registry;

public class AuthService : IAuthService
{
    private readonly string clientId;
    private readonly string authority;
    private static IPublicClientApplication app;

    public AuthService(IConfiguration configuration)
    {
        clientId = configuration["clientId"];
        var tenantId = configuration["tenantId"];
        authority = $"https://login.microsoftonline.com/{tenantId}";

        app = PublicClientApplicationBuilder.Create(clientId)
                                             .WithAuthority(authority)
                                             .WithDefaultRedirectUri()
                                             .Build();
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var accounts = await app.GetAccountsAsync();
        AuthenticationResult result;
        try
        {
            result = await app.AcquireTokenSilent(new[] { "User.Read" }, accounts.FirstOrDefault())
                              .ExecuteAsync();
        }
        catch (MsalUiRequiredException)
        {
            result = await app.AcquireTokenInteractive(new[] { "User.Read" })
                              .ExecuteAsync();
        }
        return result.AccessToken;
    }
}