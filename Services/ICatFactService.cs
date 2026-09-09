namespace FactChecker.Services
{
    public interface ICatFactService
    {
        Task<CatFact> GetCatFact(string url);
        void SaveCatFactToFile(CatFact fact);
        void SaveCatFactToCSV(CatFact fact);
    }
}