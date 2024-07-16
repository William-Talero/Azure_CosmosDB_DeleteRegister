using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace delete_cosmosdb_registry;

public class AuthService : IAuthService
{
    private readonly string clientId;
    private readonly string authority;
    private static IPublicClientApplication app;
    private readonly string[] scopes;

    public AuthService(IConfiguration configuration)
    {
        clientId = configuration["clientId"];
        var tenantId = configuration["tenantId"];
        authority = $"https://login.microsoftonline.com/{tenantId}";

        //var redirectUri = "https://login.microsoftonline.com/common/oauth2/nativeclient";

        app = PublicClientApplicationBuilder.Create(clientId)
                                            .WithAuthority(authority)
                                            .WithDefaultRedirectUri()
                                            .Build();

        scopes = new[] { "https://cosmos.azure.com/user_impersonation" };
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var accounts = await app.GetAccountsAsync();
        AuthenticationResult result;

        try
        {
            result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                              .ExecuteAsync();
        }
        catch (MsalUiRequiredException)
        {
            try
            {
                result = await app.AcquireTokenInteractive(scopes)
                                  .WithAccount(accounts.FirstOrDefault())
                                  .WithPrompt(Prompt.SelectAccount)
                                  .ExecuteAsync();
            }
            catch (MsalException msalex)
            {
                Console.WriteLine($"Error acquiring token: {msalex.Message}");
                throw;
            }
        }

        return result.AccessToken;
    }
}