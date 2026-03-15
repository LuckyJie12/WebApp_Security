using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using WebApp_Security.Authorization;
using WebApp_Security.DTO;
using WebApp_Security.Pages.Account;

namespace WebApp_Security.Pages
{
    public class WeatherForecastModel : PageModel
    {
        private readonly IHttpClientFactory httpClientFactory;

        public WeatherForecastModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        [BindProperty]
        public List<WeatherForecastDTO>? WeatherForecastItems { get; set; }
        public string? ErrorMessage { get; set; }
        public async Task OnGetAsync()
        {
            try
            {
                JwtToken? token = new JwtToken();
                string strTokenObj = HttpContext.Session.GetString("assess_token") ?? string.Empty;
                if (string.IsNullOrEmpty(strTokenObj))
                {
                    token = await Authenticate(token);
                }
                else
                {
                    token = JsonConvert.DeserializeObject<JwtToken>(strTokenObj);
                }
                if (token == null || string.IsNullOrEmpty(token.AccessToken) || token.ExpiresAt <= DateTime.UtcNow)
                {
                    token = await Authenticate(token);
                }
                var client = httpClientFactory.CreateClient("OurWebAPI");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken ?? string.Empty);
                WeatherForecastItems = await client.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast");
            }
            catch (Exception ex)
            {
                ErrorMessage = "无法获取天气数据：" + ex.Message;
                WeatherForecastItems = [];
            }
        }

        private async Task<JwtToken?> Authenticate(JwtToken? token)
        {
            var client = httpClientFactory.CreateClient("OurWebAPI");
            var res = await client.PostAsJsonAsync("Auth", new Credential { Username = "admin", Password = "123" });
            res.EnsureSuccessStatusCode();
            string jsonJwt = await res.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(jsonJwt))
            {
                HttpContext.Session.SetString("assess_token", jsonJwt);
            }
            token = JsonConvert.DeserializeObject<JwtToken>(jsonJwt);
            return token;
        }
    }
}
