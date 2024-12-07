using App.Data;
using Moq;

namespace App.Web.UnitTests;

public class EdgarApiServiceTests
{
    [Fact]
    public async Task ConnectToApi()
    {
        // Use real client for ease, mocking HTTP Clients is complicated and its okay to hit the real thing
        var client = new HttpClient();
        client.BaseAddress = new Uri("https://data.sec.gov/");
        client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.34.0");
        client.DefaultRequestHeaders.Add("Accept", "*/*");

        var mockRepo = new Mock<ICompanyRepository>();

        var sut = new EdgarFundingCalculatorService(client, mockRepo.Object);

        var company = await sut.GetCompanyFacts(1007587);

        Assert.NotNull(company);

    }

    [Fact]
    public async Task PopulateDataTest()
    {
        // Use real client for ease, mocking HTTP Clients is complicated and its okay to hit the real thing
        var ciks = new List<int>() { 18926 };

        var client = new HttpClient();
        client.BaseAddress = new Uri("https://data.sec.gov/");
        client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.34.0");
        client.DefaultRequestHeaders.Add("Accept", "*/*");
        var mockRepo = new Mock<ICompanyRepository>();

        var sut = new EdgarFundingCalculatorService(client, mockRepo.Object);

        await sut.PopulateCompanyData(ciks);

        mockRepo.Verify(r => r.CreateEdgarCompanyInfo(It.IsAny<Company>()), Times.Exactly(ciks.Count()));

    }
}
