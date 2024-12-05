using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class CompanyContext : DbContext
{
    public DbSet<Company> Companies { get; set; }

    public CompanyContext(DbContextOptions<CompanyContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var DbPath = Path.Join(path, "edgar.db");
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }

}

public class Company
{
    // Class built from spec reference:
    // You will be interested in “cik”, “entityName”, and (...) the “val”, “form”, and “frame” fields
    [Key]
    public int Cik { get; set; }
    public required string EntityName { get; set; }
    public required string Frame { get; set; }
    public required decimal Value { get; set; }
}