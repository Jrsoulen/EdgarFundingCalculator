using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class EdgarContext : DbContext
{
    public DbSet<EdgarCompanyInfo> EdgarCompanies { get; set; }

    public EdgarContext(DbContextOptions<EdgarContext> options) : base(options)
    {
    }
}

public class EdgarCompanyInfo
{
    [Key]
    public int Cik { get; set; }
    public required string EntityName { get; set; }
    public required string Frame { get; set; }
    public required string Value { get; set; }
}