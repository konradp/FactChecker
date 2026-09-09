namespace FactChecker.Services
{
    public class CatFactService
    {
        private readonly HttpClient _httpClient;
        private static readonly string OutputTxtPath = Path.Combine(AppContext.BaseDirectory, "catFacts.txt");

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
            File.AppendAllText(OutputTxtPath,$"Fact: {fact.Fact} ; Length: {fact.Length} \n");
        }
    }
}