using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp_UnderTheHood.Authorization;
using WebApp_UnderTheHood.DTO;
using WebApp_UnderTheHood.Pages.Account;

namespace WebApp_UnderTheHood.Pages
{
    [Authorize(Policy = "HRManagerOnly")]
    public class HRManagerModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty]
        public List<WeatherForecastDTO> WeatherForecastItems { get; set; }

        public HRManagerModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            WeatherForecastItems = await InvokeEndPoint<List<WeatherForecastDTO>>("OurWebAPI", "WeatherForecast");
        }

        private async Task<T> InvokeEndPoint<T>(string clientName, string url)
        {
            // get token from session
            JwtToken token = null;

            var strTokenObject = HttpContext.Session.GetString("access_token");

            if (string.IsNullOrWhiteSpace(strTokenObject))
            {
                token = await Authenticate();
            }
            else
            {
                token = JsonSerializer.Deserialize<JwtToken>(strTokenObject);
            }

            if (token == null ||
                string.IsNullOrWhiteSpace(token.AccessToken) ||
                token.ExpiresAt <= DateTime.UtcNow)
                token = await Authenticate();

            var httpClient = _httpClientFactory.CreateClient(clientName);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            return await httpClient.GetFromJsonAsync<T>(url);

        }

        private async Task<JwtToken> Authenticate()
        {
            //authentication and getting the token
            var httpClient = _httpClientFactory.CreateClient("OurWebAPI");

            var response =
                await httpClient.PostAsJsonAsync("auth",
                    new Credential { UserName = "admin", Password = "password" });

            response.EnsureSuccessStatusCode();
            
            var stringJwt = await response.Content.ReadAsStringAsync();
            HttpContext.Session.SetString("access_token", stringJwt);

            return JsonSerializer.Deserialize<JwtToken>(stringJwt);
        }
    }
    
}
