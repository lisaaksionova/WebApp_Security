using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp_UnderTheHood.Dto;

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
        var client = httpClientFactory.CreateClient("OurWebApi");
        WeatherForecastItems = await client.GetFromJsonAsync<List<WeatherForecastDto>>("WeatherForecast");
        
    }
}