using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using static EdgarCompanyFactsResponse;

public static class ResponseExtensions
{
    public static Company MapToCore(this EdgarCompanyFactsResponse response)
    {
        var usdArray = response.Facts.UsGaap.NetIncomeLoss.Units.Usd;
        var filteredUsd = usdArray == null ? new List<InfoFactUsGaapIncomeLossUnitsUsd>() :
            usdArray.Where(u => u.Form == "10-K" && u.Frame != null && u.Frame.StartsWith("CY") && u.Frame.Length == 6).ToList();

        return new Company()
        {
            Cik = response.Cik,
            EntityName = response.EntityName,
            YearlyNetIncomeLosses = new Collection<YearlyNetIncomeLoss>(filteredUsd.Select(u =>
                new YearlyNetIncomeLoss()
                {
                    Frame = u.Frame!,
                    Value = u.Val,
                    Year = int.Parse(u.Frame!.Substring(2, 4))
                }
            ).ToArray())
        };
    }
}

public class EdgarCompanyFactsResponse
{
    public int Cik { get; set; }
    public required string EntityName { get; set; }
    public required InfoFact Facts { get; set; }
    public class InfoFact
    {
        [JsonPropertyName("us-gaap")]
        public required InfoFactUsGaap UsGaap { get; set; }
    }

    public class InfoFactUsGaap
    {
        public required InfoFactUsGaapNetIncomeLoss NetIncomeLoss { get; set; }
    }

    public class InfoFactUsGaapNetIncomeLoss
    {
        public required InfoFactUsGaapIncomeLossUnits Units { get; set; }
    }

    public class InfoFactUsGaapIncomeLossUnits
    {
        public InfoFactUsGaapIncomeLossUnitsUsd[] Usd { get; set; }
    }

    public class InfoFactUsGaapIncomeLossUnitsUsd
    {
        /// <summary>
        /// Possibilities include 10-Q, 10-K,8-K, 20-F, 40-F, 6-K, and their variants.
        /// YOU ARE INTERESTED ONLY IN 10-K DATA!
        /// </summary>
        public required string Form { get; set; }
        /// <summary>
        /// For yearly information, the format is CY followed by the year number.
        /// For example: CY2021.YOU ARE INTERESTED ONLY IN YEARLY INFORMATION WHICH FOLLOWS THIS FORMAT!
        /// </summary>
        public string? Frame { get; set; }
        /// <summary>
        /// The income/loss amount.
        /// </summary>
        public decimal Val { get; set; }
    }
}