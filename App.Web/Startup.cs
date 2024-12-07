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

        // Prepopulates company data
        using (var scope = app.ApplicationServices.CreateScope())
        {
            if (ciks != null)
                await scope.ServiceProvider.GetRequiredService<IEdgarFundingCalculatorService>()
                    .PopulateCompanyData(ciks);
        }
        Console.WriteLine("Data Population step complete, Application ready for queries.");

        // Configure the HTTP request pipeline.
        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseEndpoints(e =>
        {
            e.MapGet("/companies", (string? firstLetter, IEdgarFundingCalculatorService s) =>
            {
                return s.GetCompanyInfo(firstLetter);
            });
        });
    }
}