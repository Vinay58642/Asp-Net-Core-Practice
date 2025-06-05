using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace xUnit_CRUDEx.Controllers
{
    [Route("[controller]")]
    public class CountriesController : Controller
    {
        private readonly ICountriesService _countriesService;
        public CountriesController(ICountriesService countriesService) 
        { 
            _countriesService= countriesService;
        }
        [Route("UploadExcel")]
        public IActionResult UploadExcel()
        {
            return View("UploadExcelToDB");
        }

        [Route("UploadExcel")]
        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile excelfile)
        {
            if (excelfile == null || excelfile.Length == 0) 
            {
                ViewBag.ErrorMessage = "Choose .xlsx file to Upload a Data";
                return View("UploadExcelToDB");
            }

            if (!Path.GetExtension(excelfile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase)) 
            {
                ViewBag.ErrorMessage = "Choose .xlsx file to Upload a Data";
                return View("UploadExcelToDB");
            }

            int EffectedRowsCount= await _countriesService.UploadDataFromExcel(excelfile);

            ViewBag.Message = EffectedRowsCount;
            return View("UploadExcelToDB");
        }
    }
}
