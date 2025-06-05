using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Repositories
{
    public class CountriesRepository : ICountriesRepository
    {
        private readonly ApplicationDbContext _db;
        public CountriesRepository(ApplicationDbContext applicationDbContext) 
        {
            _db = applicationDbContext;
        }
        public async Task<Country> AddCountry(Country country)
        {
            await _db.Countries.AddAsync(country);

            await _db.SaveChangesAsync();

            return country;
        }

        public async Task<List<Country>> GetAllCountries()
        {
            return await _db.Countries.ToListAsync();
        }

        public async Task<Country?> GetCountryByCountryId(Guid? countryId)
        {
            return await _db.Countries.FirstOrDefaultAsync(temp => temp.Countryid == countryId);
        }
        public async Task<Country?> GetCountryByCountryName(string countryName)
        {
            return await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryName == countryName);
        }
    }
}
