using Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace xUnit_CRUDEx.Controllers
{
    //Route Attribute can write like [Route("[controller]")] instead of write [Route("persons")], this is Route token.
    [Route("[controller]")]
    public class PersonsController : Controller
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        public PersonsController(IPersonsService personsService, ICountriesService countriesService)
        {
            _personsService = personsService;
            _countriesService = countriesService;
        }

        //Url: "persons/Index" if You mention like [Route("Index")] instead of [Route("persons/Index")]
        //Route Attribute can write like [Route("[action]")] instead of write [Route("Index")], this is Route token.
        [Route("[action]")]
        [Route("/")]
        public async Task<IActionResult> Index(string? SearchBy, string? searchString,
            string? SortedBy = nameof(PersonResponse.PersonName), SortingOrdersOptions? Sortingstring = SortingOrdersOptions.ASC)
        {
            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateofBirth), "Date Of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryId), "CountryId" },
                { nameof(PersonResponse.Address), "Address" }
            };

            List<PersonResponse> personResponses = await _personsService.GetFilteredPersons(SearchBy, searchString);

            ViewBag.SearchBy = SearchBy;
            ViewBag.SearchString = searchString;

            List<PersonResponse> Sorted_person = await _personsService.GetSortedPersons(personResponses, SortedBy, Sortingstring);
            ViewBag.CurrentSortedBy = SortedBy;
            ViewBag.CurrentSortString = Sortingstring.ToString();

            return View(Sorted_person);
        }

        //Url: "persons/Create" if You mention like [Route("Create")] instead of [Route("persons/Create")]
        //Route Attribute can write like [Route("[action]")] instead of write [Route("Create")], this is Route token.
        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countryResponse = await _countriesService.GetAllCountries();

            ViewBag.Countries = countryResponse.Select(temp => new SelectListItem() { Text=temp.CountryName, Value=temp.CountryID.ToString() });

            return View();
        }

        //Url: "persons/Create" if You mention like [Route("Create")] instead of [Route("persons/Create")]
        //[Route("Create")]
        //Route Attribute can write like [Route("[action]")] instead of write [Route("Create")], this is Route token.
        [Route("[action]")]
        [HttpPost] 
        public async Task<IActionResult> Create(AddPersonRequest? addPersonRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countryResponse = await _countriesService.GetAllCountries();

                ViewBag.Countries = countryResponse.Select(temp => new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

                ViewBag.Errors= ModelState.Values.SelectMany(v=>v.Errors).Select(e=>e.ErrorMessage).ToList();

                return View();
            }

            await _personsService.ToAddPersonRequest(addPersonRequest);
            
            return RedirectToAction("Index","Persons");
        }

        [Route("[action]/{PersonId}")]
        [HttpGet]
        public async  Task<IActionResult> Edit(Guid PersonId)
        {
            PersonResponse personResponse= await _personsService.GetPersonByPersonId(PersonId);

            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            UpdatePersonRequest updatePersonRequest = personResponse.ToUpdatePersonRequest();

            List<CountryResponse> countryResponse = await _countriesService.GetAllCountries();

            ViewBag.Countries = countryResponse.Select(temp => new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

            return View(updatePersonRequest);
        }

        [Route("[action]/{PersonId}")]
        [HttpPost]
        public async Task<IActionResult> Edit(UpdatePersonRequest? updatePersonRequest)
        {
            PersonResponse personResponse= await _personsService.GetPersonByPersonId(updatePersonRequest.PersonId);

            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countryResponse = await _countriesService.GetAllCountries();

                ViewBag.Countries = countryResponse.Select(temp => new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                return View(personResponse.ToUpdatePersonRequest());
            }
            else
            {
                PersonResponse personResponse_act = await _personsService.UpdateThePersonDate(updatePersonRequest);

                return RedirectToAction("Index", "Persons");
            }
        }

        [Route("[action]/{PersonId}")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid PersonId)
        {
            PersonResponse personResponse = await _personsService.GetPersonByPersonId(PersonId);

            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }
            
            return View(personResponse);
        }

        [Route("[action]/{PersonId}")]
        [HttpPost]
        public async Task<IActionResult> Delete(UpdatePersonRequest? updatePersonRequest)
        {
            PersonResponse personResponse = await _personsService.GetPersonByPersonId(updatePersonRequest.PersonId);

            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            await _personsService.Delete_Person(personResponse.PersonId);

            return RedirectToAction("Index", "Persons");
            
        }

        [Route("GenaratePDF")]
        public async Task<IActionResult> GenaratePDF()
        {
            List<PersonResponse?> personResponses = await _personsService.GetAllPersons();

            return new ViewAsPdf("GenaratePDF", personResponses, ViewData)
            {
                PageMargins=new Rotativa.AspNetCore.Options.Margins()
                {
                    Top=20, Bottom=20, Left=20, Right=20
                },
                PageOrientation=Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }

        [Route("PersonsCSV")]
        public async Task<IActionResult> PersonsCSV()
        {
           MemoryStream memoryStream=await  _personsService.PersonsCSV();

            return File(memoryStream, "application/octet-stream", "Persons.csv");
        }
        

        [Route("PersonsExcel")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream memoryStream = await _personsService.PersonsExcel();

            return File(memoryStream, "application/vnd.ms-excel", "Persons.xlsx");
        }
    }
}
