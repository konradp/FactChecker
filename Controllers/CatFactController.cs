using FactChecker.Services;
using Microsoft.AspNetCore.Mvc;

namespace FactChecker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CatFactController : ControllerBase
    {
        private readonly CatFactService _catFactService;

        public CatFactController(CatFactService catFactService)
        {
            _catFactService = catFactService;
        }

        [HttpGet(Name = "GetCatFact")]
        public async Task<CatFact> Get()
        {
            var catFact = await _catFactService.GetCatFact("https://catfact.ninja/fact");
            _catFactService.SaveCatFactToFile(catFact);
            return catFact;
        }
    }
}