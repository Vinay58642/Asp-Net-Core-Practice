using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _db;

        public PersonsRepository(ApplicationDbContext applicationDbContext) 
        {
            _db = applicationDbContext;
        }
        public async Task<Person> AddPerson(Person person)
        {
            _db.Persons.AddAsync(person);
            await _db.SaveChangesAsync();

            return person;
        }

        public async Task<bool> Delete_PersonByPersonId(Guid? PersonId)
        {
            _db.Persons.Remove(await _db.Persons.FirstAsync(tem => tem.PersonId == PersonId));

            int rows=await _db.SaveChangesAsync();

            return rows>0;
        }

        public async Task<List<Person?>> GetAllPersons()
        {
            return await _db.Persons.Include("country").ToListAsync();
              
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            return await _db.Persons.Include("country").Where(predicate).ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonId(Guid? personId)
        {
            return await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonId == personId);
        }

        public async Task<Person> UpdateThePersonData(Person person)
        {
            Person? MatchedPerson = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonId == person.PersonId);

            if (MatchedPerson == null) return person;

            MatchedPerson.PersonName = person.PersonName;
            MatchedPerson.CountryId = person.CountryId;
            MatchedPerson.Email = person.Email;
            MatchedPerson.DateofBirth = person.DateofBirth;
            MatchedPerson.Gender = person.Gender;
            MatchedPerson.ReceivNewsLetters = person.ReceivNewsLetters;
            MatchedPerson.Address = person.Address;

            await _db.SaveChangesAsync();

            return MatchedPerson;
        }
    }
}
