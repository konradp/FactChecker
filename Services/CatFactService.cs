using System.Text;

namespace FactChecker.Services
{
    public class CatFactService
    {
        private readonly HttpClient _httpClient;
        private static readonly string OutputTxtPath = Path.Combine(AppContext.BaseDirectory, "catFacts.txt");
        private static readonly string OutputCsvPath = Path.Combine(AppContext.BaseDirectory, "catFacts.csv");

        public CatFactService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CatFact> GetCatFact(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var catFact = await response.Content.ReadFromJsonAsync<CatFact>();
            return catFact ?? throw new Exception("Failed to retrieve cat fact.");
        }

        public void SaveCatFactToFile(CatFact fact)
        {
            ArgumentNullException.ThrowIfNull(fact);
            if (string.IsNullOrEmpty(fact.Fact))
                throw new ArgumentException("Fact cannot be null or empty.", nameof(fact.Fact));

            File.AppendAllText(OutputTxtPath,$"Fact: {fact.Fact} ; Length: {fact.Length} \n");
        }

        public void SaveCatFactToCSV(CatFact fact)
        {
            ArgumentNullException.ThrowIfNull(fact);
            if (string.IsNullOrEmpty(fact.Fact))
                throw new ArgumentException("Fact cannot be null or empty.", nameof(fact.Fact));

            var row = string.Join(",",
                EscapeCsv(fact.Fact),
                fact.Length.ToString()
            );
            
            if (!File.Exists(OutputCsvPath))
            {
                var header = "Fact,Length" + Environment.NewLine;
                File.WriteAllText(OutputCsvPath, header, Encoding.UTF8);
            }

            File.AppendAllTextAsync(OutputCsvPath, row + Environment.NewLine, Encoding.UTF8);
        }

        private static string EscapeCsv(string fact)
        {
            if (string.IsNullOrEmpty(fact))
                return "\"\"";

            return "\"" + fact.Replace("\"", "\"\"") + "\"";
        }
    }
}