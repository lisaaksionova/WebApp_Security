using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using WebApp_UnderTheHood.Authorization;
using WebApp_UnderTheHood.Dto;
using WebApp_UnderTheHood.Pages.Accounts;

namespace WebApp_UnderTheHood.Pages;

[Authorize(Policy = "HRManagerOnly")]
public class HRManager : PageModel
{
    public HRManager(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    public readonly IHttpClientFactory httpClientFactory;

    public List<WeatherForecastDto>? WeatherForecastItems { get; set; }

   
    public async Task OnGetAsync()
    {
        // get token from session
        JwtToken token = new JwtToken();

        var strTokenObj = HttpContext.Session.GetString("access_token");
        if (strTokenObj == null)
        {
            token = await Authenticate();
        }
        else
        {
            token = JsonConvert.DeserializeObject<JwtToken>(strTokenObj)??new JwtToken();
        }

        if (token == null || string.IsNullOrWhiteSpace(token.AccessToken) || token.ExpireAt <= DateTime.UtcNow)
        {
            token = await Authenticate();
        }
        var client = httpClientFactory.CreateClient("OurWebApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken ?? string.Empty);
        WeatherForecastItems = await client.GetFromJsonAsync<List<WeatherForecastDto>>("WeatherForecast");

    }
    private async Task<JwtToken> Authenticate()
    {
        var client = httpClientFactory.CreateClient("OurWebApi");
        var res = await client.PostAsJsonAsync("auth", new Credential { Username = "admin", Password = "password" });
        res.EnsureSuccessStatusCode();
        string jwt = await res.Content.ReadAsStringAsync();
        HttpContext.Session.SetString("access_token", jwt);
        return JsonConvert.DeserializeObject<JwtToken>(jwt) ?? new JwtToken();
    }
}