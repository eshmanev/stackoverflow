using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

namespace StackOverflow.Answers.AspNet.WebApi.UseJwtAndAzureADTokensSimultaneously;

/// <summary>
/// How to use JWT Token Authorization and Azure AZ Token Authorization Simultaneously in DOTNET Core
/// </summary>
/// <see href="https://stackoverflow.com/questions/76719671/how-to-use-jwt-token-authorization-and-azure-az-token-authorization-simultaneous/76720378#76720378"/>
public static class WebApplicationBuilderExensions
{
    public static void ConfigureJwtTokens(this WebApplicationBuilder builder)
    {
        builder.Services
            // specify default schema
            .AddAuthentication(AzureADDefaults.BearerAuthenticationScheme)
            // add JWT Bearer
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    //ValidIssuer = tokenOptions.Issuer,
                    //ValidAudience = tokenOptions.Audience,
                    //IssuerSigningKey = signingConfigurations.Key,
                    ClockSkew = TimeSpan.Zero
                };
            })
            // add AzureAD
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"), AzureADDefaults.BearerAuthenticationScheme)
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddInMemoryTokenCaches();
    }
}
