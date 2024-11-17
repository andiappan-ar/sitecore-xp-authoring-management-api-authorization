using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SitecoreXP.AuthoringManagement.Authorization.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class SitecoreController : Controller
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private readonly IConfiguration _configuration;       

        public SitecoreController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task<IActionResult> Index()
        {
            string sitecorePlatformGraphQLURL = _configuration["MyAppSettings:SitecorePlatformGraphQLURL"];
           
            // GraphQL URL and API key (ensure the API key is passed correctly)
            var gqlUrl = sitecorePlatformGraphQLURL;

            // Define the GraphQL query to fetch site information
            var query = @"
        query {
            sites {
                name
            }
        }";

            // Convert the query to JSON format
            var requestContent = new StringContent(JsonConvert.SerializeObject(new { query }), Encoding.UTF8, "application/json");

            // Retrieve the access token from the authentication context
            string accessToken = await HttpContext.GetTokenAsync("access_token");

            // Set up the request headers with the access token
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            // Send the request and get the response
            var response = await _httpClient.PostAsync(gqlUrl, requestContent);

            // Handle the response and check if it was successful
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                // Deserialize the response JSON into a dynamic object (or model)
                var data = JsonConvert.DeserializeObject<dynamic>(responseContent);

                // Pass the response data to the view
                ViewBag.SiteList = data.data.sites; // assuming "data" and "sites" structure in the response
            }
            else
            {
                // In case of an error, store the error message
                ViewBag.SiteList = "Error retrieving site list. Status Code: " + response.StatusCode;
            }

            // Return the view
            return View();
        }
    }
}
