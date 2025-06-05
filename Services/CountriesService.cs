using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using System.Diagnostics.Metrics;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        //private readonly ApplicationDbContext _db;
        private readonly ICountriesRepository _db;
        public CountriesService(ICountriesRepository countriesRepository) 
        {
            _db = countriesRepository;

        }

        #region AddCountry
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }
            else if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }
            else if (await _db.GetCountryByCountryName(countryAddRequest.CountryName)!=null)
            {
                throw new ArgumentException("CountryName Can't Be Duplicate");
            }
            else
            {
                Country? country = countryAddRequest.ToCountry();

                country.Countryid = Guid.NewGuid();

                await _db.AddCountry(country);

                return country.ToCountryResponse();
            }


        }

        #endregion

        #region GetAllCountries
        public async Task<List<CountryResponse>> GetAllCountries()
        {
            List<Country> countries = await _db.GetAllCountries();

            return countries.Select(Country => Country.ToCountryResponse()).ToList(); 
        }
        #endregion

        #region GetCountryByCountryId
        public async Task<CountryResponse> GetCountryByCountryId(Guid? countryId)
        {
            if (countryId == null) return null;

            Country? country = await _db.GetCountryByCountryId(countryId.Value);

            if (country == null) return null;

            return country.ToCountryResponse();
        }

        public async Task<int> UploadDataFromExcel(IFormFile formfile)
        {
            MemoryStream memoryStream = new MemoryStream();

            await formfile.CopyToAsync(memoryStream);

            int Effectedrows = 0;
            using (ExcelPackage excelPackage=new ExcelPackage(memoryStream))
            {
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets["Countries"];

                int row = 2;
                int rowcount = excelWorksheet.Dimension.Rows;

                for (row = 2; row <= rowcount; row++) 
                {
                    string? cellvalue = Convert.ToString(excelWorksheet.Cells[row, 1].Value);

                    if (!string.IsNullOrEmpty(cellvalue))
                    {
                        string? countryname = cellvalue;

                        if (await _db.GetCountryByCountryName(countryname)== null)
                        {
                            Country country = new Country() { CountryName = countryname };

                            await _db.AddCountry(country);
                            
                            Effectedrows++;
                        }
                    }
                }
            }
            return Effectedrows;
        }
        #endregion
    }
}
