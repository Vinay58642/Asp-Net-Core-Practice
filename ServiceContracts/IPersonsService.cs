using Entities;
using Entities.Enums;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface IPersonsService
    {
        Task<PersonResponse> ToAddPersonRequest(AddPersonRequest? addPersonRequest);

        Task<List<PersonResponse?>> GetAllPersons();

        Task<PersonResponse> GetPersonByPersonId(Guid? personId);

        Task<List<PersonResponse?>> GetFilteredPersons(string? SearchBy, string? Searchstring);

        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> Allpersons, string? SearchBy, SortingOrdersOptions? SortOrder);

        Task<PersonResponse> UpdateThePersonDate(UpdatePersonRequest? updatePersonRequest);

        Task<bool> Delete_Person(Guid? PersonId);

        Task<MemoryStream> PersonsCSV();

        Task<MemoryStream> PersonsExcel();
    }
}
