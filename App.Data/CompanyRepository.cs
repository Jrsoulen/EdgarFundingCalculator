﻿namespace App.Data;

public class CompanyRepository : ICompanyRepository
{
    private CompanyContext _dbContext;
    public CompanyRepository(CompanyContext context)
    {
        _dbContext = context;
    }
    public Company CreateEdgarCompanyInfo(Company entity)
    {
        _dbContext.Add(entity);
        _dbContext.SaveChanges();
        return entity;
    }
    public Company? GetEdgarCompanyInfo(int id)
    {
        return _dbContext.Companies
            .Where(b => b.Cik == id)
            .FirstOrDefault();
    }
    public Company UpdateEdgarCompanyInfo(Company entity)
    {
        var company = _dbContext.Companies
            .Where(b => b.Cik == entity.Cik)
            .FirstOrDefault();

        if (company is null) throw new ArgumentException($"Company with Cik {entity.Cik} does not exist");

        company.EntityName = entity.EntityName;

        _dbContext.SaveChanges();

        return company;
    }
    public void DeleteCompany(int id)
    {
        var company = _dbContext.Companies
            .Where(b => b.Cik == id)
            .FirstOrDefault();

        if (company is null) throw new ArgumentException($"Company with Cik {id} does not exist");

        _dbContext.Remove(company);
        _dbContext.SaveChanges();
    }
}

public interface ICompanyRepository
{
    Company CreateEdgarCompanyInfo(Company entity);
    Company? GetEdgarCompanyInfo(int id);
    Company UpdateEdgarCompanyInfo(Company entity);
    void DeleteCompany(int id);
}
