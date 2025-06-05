using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Entities.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using Xunit.Abstractions;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using AutoFixture;
using FluentAssertions;
using RepositoryContracts;
using Moq;
using System.Linq.Expressions;

namespace xUnits
{
    public class PersonsServiceTest
    {
        public readonly IPersonsService? _personsService;
        public readonly ICountriesService _countriesService;

        public readonly Mock<IPersonsRepository> _personsRepositoryMock;
        public readonly IPersonsRepository personsRepository;

        public readonly ITestOutputHelper _outputHelper;
        public readonly IFixture _fixture;
        public PersonsServiceTest(ITestOutputHelper testOutputHelper) 
        {
            //_personsService = new PersonsService(false);
            //_countriesService = new CountriesService(false);
            //_outputHelper = testOutputHelper;

            //Mocking of A DB context..
            //var personslistIntial = new List<Person>() { };
            //var countrieslistIntial = new List<Country>() { };

            //DbContextMock<ApplicationDbContext> dbContextMock= new DbContextMock<ApplicationDbContext>(
            //    new DbContextOptionsBuilder<ApplicationDbContext>().Options
            //    );

            //ApplicationDbContext dbContext = dbContextMock.Object;
            //dbContextMock.CreateDbSetMock(temp => temp.Persons, personslistIntial);
            //dbContextMock.CreateDbSetMock(temp => temp.Countries, countrieslistIntial);

            //_countriesService=new CountriesService(dbContext);
            //_personsService = new PersonsService(dbContext, _countriesService);

            //Mocking of A Repository..
            _personsRepositoryMock = new Mock<IPersonsRepository> ();
            personsRepository = _personsRepositoryMock.Object;

            //_countriesService = new CountriesService(null);
            _personsService = new PersonsService(personsRepository);

            _outputHelper = testOutputHelper;
            _fixture = new Fixture();
        }

        [Fact]
        public async Task ToAddPerson_IsNull_ToBeNUll()
        {
            //Arrange
            AddPersonRequest? addPersonRequest = null;

            //Assert
            //await Assert.ThrowsAsync<ArgumentNullException>(async () => { 
            //    //Act
            //    await _personsService?.ToAddPersonRequest(addPersonRequest);
            //});

            Func<Task> action=(async () => {
                //Act
                await _personsService?.ToAddPersonRequest(addPersonRequest);
            });

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task ToAddPerson_IsNullOfSingleProperty_ToBeNUllProperty()
        {
            //Arrange
            AddPersonRequest? addPersonRequest = new AddPersonRequest() { PersonName=null};

            Person person=addPersonRequest.ToAddPerson();

            _personsRepositoryMock.Setup(temp=>temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Assert
            Func<Task> action= (async () => {
                //Act
                await _personsService?.ToAddPersonRequest(addPersonRequest);
            });

            await action.Should().ThrowAsync<ArgumentException>();

        }
        [Fact]
        public async Task ToAddPerson_IsNotNull_ToBeSuccesfull()
        {
            //Arrange
            AddPersonRequest? addPersonRequest = new AddPersonRequest() { PersonName = "Chinna",
                Address = "India",
                Email = "Chinna@Gmail.com",
                CountryId = Guid.NewGuid(),
                DateofBirth = Convert.ToDateTime("04/Aug/1997"),
                Gender = GenderEnum.Male,
                ReceivNewsLetters = true
            };

            Person person = addPersonRequest.ToAddPerson();

            PersonResponse Expect_personResponse = person.ToPersonResponse();

            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Act
            PersonResponse? Act_personResponse= await _personsService?.ToAddPersonRequest(addPersonRequest);

            Expect_personResponse.PersonId=Act_personResponse.PersonId;

            //Assert
            //Assert.True(personResponse?.PersonId != null);

            Act_personResponse.PersonId.Should().NotBe(Guid.Empty);

            //Assert.Contains(personResponse, ActPersonResponse);

            Act_personResponse.Should().Be(Expect_personResponse);
        }
        [Fact]
        public async Task GetPersonByPersonId_IsNull_ToBeNull()
        {
            //Arrange
            Guid? PersonId = null;

           //Act
           PersonResponse? personResponse= await _personsService?.GetPersonByPersonId(PersonId);

           //Assert
           //Assert.Null(personResponse);

            personResponse.Should().BeNull();
        }

        [Fact]
        public async Task GetPersonByPersonId_IsNotNull_ToBeSuccesfull()
        {

            //Arrange
            Person person = _fixture.Build<Person>()
                           .With(temp => temp.Email, "email@sample.com")
                           .With(temp => temp.country, null as Country)
                           .Create();


            
            PersonResponse? Expect_personResponse=person.ToPersonResponse();

            _personsRepositoryMock.Setup(temp=>temp.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(person);

            //Act
            PersonResponse? Act_personResponse = await _personsService?.GetPersonByPersonId(Expect_personResponse?.PersonId);

            //Assert
           //Assert.Equal(personResponse, Act_personResponse);

            Act_personResponse.Should().BeEquivalentTo(Expect_personResponse);
        }

        [Fact]
        public async Task GetAllPersons_IsEmpty_ToBeEmpty()
        {
            List<Person> persons = new List<Person>();  

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            //Act
            List<PersonResponse?> personResponse= await _personsService?.GetAllPersons();

            //Assert
            //Assert.Empty(personResponse);

            personResponse.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllPersons_IsNotEmpty_ToBeSuccesfull()
        {
            //Arrange
            List<Person> persons = new List<Person>() {
                        _fixture.Build<Person>()
                        .With(temp => temp.Email, "someone_1@example.com")
                        .With(temp => temp.country, null as Country)
                        .Create(),

                        _fixture.Build<Person>()
                        .With(temp => temp.Email, "someone_2@example.com")
                        .With(temp => temp.country, null as Country)
                        .Create(),

                        _fixture.Build<Person>()
                        .With(temp => temp.Email, "someone_3@example.com")
                        .With(temp => temp.country, null as Country)
                        .Create()
                       };

            List<PersonResponse?> Expe_personResponses =persons.Select(temp=>temp.ToPersonResponse()).ToList();

            //Act
           
            _outputHelper.WriteLine("Expected Result:");
            foreach (PersonResponse? response in Expe_personResponses)
            {
                _outputHelper.WriteLine(response?.ToString());
            }

            _personsRepositoryMock.Setup(temp=>temp.GetAllPersons()).ReturnsAsync(persons);

            List<PersonResponse?> Act_personResponse = await _personsService?.GetAllPersons();

            _outputHelper.WriteLine("Actual Result:");
            foreach (PersonResponse? Act_response in Act_personResponse)
            {
                _outputHelper.WriteLine(Act_response?.ToString());
            }
            //Assert
            //foreach (var personResponse in personResponses)
            //{
            //    Assert.Contains(personResponse, Act_personResponse);
            //}

            Act_personResponse.Should().BeEquivalentTo(Expe_personResponses);
        }

        [Fact]
        public async Task GetFilteredPersons_ToBeSuccessfull()
        {
            //Arrange
            List<Person> persons = new List<Person>() {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.country, null as Country)
                .Create()
               };

           
            List<PersonResponse?> Expe_personResponses = persons.Select(temp => temp.ToPersonResponse()).ToList();


            //Act

            _outputHelper.WriteLine("Expected Result:");
            foreach (PersonResponse? response in Expe_personResponses)
            {
                _outputHelper.WriteLine(response?.ToString());
            }

            //_personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            List<PersonResponse?> Act_personResponse_get = await _personsService?.GetFilteredPersons(nameof(Person.PersonName),"");

            _outputHelper.WriteLine("Actual Result:");
            foreach (PersonResponse? Act_response in Act_personResponse_get)
            {
                _outputHelper.WriteLine(Act_response?.ToString());
            }
            //Assert
            //foreach (var personResponse in personResponses)
            //{
            //    Assert.Contains(personResponse, Act_personResponse_get);
            //}

            Act_personResponse_get.Should().BeEquivalentTo(Expe_personResponses);
        }

        [Fact]
        public async Task GetFilteredPersonsByPersonSeach()
        {

            //Arrange
            List<Person> persons = new List<Person>() {
                _fixture.Build<Person>()
                .With(temp => temp.PersonName, "Nani")
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.PersonName, "Nagesh")
                .With(temp => temp.country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.PersonName, "Suresh")
                .With(temp => temp.country, null as Country)
                .Create()
               };


            List<PersonResponse?> Expe_personResponses = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _outputHelper.WriteLine("Expected Result:");
            foreach (PersonResponse? response in Expe_personResponses)
            {
                _outputHelper.WriteLine(response?.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            List<PersonResponse?> Act_personResponse_get = await _personsService?.GetFilteredPersons(nameof(Person.PersonName), "na");

            _outputHelper.WriteLine("Actual Result:");
            foreach (PersonResponse? Act_response in Act_personResponse_get)
            {
                _outputHelper.WriteLine(Act_response?.ToString());
            }
            //Assert
            //foreach (var personResponse in personResponses)
            //{
            //    if (personResponse?.PersonName != null) 
            //    { 
            //        if (personResponse.PersonName.Contains("na", StringComparison.OrdinalIgnoreCase))
            //        {
            //            Assert.Contains(personResponse, Act_personResponse_get);
            //        }
            //    }

            //}

            Act_personResponse_get.Should().BeEquivalentTo(Expe_personResponses);
        }


        [Fact]
        public async Task GetSortedPersons()
        {
            //Arrange
            List<Person> persons = new List<Person>() {
                    _fixture.Build<Person>()
                    .With(temp => temp.Email, "someone_1@example.com")
                    .With(temp => temp.country, null as Country)
                    .Create(),

                    _fixture.Build<Person>()
                    .With(temp => temp.Email, "someone_2@example.com")
                    .With(temp => temp.country, null as Country)
                    .Create(),

                    _fixture.Build<Person>()
                    .With(temp => temp.Email, "someone_3@example.com")
                    .With(temp => temp.country, null as Country)
                    .Create()
                   };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons())
             .ReturnsAsync(persons);

            _outputHelper.WriteLine("Expected Result:");
            foreach (PersonResponse? response in person_response_list_expected)
            {
                _outputHelper.WriteLine(response?.ToString());
            }

            List<PersonResponse> Allpersons = await _personsService.GetAllPersons();

            List<PersonResponse?> Act_personResponse_get = 
                await _personsService?.GetSortedPersons(Allpersons, nameof(Person.PersonName), SortingOrdersOptions.DESC);

            _outputHelper.WriteLine("Actual Result:");
            foreach (PersonResponse? Act_response in Act_personResponse_get)
            {
                _outputHelper.WriteLine(Act_response?.ToString());
            }

            //personResponses =personResponses.OrderByDescending(temp=>temp.PersonName).ToList();

            ////Assert
            //for (int i= 0; i < personResponses.Count ; i++){
            //    Assert.Equal(personResponses[i], Act_personResponse_get[i]);
            //}

            //Act_personResponse_get.Should().BeEquivalentTo(personResponses);

            Act_personResponse_get.Should().BeInDescendingOrder(temp => temp.PersonName);
        }

        [Fact]
        public async Task UpdatePerson_IfNullObject()
        {
            //Arrange
            UpdatePersonRequest? updatePersonRequest = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => {
                //Act
                await _personsService.UpdateThePersonDate(updatePersonRequest);
            });
            
        }

        [Fact]
        public async Task UpdatePerson_IfNullProperty()
        {
            //Arrange
            UpdatePersonRequest? updatePersonRequest = new UpdatePersonRequest { PersonId=Guid.NewGuid() };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async() => {
                //Act
                await _personsService.UpdateThePersonDate(updatePersonRequest);
            });

        }

        [Fact]
        public async Task UpdatePerson_IfNullPropertyofpersonNm_ToBeArgumentException()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
             .With(temp => temp.PersonName, null as string)
             .With(temp => temp.Email, "someone@example.com")
             .With(temp => temp.country, null as Country)
             .With(temp => temp.Gender, "Male")
             .Create();

            PersonResponse person_response_from_add = person.ToPersonResponse();

            UpdatePersonRequest person_update_request = person_response_from_add.ToUpdatePersonRequest();


            //Act
            var action = async () =>
            {
                await _personsService.UpdateThePersonDate(person_update_request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }


        [Fact]
        public async Task UpdatePerson_IfNotNullPropertyofpersonNm()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
             .With(temp => temp.Email, "someone@example.com")
             .With(temp => temp.country, null as Country)
             .With(temp => temp.Gender, "Male")
             .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            UpdatePersonRequest person_update_request = person_response_expected.ToUpdatePersonRequest();

            _personsRepositoryMock
             .Setup(temp => temp.UpdateThePersonData(It.IsAny<Person>()))
             .ReturnsAsync(person);

            _personsRepositoryMock
             .Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
             .ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_update = await _personsService.UpdateThePersonDate(person_update_request);

            //Assert
            person_response_from_update.Should().Be(person_response_expected);
        }

        [Fact]
        public async Task Delete_IfNull()
        {
            //Arrange
            Guid? PersonId = null;
                        
            //Act
            bool isvalid= await _personsService.Delete_Person(PersonId);

            //Assert
            //Assert.False(isvalid);

            isvalid.Should().BeFalse();
        }

        [Fact]
        public async Task Delete_IfNotNull()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
             .With(temp => temp.Email, "someone@example.com")
             .With(temp => temp.country, null as Country)
             .With(temp => temp.Gender, "Female")
             .Create();

            PersonResponse personResponse = person.ToPersonResponse();

            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(person);

            _personsRepositoryMock.Setup(temp => temp.Delete_PersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(true);

            //Act
            bool isvalid = await _personsService.Delete_Person(personResponse.PersonId);

            //Assert
            //Assert.True(isvalid);

            isvalid.Should().BeTrue();
        }
    }
}
