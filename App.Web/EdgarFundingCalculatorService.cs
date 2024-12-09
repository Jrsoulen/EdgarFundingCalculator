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

                var edgarResponse = await GetEdgarCompanyFacts(cik);
                if (edgarResponse == null) continue; // No Edgar Data

                _companyRepo.CreateEdgarCompanyInfo(edgarResponse.MapToCore());
            }
        }

        public async Task<EdgarCompanyFactsResponse?> GetEdgarCompanyFacts(int cik)
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

        public List<CompanyResponse> GetCompanyInfo(string? firstLetter)
        {
            var companies = firstLetter != null ?
                _companyRepo.GetEdgarCompanyInfoByFirstLetter(firstLetter) :
                _companyRepo.GetAllEdgarCompanyInfo();

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
            var requiredYears = new List<int>() { 2018, 2019, 2020, 2021, 2022 };
            decimal maxRequiredYearValue = 0;
            foreach (var requiredYear in requiredYears)
            {
                var requiredYearIncome = company.YearlyNetIncome.Where(y => y.Year == requiredYear).FirstOrDefault();
                if (requiredYearIncome == null) return 0;

                // Using highest income between 2018 and 2022
                if (requiredYearIncome.Value > maxRequiredYearValue) maxRequiredYearValue = requiredYearIncome.Value;
            }

            // Company must have had positive income in both 2021 and 2022.
            var positiveYears = new List<int>() { 2021, 2022 };
            foreach (var requiredYear in positiveYears)
            {
                var yearData = company.YearlyNetIncome.Where(y => y.Year == requiredYear).First();

                if (yearData.Value <= 0)
                    return 0;
            }


            if (maxRequiredYearValue >= 10000000000)
            {
                // If income is greater than or equal to $10B, standard fundable amount is 12.33% of income
                return maxRequiredYearValue * .1233m;
            }
            // If income is less than $10B, standard fundable amount is 21.51% of income
            return maxRequiredYearValue * .2151m;
        }

        private decimal CalculateSpecialFundableAmount(Company company)
        {
            var standardFundingAmount = CalculateStandardFundableAmount(company);
            if (standardFundingAmount == 0) return 0;

            // If the company name starts with a vowel, add 15% to the standard funding amount
            var vowels = new List<char>() { 'a', 'e', 'i', 'o', 'u', 'A', 'E', 'I', 'O', 'U' };
            // Sometimes Y?
            if (new Random().NextDouble() >= 0.5) vowels.AddRange('y', 'Y');
            decimal vowelBonus = 0;
            if (vowels.Exists(c => c == company.EntityName[0])) vowelBonus = standardFundingAmount * .15m;


            // If the company’s 2022 income was less than their 2021 income, subtract 25% from their standard funding amount
            decimal incomePenalty = 0;
            if (company.YearlyNetIncome.Single(i => i.Year == 2022).Value <
                company.YearlyNetIncome.Single(i => i.Year == 2021).Value)
                incomePenalty = standardFundingAmount * .25m;

            return standardFundingAmount + vowelBonus - incomePenalty;
        }
    }
}

public interface IEdgarFundingCalculatorService
{
    Task<EdgarCompanyFactsResponse?> GetEdgarCompanyFacts(int cik);
    Task PopulateCompanyData(List<int> ciks);
    List<CompanyResponse> GetCompanyInfo(string? firstLetter);
}

public class CompanyResponse
{
    public required int id { get; set; }
    public required string name { get; set; }
    public decimal standardFundableAmount { get; set; }
    public decimal specialFundableAmount { get; set; }
}