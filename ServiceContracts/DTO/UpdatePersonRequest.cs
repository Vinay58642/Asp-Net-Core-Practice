using Entities.Enums;
using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class UpdatePersonRequest
    {
        [Required(ErrorMessage = "Person Name Can't Be Blank")]
        public Guid PersonId { get; set; }

        [Required(ErrorMessage = "Person Name Can't Be Blank")]
        public string? PersonName { get; set; }
        [Required(ErrorMessage = "Email Can't Be Bull or Empty")]
        [EmailAddress(ErrorMessage = "Email Must Be In Currect Format")]
        public String? Email { get; set; }
        public DateTime? DateofBirth { get; set; }
        public bool ReceivNewsLetters { get; set; }
        public string? Address { get; set; }
        public Guid? CountryId { get; set; }
        public GenderEnum? Gender { get; set; }

        public Person ToAddPerson()
        {
            return new Person()
            {
                PersonId=PersonId,
                PersonName = PersonName,
                Address = Address,
                Email = Email,
                CountryId = CountryId,
                DateofBirth = DateofBirth,
                Gender = Gender.ToString(),
                ReceivNewsLetters = ReceivNewsLetters
            };
        }
    }
}
