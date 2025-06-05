using Moq;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using ServiceContracts;
using Entities;
using ServiceContracts.DTO;
using Entities.Enums;
using xUnit_CRUDEx.Controllers;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;

namespace xUnits
{
    public class PersonsControllerTest
    {
        private readonly Mock<IPersonsService> _personsServiceMock;
        private readonly IPersonsService _personsService;

        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly ICountriesService _countriesService;

        private readonly IFixture _fixture;

        public PersonsControllerTest()
        { 
            _fixture = new Fixture();

            _personsServiceMock = new Mock<IPersonsService>();
            _countriesServiceMock = new Mock<ICountriesService>();

            _personsService=_personsServiceMock.Object;
            _countriesService= _countriesServiceMock.Object;
        }

        [Fact]
        public async void Index_ToBeReturnPersonResposeofIndexView()
        {
            List<PersonResponse> persons = _fixture.Create<List<PersonResponse>>();
                      

            _personsServiceMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(persons);

            _personsServiceMock.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortingOrdersOptions>())).ReturnsAsync(persons);

            PersonsController personsController = new PersonsController(_personsService, _countriesService);

            IActionResult result= await personsController.Index(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<SortingOrdersOptions>());

            ViewResult viewResult= Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();

            viewResult.ViewData.Model.Should().Be(persons);
        }
    }
}
