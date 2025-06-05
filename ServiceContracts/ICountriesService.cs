using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface ICountriesService
    {
        /// <summary>
        /// Add country data through this AddCountry method with using DTO.
        /// </summary>
        /// <param name="countryAddRequest"></param>
        /// <returns></returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Get list of countries into List<CountryResponse> GetAllCountries() method.
        /// </summary>
        /// <returns></returns>
        Task<List<CountryResponse>> GetAllCountries();

        /// <summary>
        /// Get country object by passing countryid.
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns>Returns CountryResponse Object according to the countryId</returns>
        Task<CountryResponse> GetCountryByCountryId(Guid? countryId);

        Task<int> UploadDataFromExcel(IFormFile formfile);
    }
}
