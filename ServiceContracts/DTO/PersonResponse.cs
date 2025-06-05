using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using System.Runtime.CompilerServices;
using System.Net;
using System.Reflection;

namespace ServiceContracts.DTO
{
    public class PersonResponse
    {
        public Guid PersonId { get; set; }
        public string? PersonName { get; set; }
        public String? Email { get; set; }
        public DateTime? DateofBirth { get; set; }
        public bool ReceivNewsLetters { get; set; }
        public string? Address { get; set; }
        public Guid? CountryId { get; set; }
        public string? Country { get; set; }
        public string? Gender { get; set; }
        public double? Age { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false; 
            if (obj.GetType() != typeof(PersonResponse)) return false;

            PersonResponse other = (PersonResponse)obj;
            return PersonId == other.PersonId && PersonName == other.PersonName &&
                Email == other.Email && Address == other.Address &&
                Country == other.Country && CountryId == other.CountryId &&
                DateofBirth == other.DateofBirth && ReceivNewsLetters == other.ReceivNewsLetters &&
                Gender == other.Gender;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"Person Id : {PersonId}, Person Name : {PersonName}, Email : {Email}," +
                $"Address : {Address}, Age : {Age}, Country : {Country}";
        }

        public UpdatePersonRequest ToUpdatePersonRequest()
        {
            return new UpdatePersonRequest()
            {
                PersonId=PersonId, Gender= (GenderEnum)Enum.Parse(typeof(GenderEnum), Gender, true), Address=Address,CountryId=CountryId, DateofBirth=DateofBirth,
                ReceivNewsLetters=ReceivNewsLetters, Email=Email, PersonName = PersonName
            };
        }

    }

    public static class PersonExtensions
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse() {
                PersonId = person.PersonId,
                PersonName = person.PersonName,
                Address = person.Address,
                Email = person.Email,
                CountryId = person.CountryId,
                DateofBirth = person.DateofBirth,
                Gender = person.Gender.ToString(),
                ReceivNewsLetters = person.ReceivNewsLetters,
                Age=(person.DateofBirth != null) ? Math.Round((DateTime.Now-person.DateofBirth.Value).TotalDays/365.25) : 0,
                Country= person.country?.CountryName
            };
        }
    }
   
}
