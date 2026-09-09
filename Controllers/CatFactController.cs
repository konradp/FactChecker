using FactChecker.Services;
using Microsoft.AspNetCore.Mvc;

namespace FactChecker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CatFactController : ControllerBase
    {
        private readonly ICatFactService _catFactService;

        public CatFactController(ICatFactService catFactService)
        {
            _catFactService = catFactService;
        }

        [HttpGet("GetCatFact", Name = "GetCatFact")]
        public async Task<CatFact> Get()
        {
            var catFact = await _catFactService.GetCatFact("https://catfact.ninja/fact");
            _catFactService.SaveCatFactToFile(catFact);
            _catFactService.SaveCatFactToCSV(catFact);
            return catFact;
        }
    }
}