﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class CompanyContext : DbContext
{
    public DbSet<Company> Companies { get; set; }

    public CompanyContext(DbContextOptions<CompanyContext> options) : base(options)
    {
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