using System.ComponentModel.DataAnnotations;

namespace Entities
{
    /// <summary>
    /// Domain model for add country data to a list of countries or else data table in a DB.
    /// </summary>
    public class Country
    {
        [Key]
        public Guid Countryid { get; set; }

        [StringLength(40)]
        public string? CountryName { get; set; }

        public virtual ICollection<Person>? Persons { get; set; }
    }
}
