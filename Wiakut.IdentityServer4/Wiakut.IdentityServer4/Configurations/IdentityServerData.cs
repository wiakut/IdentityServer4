using IdentityServer4.Models;

namespace Wiakut.IdentityServer4.Configurations;

public class IdentityServerData
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource
            {
                Name = "role",
                UserClaims = new List<string> { "role" }
            }
        };
    
    public static IEnumerable<ApiScope> ApiScopes =>
        new[] { new ApiScope("WiakutApiScope") };

    public static IEnumerable<ApiResource> ApiResources =>
        new[]
        {
            new ApiResource("WiakutResource")
            {
                Scopes = new List<string> { "WiakutApiScope" },
                ApiSecrets = new List<Secret> { new ("WiakutApiSecret".Sha256()) },
                UserClaims = new List<string> { "role" }
            }
        };

    public static IEnumerable<Client> Clients =>
        new[]
        {
            new Client
            {
                ClientId = "WiakutClient1",
                ClientName = "WiakutClientName",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = new List<Secret> { new ("WiakutClient1Secret".Sha256()) },
                AllowedScopes = { "WiakutApiScope" }
            },
            new Client
            {
                ClientId = "WiakutClient2",
                ClientSecrets = new List<Secret> { new ("WiakutClient2Secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                RedirectUris = { "https://localhost:7054/signin-oidc" },
                FrontChannelLogoutUri = "https://localhost:7054/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:7054/signout-callback-oidc" },
                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "WiakutApiScope" }
            }
        };
}