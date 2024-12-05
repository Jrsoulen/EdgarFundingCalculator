using App.Data;

namespace App.Web
{
    public interface IEdgarFundingCalculatorService
    {
        Task<EdgarCompanyFactsResponse> GetCompanyFacts(int cik);
        Task PopulateCompanyData(List<int> ciks);
    }

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

                _companyRepo.CreateEdgarCompanyInfo(edgarResponse.MapToCore());
            }
        }

        public async Task<EdgarCompanyFactsResponse> GetCompanyFacts(int cik)
        {
            // API wants leading zeros on 10 digit CIK
            var cikString = cik.ToString();
            while (cikString.Length < 10) cikString = "0" + cikString;

            var companyFacts = await _httpClient.GetFromJsonAsync<EdgarCompanyFactsResponse>($"api/xbrl/companyfacts/CIK{cikString}.json");
            if (companyFacts == null)
                throw new ArgumentException($"CIK ${cik} does not exist.");

            return companyFacts;
        }
    }

}
