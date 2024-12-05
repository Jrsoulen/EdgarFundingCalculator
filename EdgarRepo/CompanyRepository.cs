namespace App.Data;

public class CompanyRepository : ICompanyRepository
{
    private EdgarContext _dbContext;
    public CompanyRepository(EdgarContext context)
    {
        _dbContext = context;
    }
    public EdgarCompanyInfo CreateEdgarCompanyInfo(EdgarCompanyInfo entity)
    {
        _dbContext.Add(entity);
        _dbContext.SaveChanges();
        return entity;
    }
    public EdgarCompanyInfo GetEdgarCompanyInfo(int id)
    {
        var company = _dbContext.EdgarCompanies
            .Where(b => b.Cik == id)
            .FirstOrDefault();

        if (company is null) throw new ArgumentException($"Company with Cik {id} does not exist");

        return company;
    }
    public EdgarCompanyInfo UpdateEdgarCompanyInfo(EdgarCompanyInfo entity)
    {
        var company = _dbContext.EdgarCompanies
            .Where(b => b.Cik == entity.Cik)
            .FirstOrDefault();

        if (company is null) throw new ArgumentException($"Company with Cik {entity.Cik} does not exist");

        company.EntityName = "new test company name";

        _dbContext.SaveChanges();

        return company;
    }
    public void DeleteCompany(int id)
    {
        var company = _dbContext.EdgarCompanies
            .Where(b => b.Cik == id)
            .FirstOrDefault();

        if (company is null) throw new ArgumentException($"Company with Cik {id} does not exist");

        _dbContext.Remove(company);
        _dbContext.SaveChanges();
    }
}

public interface ICompanyRepository
{
    EdgarCompanyInfo CreateEdgarCompanyInfo(EdgarCompanyInfo entity);
    EdgarCompanyInfo GetEdgarCompanyInfo(int id);
    EdgarCompanyInfo UpdateEdgarCompanyInfo(EdgarCompanyInfo entity);
    void DeleteCompany(int id);
}
