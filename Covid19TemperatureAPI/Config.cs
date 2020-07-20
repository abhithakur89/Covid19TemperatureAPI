using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19TemperatureAPI
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApis(string apiName, string clientSecret)
        {
            return new ApiResource[]
            {
                new ApiResource(apiName, "covid19temperatureapi")
                {
                    ApiSecrets =
                    {
                        new Secret(clientSecret.Sha256())
                    }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<Client> GetClients(string apiClientId, string clientSecret, string apiName)
        {
            return new[]
            {
                new Client
                {
                    ClientId = apiClientId,
                    AccessTokenType = AccessTokenType.Reference,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret(clientSecret.Sha256()) },
                    RequireClientSecret = true,
                    AllowAccessTokensViaBrowser = false,

                    AllowedScopes = { apiName },
                    RequireConsent = false,
                    AllowOfflineAccess = false,

                    AccessTokenLifetime=31536000

                }
            };
        }
    }
}
