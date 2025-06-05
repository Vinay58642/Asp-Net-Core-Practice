using CsvHelper;
using Entities;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using CsvHelper.Configuration;
using OfficeOpenXml;
using RepositoryContracts;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        public readonly IPersonsRepository _db;

        public PersonsService(IPersonsRepository personsRepository) 
        { 
            _db = personsRepository;
            
        }

        public async Task<bool> Delete_Person(Guid? PersonId)
        {
            if (PersonId == null) return false;

            Person? person= await _db.GetPersonByPersonId(PersonId.Value);

            if (person == null) return false;

            await _db.Delete_PersonByPersonId(PersonId.Value);

            return true;
        }
        //private PersonResponse ConvertPersonToPersonResponse(Person person)
        //{

        //    PersonResponse personResponse = person.ToPersonResponse();

        //    personResponse.Country = person.country?.CountryName;

        //    return personResponse;
        //}

        public async Task<List<PersonResponse?>> GetAllPersons()
        {
            //return _db.Persons.ToList().
            //    Select(temp => ConvertPersonToPersonResponse(temp)).ToList();

            var persons= await _db.GetAllPersons();

            return persons.
                Select(temp => temp.ToPersonResponse()).ToList();

            //return _db.Get_AllPersons().
            //    Select(temp=> ConvertPersonToPersonResponse(temp)).ToList();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string SearchBy, string? Searchstring)
        {
            List<Person> persons = SearchBy switch
            {
                nameof(PersonResponse.PersonName) =>
                 await _db.GetFilteredPersons(temp =>
                 temp.PersonName.Contains(Searchstring)),

                nameof(PersonResponse.Email) =>
                 await _db.GetFilteredPersons(temp =>
                 temp.Email.Contains(Searchstring)),

                nameof(PersonResponse.DateofBirth) =>
                 await _db.GetFilteredPersons(temp =>
                 temp.DateofBirth.Value.ToString("dd MMMM yyyy").Contains(Searchstring)),


                nameof(PersonResponse.Gender) =>
                 await _db.GetFilteredPersons(temp =>
                 temp.Gender.Contains(Searchstring)),

                nameof(PersonResponse.CountryId) =>
                 await _db.GetFilteredPersons(temp =>
                 temp.country.CountryName.Contains(Searchstring)),

                nameof(PersonResponse.Address) =>
                await _db.GetFilteredPersons(temp =>
                temp.Address.Contains(Searchstring)),

                _ => await _db.GetAllPersons()
            };
            return persons.Select(temp => temp.ToPersonResponse()).ToList();
        }

        public async Task<PersonResponse?> GetPersonByPersonId(Guid? personId)
        {
            if (personId == null) { return null; }

            Person? person= await _db.GetPersonByPersonId(personId.Value);

            if (person==null) { return null; } 

            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> Allpersons, string? SearchBy, SortingOrdersOptions? SortOrder)
        {
            if (SearchBy == null)
            {
                return Allpersons;
            }

            List<PersonResponse> SwitchResponse = (SearchBy, SortOrder)
                switch
            {
                (nameof(Person.PersonName), SortingOrdersOptions.ASC)
                => Allpersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(Person.PersonName), SortingOrdersOptions.DESC)
                => Allpersons.OrderByDescending(temp=>temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(Person.Email), SortingOrdersOptions.ASC)
                => Allpersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(Person.Email), SortingOrdersOptions.DESC)
                => Allpersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                _ => Allpersons
            };
            return SwitchResponse;
        }

        public async Task<MemoryStream> PersonsCSV()
        {
            MemoryStream memoryStream=new MemoryStream();
            StreamWriter streamWriter=new StreamWriter(memoryStream);

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            //CsvWriter csvWriter=new CsvWriter(streamWriter, CultureInfo.InvariantCulture, leaveOpen: true);

            CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration);

            //csvWriter.WriteHeader<PersonResponse>();

            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.DateofBirth));
            csvWriter.WriteField(nameof(PersonResponse.Address));
            csvWriter.WriteField(nameof(PersonResponse.Country));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.ReceivNewsLetters));

            csvWriter.NextRecord();

            List<PersonResponse> persons = await GetAllPersons();
             
            //await csvWriter.WriteRecordsAsync(persons);
            foreach(PersonResponse person in persons)
            {
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Email);
                if (person.DateofBirth.HasValue)
                {
                    csvWriter.WriteField(person.DateofBirth.Value.ToString("dd-MMM-yyyy"));
                }
                else { csvWriter.WriteField(""); }
                csvWriter.WriteField(person.Address);
                csvWriter.WriteField(person.Country);
                csvWriter.WriteField(person.Age);
                csvWriter.WriteField(person.ReceivNewsLetters);
                csvWriter.NextRecord();
                csvWriter.Flush();
            }
            
            memoryStream.Position = 0;
            return memoryStream;
        }

        public async Task<MemoryStream> PersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();

            using (ExcelPackage excelPackage= new ExcelPackage(memoryStream))
            {
               ExcelWorksheet excelWorksheet=excelPackage.Workbook.Worksheets.Add("PersonsSheet");

                excelWorksheet.Cells["A1"].Value = nameof(PersonResponse.PersonName);
                excelWorksheet.Cells["B1"].Value = nameof(PersonResponse.Email);
                excelWorksheet.Cells["C1"].Value = nameof(PersonResponse.DateofBirth);
                excelWorksheet.Cells["D1"].Value = nameof(PersonResponse.Age);
                excelWorksheet.Cells["E1"].Value = nameof(PersonResponse.Address);
                excelWorksheet.Cells["F1"].Value = nameof(PersonResponse.Gender);
                excelWorksheet.Cells["G1"].Value = nameof(PersonResponse.Country);
                excelWorksheet.Cells["H1"].Value = nameof(PersonResponse.ReceivNewsLetters);

                int row = 2;

                List<PersonResponse> persons=await GetAllPersons();

                foreach(PersonResponse person in persons)
                {
                    excelWorksheet.Cells[row, 1].Value = person.PersonName;
                    excelWorksheet.Cells[row, 2].Value = person.Email;
                    if (person.DateofBirth.HasValue) {
                        excelWorksheet.Cells[row, 3].Value = person.DateofBirth.Value.ToString("dd-MM-yyyy");
                    }
                    else
                    {
                        excelWorksheet.Cells[row, 3].Value = "";
                    }
                    excelWorksheet.Cells[row, 4].Value = person.Age;
                    excelWorksheet.Cells[row, 5].Value = person.Address;
                    excelWorksheet.Cells[row, 6].Value = person.Gender;
                    excelWorksheet.Cells[row, 7].Value = person.Country;
                    excelWorksheet.Cells[row, 8].Value = person.ReceivNewsLetters;

                    row++;
                }
                excelWorksheet.Cells[$"A1:H{row}"].AutoFitColumns();

                await excelPackage.SaveAsync();
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        public async Task<PersonResponse> ToAddPersonRequest(AddPersonRequest? addPersonRequest)
        {
            if (addPersonRequest == null)
            {
                throw new ArgumentNullException(nameof(addPersonRequest));
            }

            ValidationHelper.ModelValidation(addPersonRequest);

            Person? person = addPersonRequest.ToAddPerson();

            person.PersonId = Guid.NewGuid();

            await _db.AddPerson(person);

            //_db.AddPerson_SP(person);

            return person.ToPersonResponse();

        }

        public async Task<PersonResponse> UpdateThePersonDate(UpdatePersonRequest? updatePersonRequest)
        {
            if (updatePersonRequest == null) { 
               throw new ArgumentNullException(nameof(updatePersonRequest));
            }

            ValidationHelper.ModelValidation(updatePersonRequest);

            Person? MatchedPerson = await _db.GetPersonByPersonId(updatePersonRequest.PersonId);
       
            if (MatchedPerson == null) throw new ArgumentException(nameof(MatchedPerson.PersonId));
                         
            MatchedPerson.PersonName = updatePersonRequest.PersonName;
            MatchedPerson.CountryId = updatePersonRequest.CountryId;
            MatchedPerson.Email = updatePersonRequest.Email;
            MatchedPerson.DateofBirth = updatePersonRequest.DateofBirth;
            MatchedPerson.Gender = updatePersonRequest.Gender.ToString();
            MatchedPerson.ReceivNewsLetters = updatePersonRequest.ReceivNewsLetters;
            MatchedPerson.Address = updatePersonRequest.Address;

            await _db.UpdateThePersonData(MatchedPerson);

            return MatchedPerson.ToPersonResponse();
        }
    }
}
