using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using System.Net.Http.Headers;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    public class AddPersonRequest
    {
        [Required(ErrorMessage ="Person Name Can't Be Blank")]
        public string? PersonName { get; set; }
        [Required(ErrorMessage ="Email Can't Be Bull or Empty")]
        [EmailAddress(ErrorMessage ="Email Must Be In Currect Format")]
        public String? Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateofBirth { get; set; }
        public bool ReceivNewsLetters { get; set; }
        [Required(ErrorMessage = "Address Can't Be Blank")]
        public string? Address { get; set; }
        [Required(ErrorMessage = "Country Can't Be Un-Select")]
        public Guid? CountryId { get; set; }
        [Required(ErrorMessage = "Gender Can't Be Unselect")]
        public GenderEnum? Gender { get; set; }

        public Person ToAddPerson()
        {
            return new Person() { PersonName=PersonName, Address=Address,Email=Email, CountryId = CountryId,
             DateofBirth=DateofBirth,Gender=Gender.ToString(), ReceivNewsLetters=ReceivNewsLetters };
        }
    }
}
