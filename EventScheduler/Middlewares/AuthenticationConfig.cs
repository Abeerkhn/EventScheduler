using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace QMS_DotNetCore_API.Authentication
{
    public static class AuthenticationConfig
    {
        internal static TokenValidationParameters tokenValidationParams;

        public static void ConfigureJwtAuthentication(this IServiceCollection services)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my secret top key"));
            // fo
            //var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
            tokenValidationParams = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                ValidIssuer = null,
                ValidateIssuer = false,
                ValidateLifetime = true,
                ValidAudience = null,
                ValidateAudience = false,
                RequireSignedTokens = true,
                IssuerSigningKey = credentials.Key,
                ClockSkew = TimeSpan.FromMinutes(10)
            };
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParams;
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var accessToken = context.SecurityToken as JwtSecurityToken;
                        var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                        //var Claim = claimsIdentity.Claims.ToList();
                        //string RoleID = Claim[0].Value.ToString();
                        //string TokenID = Claim[1].Value.ToString();

                        //if (RoleID == null)
                        //{
                        //    // return unauthorized //if user no longer exists
                        //    context.Fail("Unauthorized");
                        //}

                        return Task.CompletedTask;
                    }
                };
                //#if PROD || UAT
                //                options.IncludeErrorDetails = False;
                //#elif DEBUG
                //                options.RequireHttpsMetadata = false;
                //#endif
            });
        }
    }
}
