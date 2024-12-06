using App.Data;


namespace App.Web
{

    /// <summary>
    /// All Business Logic for application is here
    /// </summary>
    public class EdgarFundingCalculatorService : IEdgarFundingCalculatorService
    {
        private readonly HttpClient _httpClient;
        private readonly ICompanyRepository _companyRepo;
        public EdgarFundingCalculatorService(HttpClient httpClient, ICompanyRepository companyRepo)
        {
            _httpClient = httpClient;
            _companyRepo = companyRepo;
        }

        public async Task PopulateCompanyData(List<int> ciks)
        {
            foreach (var cik in ciks)
            {
                var foraData = _companyRepo.GetEdgarCompanyInfo(cik);
                if (foraData != null) continue; // Already populated, do we care?

                var edgarResponse = await GetCompanyFacts(cik);
                if (edgarResponse == null) continue; // No Edgar Data

                _companyRepo.CreateEdgarCompanyInfo(edgarResponse.MapToCore());
            }
        }

        public async Task<EdgarCompanyFactsResponse?> GetCompanyFacts(int cik)
        {
            // API wants leading zeros on 10 digit CIK
            var cikString = cik.ToString();
            while (cikString.Length < 10) cikString = "0" + cikString;

            try
            {
                var companyFacts = await _httpClient.GetFromJsonAsync<EdgarCompanyFactsResponse>($"api/xbrl/companyfacts/CIK{cikString}.json");
                return companyFacts;
            }
            catch
            {
                Console.WriteLine($"CIK ${cik} does not exist.");
                return null;
            }
        }

        public List<CompanyResponse> GetAllCompanyInfo()
        {
            var companies = _companyRepo.GetAllEdgarCompanyInfo();

            return companies.Select(company => new CompanyResponse()
            {
                id = company.Cik,
                name = company.EntityName,
                standardFundableAmount = CalculateStandardFundableAmount(company),
                specialFundableAmount = CalculateSpecialFundableAmount(company),
            }).ToList();
        }

        private decimal CalculateStandardFundableAmount(Company company)
        {
            // Company must have income data for all years between (and including) 2018 and 2022.
            var requiredYears = new int[2018, 2019, 2020, 2021, 2022];
            foreach (var requiredYear in requiredYears)
            {
                if (!company.YearlyNetIncomeLosses.Where(y => y.Year == requiredYear).Any()) return 0;
            }

            // Company must have had positive income in both 2021 and 2022.
            var positiveYears = new int[2021, 2022];
            foreach (var requiredYear in positiveYears)
            {
                var yearData = company.YearlyNetIncomeLosses.Where(y => y.Year == requiredYear).First();
                // Less than? Because these are income losses?
                if (0 < yearData.Value)
                    return 0;
            }

            return 1;
        }

        private decimal CalculateSpecialFundableAmount(Company company)
        {
            return 0;
        }
    }
}

public interface IEdgarFundingCalculatorService
{
    Task<EdgarCompanyFactsResponse> GetCompanyFacts(int cik);
    Task PopulateCompanyData(List<int> ciks);
    List<CompanyResponse> GetAllCompanyInfo();
}

public class CompanyResponse
{
    public required int id { get; set; }
    public required string name { get; set; }
    public decimal standardFundableAmount { get; set; }
    public decimal specialFundableAmount { get; set; }
}