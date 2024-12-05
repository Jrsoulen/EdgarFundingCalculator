using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class EdgarContext : DbContext
{
    public DbSet<EdgarCompanyInfo> EdgarCompanies { get; set; }

    public string DbPath { get; }

    public EdgarContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "edgar.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}

public class EdgarCompanyInfo
{
    [Key]
    public int Cik { get; set; }
    public required string EntityName { get; set; }
    public required string Frame { get; set; }
    public required string Value { get; set; }
}