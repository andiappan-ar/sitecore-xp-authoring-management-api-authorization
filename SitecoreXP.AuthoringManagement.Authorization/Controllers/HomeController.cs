using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CustomIdentity.AuthoringMangement.Controllers
{
    [Authorize]
    public class Home : Controller
    {
        private readonly IConfiguration _configuration;

        private static readonly HttpClient HttpClient = new HttpClient();

        public Home(IConfiguration configuration)
        {
            _configuration = configuration;
        }        

        public async Task<IActionResult> Index()
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            string refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            return Content($"Current user: <span id=\"UserIdentityName\">{User.Identity.Name ?? "anonymous"}</span><br/>" +
                $"<div>Access token: {accessToken}</div><br/>" +
                $"<div>Refresh token: {refreshToken}</div><br/>"
                , "text/html");
        }

        [Route("/callapi")]
        public async Task<IActionResult> CallApi()
        {
            string currentAppIdentityEndpoint = _configuration["MyAppSettings:CurrentAppIdentityEndpoint"];
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            var request = new HttpRequestMessage(HttpMethod.Get, currentAppIdentityEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await HttpClient.SendAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return Content(response.ToString());
            }

            return Content($"{await response.Content.ReadAsStringAsync()}");
        }

        [Route("/exchange")]
        public async Task<IActionResult> Exchange()
        {
            string sitecoreIdentityEndpoint = _configuration["MyAppSettings:SitecoreIdentityEndpoint"];
            var disco = await DiscoveryClient.GetAsync(sitecoreIdentityEndpoint);
            if (disco.IsError) throw new Exception(disco.Error);
            string sitecoreIdentityServerClientId = _configuration["MyAppSettings:SitecoreIdentityServerClientId"];
            var tokenClient = new TokenClient(disco.TokenEndpoint, sitecoreIdentityServerClientId, "secret");
            var rt = await HttpContext.GetTokenAsync("refresh_token");
            var tokenResult = await tokenClient.RequestRefreshTokenAsync(rt);
            if (!tokenResult.IsError)
            {
                var expiresAt = (DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn)).ToString("o", CultureInfo.InvariantCulture);
                var authService = HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();
                AuthenticateResult authenticateResult = await authService.AuthenticateAsync(HttpContext, null);
                AuthenticationProperties properties = authenticateResult.Properties;
                properties.UpdateTokenValue(OpenIdConnectParameterNames.RefreshToken, tokenResult.RefreshToken);
                properties.UpdateTokenValue(OpenIdConnectParameterNames.AccessToken, tokenResult.AccessToken);
                properties.UpdateTokenValue(OpenIdConnectParameterNames.ExpiresIn, expiresAt);
                await authService.SignInAsync(HttpContext, null, authenticateResult.Principal, authenticateResult.Properties);
                return Redirect("/");
            }
            return BadRequest();
        }
    }

}
