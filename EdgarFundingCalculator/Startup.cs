using App.Data;
using App.Web;
using Microsoft.EntityFrameworkCore;
using static Program;

namespace FundingCalculatorApi;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Entity Framework SQLLite Config
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var DbPath = Path.Join(path, "edgar.db");
        services.AddDbContext<EdgarContext>(options =>
            options.UseSqlite($"Data Source={DbPath}"));

        // HttpClient for Edgar with required default headers
        var client = new HttpClient();
        client.BaseAddress = new Uri("https://data.sec.gov/");
        client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.34.0");
        client.DefaultRequestHeaders.Add("Accept", "*/*");

        // SQLLite DbContext will be injected here
        services.AddScoped<ICompanyRepository, CompanyRepository>();

        // Repo and HttpClient will be injected here
        services.AddScoped<IEdgarFundingCalculatorService, EdgarFundingCalculatorService>();
    }

    public async void Configure(IApplicationBuilder app)
    {
        var ciks = new List<int>() { 1 };
        await app.ApplicationServices.GetRequiredService<IEdgarFundingCalculatorService>()
            .PopulateCompanyData(ciks);

        // Configure the HTTP request pipeline.
        app.UseHttpsRedirection();

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        app.UseEndpoints(e =>
        {

            e.MapGet("/weatherforecast", () =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    (
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    ))
                    .ToArray();
                return forecast;
            });
        });
    }
}