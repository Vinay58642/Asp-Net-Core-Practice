using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Enums;

namespace Entities
{
    public class Person
    {
        [Key]
        public Guid PersonId { get; set; }

        [StringLength(40)]
        public string? PersonName { get; set; }

        [StringLength(40)]
        public String? Email { get; set; }
        public DateTime? DateofBirth { get; set; }
        
        public bool ReceivNewsLetters { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }
        public Guid? CountryId { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        public string? TIN { get; set; }

        [ForeignKey(nameof(CountryId))]
        public virtual Country? country { get; set; }
    }
}
