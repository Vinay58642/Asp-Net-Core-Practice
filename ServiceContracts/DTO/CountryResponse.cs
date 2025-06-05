using Entities;
using System;

namespace ServiceContracts.DTO
{
    
    public class CountryResponse
    {
        public Guid CountryID { get; set; }
        public string? CountryName { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(CountryResponse)) return false;

            CountryResponse other = (CountryResponse)obj;

            return CountryID==other.CountryID && CountryName==other.CountryName;
        }
        /// <summary>
        /// It makes keys as objects of countryResponse class as Dictionary
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }

    /// <summary>
    /// country model class convert to countryResponse object to get the data from CountryReponse Extension
    /// method. 
    /// </summary>
    public static class CountryExtensions
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse()
            { CountryID = country.Countryid, CountryName = country.CountryName };
        }
    }
}
