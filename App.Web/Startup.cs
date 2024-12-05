using App.Data;
using App.Web;
using Microsoft.EntityFrameworkCore;

namespace FundingCalculatorApi;

public class Startup
{
    public IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Entity Framework SQLLite Config
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var DbPath = Path.Join(path, "edgar.db");
        services.AddDbContext<CompanyContext>(options =>
            options.UseSqlite($"Data Source={DbPath}"));

        // SQLLite DbContext will be injected here
        services.AddScoped<ICompanyRepository, CompanyRepository>();

        // Repo and HttpClient will be injected here
        services.AddHttpClient<IEdgarFundingCalculatorService, EdgarFundingCalculatorService>(client =>
        {
            client.BaseAddress = new Uri("https://data.sec.gov/");
            client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.34.0");
            client.DefaultRequestHeaders.Add("Accept", "*/*");
        });
    }

    public async void Configure(IApplicationBuilder app)
    {

        var ciks = Configuration.GetSection("ProvidedCiks").Get<List<int>>();

        using (var scope = app.ApplicationServices.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<IEdgarFundingCalculatorService>()
                .PopulateCompanyData(ciks);
        }

        // Configure the HTTP request pipeline.
        app.UseHttpsRedirection();

        app.UseEndpoints(e =>
        {
            e.MapGet("/companies", (IEdgarFundingCalculatorService s) =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index => s.GetAllCompanyInfo())
                    .ToArray();
                return forecast;
            });
        });
    }
}