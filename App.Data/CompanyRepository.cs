using Microsoft.EntityFrameworkCore;

namespace App.Data;

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
        foreach (var yearlyNetIncomeLoss in entity.YearlyNetIncomeLosses)
        {
            _dbContext.YearNetIncomeLoss.Add(yearlyNetIncomeLoss);
        }
        _dbContext.SaveChanges();
        return entity;
    }
    public Company? GetEdgarCompanyInfo(int id)
    {
        return _dbContext.Companies
            .Where(b => b.Cik == id)
            .FirstOrDefault();
    }
    public List<Company> GetAllEdgarCompanyInfo()
    {
        return _dbContext.Companies.Include(c => c.YearlyNetIncomeLosses).ToList();
    }
}

public interface ICompanyRepository
{
    Company CreateEdgarCompanyInfo(Company entity);
    Company? GetEdgarCompanyInfo(int id);
    List<Company> GetAllEdgarCompanyInfo();
}
