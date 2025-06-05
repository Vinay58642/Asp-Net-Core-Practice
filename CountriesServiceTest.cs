using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace xUnits
{
    public class CountriesServiceTest
    {
        public readonly ICountriesService countriesService;
        public readonly IFixture fixture;

        public readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        public readonly ICountriesRepository _countriesRepository;

        public CountriesServiceTest()
        {
            //var countriesIntialData = new List<Country>() { };
            //DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
            //        new DbContextOptionsBuilder<ApplicationDbContext>().Options
            //    );

            //ApplicationDbContext dbcontext = dbContextMock.Object;
            //dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesIntialData);

            //countriesService = new CountriesService(null);

            _countriesRepositoryMock= new Mock<ICountriesRepository>();
            _countriesRepository= _countriesRepositoryMock.Object;

            countriesService = new CountriesService(_countriesRepository);

            fixture = new Fixture();
        }

        #region AddCountryTest

        [Fact]
        //when CountryAddRequest is null, it should throw ArugmentNullException
        public async Task AddCountry_IsNullArgument_ToBeNullArgument()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                //Act
                await countriesService.AddCountry(request);
            });
        }

        [Fact]
        //when CountryName is null, it should throw ArugmentException
        public async Task AddCountry_CountryIsNull_ToBeNullProperty()
        {
            //Arrange
            //CountryAddRequest? request = new CountryAddRequest() { CountryName = null };
            CountryAddRequest? request = fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, null as string).Create();

            //Country country = request.ToCountry();

            //_countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>()))
            //.ReturnsAsync(country);

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                await countriesService.AddCountry(request);
            });
        }


        [Fact]
        //when CountryName is Duplicate, it should throw ArugmentException
        public async Task AddCountry_Duplicate_ToBeDuplicate()
        {
            //Arrange
            //CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "Usa" };
            //CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "Usa" };
            
            CountryAddRequest first_country_request = fixture.Build<CountryAddRequest>()
        .With(temp => temp.CountryName, "Test name").Create();
            CountryAddRequest second_country_request = fixture.Build<CountryAddRequest>()
              .With(temp => temp.CountryName, "Test name").Create();

            Country country = first_country_request.ToCountry();

          
            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);

                _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(country);

                //Act
                await countriesService.AddCountry(first_country_request);
                              

                await countriesService.AddCountry(second_country_request);
            });
        }

        [Fact]
        //when CountryName is Non Duplicate, it should Add to list or DB
        public async Task AddCountry_NonDuplicate_ToBeNonDuplicate()
        {
            //Arrange
            CountryAddRequest? request1 = fixture.Build<CountryAddRequest>().Create();

            Country country= request1.ToCountry();  

            

            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);
                       
            //Act
            CountryResponse countryRes = await countriesService.AddCountry(request1);

            country.Countryid = countryRes.CountryID;

            List<Country> countries = new List<Country>() { country };
                                    
            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);

            List<CountryResponse> ActcountryResp = 
                await countriesService.GetAllCountries(); 

            //Assert
            Assert.True(countryRes.CountryID != Guid.Empty);
            Assert.Contains(countryRes, ActcountryResp);
        }

        #endregion

        #region GetCountries

        //When List of countries are Empty.
        [Fact]
        public async Task GetCountries_AllCountriesEmpty()
        {
            //Act
            List<CountryResponse> countryResponses = await countriesService.GetAllCountries();

            //Assert
            Assert.Empty(countryResponses);
        }
        //When List of countries are Empty.
        [Fact]
        public async Task GetCountries_AllCountries()
        {
            
            //Arrange
            //List<CountryAddRequest> List_CountryResponse = new List<CountryAddRequest>() {
            //                    new CountryAddRequest() { CountryName="USA"},
            //                    new CountryAddRequest() { CountryName="UK"}
            //};
            List<CountryAddRequest> List_CountryResponse = new List<CountryAddRequest>() {
                                fixture.Build<CountryAddRequest>().
                With(temp => temp.CountryName, "USA").Create(),
                                fixture.Build < CountryAddRequest >().With(temp => temp.CountryName, "UK").Create()
            };

            //Act
            List<CountryResponse> List_CountryAddedlist = new List<CountryResponse>();
            foreach (var current_lisrt_countries in List_CountryResponse)
            {
                List_CountryAddedlist.Add(await countriesService.AddCountry(current_lisrt_countries));
            }

            List<CountryResponse> List_ActCountryResponse = await countriesService.GetAllCountries();

            //Assert
            foreach (CountryResponse expectd_country in List_CountryAddedlist)
            {
                Assert.Contains(expectd_country, List_ActCountryResponse);
            }

        }

        #endregion

        #region GetCountrybyCountryId
        [Fact]
        public async Task GetCountrybyCountryId_IsNull()
        {
            //Arrange
            Guid? countryid = null ;

            //Act
            CountryResponse country= await countriesService.GetCountryByCountryId(countryid);

            //Assert
            Assert.Null(country);
        }
        [Fact]
        public async Task GetCountrybyCountryId_IsNotNull()
        {
            //Arrange
            CountryAddRequest countryAddRequest = fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, "India").Create();
            CountryResponse countryResponse= await countriesService.AddCountry(countryAddRequest);

            //Act
            CountryResponse country = await countriesService.GetCountryByCountryId(countryResponse.CountryID);
                            
            //Assert
            Assert.Equal(countryResponse, country);
        }

        #endregion
    }
}

       

    