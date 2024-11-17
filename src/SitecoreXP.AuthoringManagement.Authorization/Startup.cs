using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CustomIdentity.AuthoringMangement
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            string sitecoreIdentityEndpoint = _configuration["MyAppSettings:SitecoreIdentityEndpoint"];          
            string sitecoreIdentityServerClientId = _configuration["MyAppSettings:SitecoreIdentityServerClientId"];

            services.AddMvc();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    options.Cookie.Name = "mvcimplicit";
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.ClientId = sitecoreIdentityServerClientId;
                    options.Authority = sitecoreIdentityEndpoint;
                    options.RequireHttpsMetadata = false;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.ResponseType = "code token";

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("sitecore.profile");
                    //options.Scope.Add("offline_access");
                    options.Scope.Add("sitecore.profile.api");

                    options.SaveTokens = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = JwtClaimTypes.Name,
                        RoleClaimType = JwtClaimTypes.Role,
                    };


                   
                });

            // Register HttpClient for Dependency Injection
            services.AddHttpClient();

            


        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
            CookiePolicyOptions cookiePolicy = new CookiePolicyOptions() { Secure = CookieSecurePolicy.Always };
            app.UseCookiePolicy(cookiePolicy);
        }
    }
}
