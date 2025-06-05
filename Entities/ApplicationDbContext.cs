using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options): base(options) { }
        public virtual DbSet<Person> Persons { get; set; }

        public virtual DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
                  

            modelBuilder.Entity<Country>().ToTable("Countries");

            modelBuilder.Entity<Person>().ToTable("Persons");
                     

            //Seed to Countries
            string countriesjson = System.IO.File.ReadAllText("countries.json");
            
            List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesjson);

            foreach (Country country in countries) 
              modelBuilder.Entity<Country>().HasData(country);

            //Seed to Persons
            string personsjoson = System.IO.File.ReadAllText("persons.json");

            List<Person> Persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsjoson);

            foreach (Person person in Persons)
                modelBuilder.Entity<Person>().HasData(person);
            // modelBuilder.Entity<Person>().HasData(new Person
            //{
            //     PersonId= Guid.Parse("c03bbe45-9aeb-4d24-99e0-4743016ffce9"),
            //     PersonName= "Marguerite",
            //     Email= "mwebsdale0@people.com.cn",
            //     DateofBirth= Convert.ToDateTime("1989-08-28"),
            //     Gender= "Female",
            //     CountryId= Guid.Parse("56bf46a4-02b8-4693-a0f5-0a95e2218bdc"),
            //     Address= "4 Parkside Point",
            //     ReceivNewsLetters= false
            // },
            // new Person
            // {
            //     PersonId = Guid.Parse("c3abddbd-cf50-41d2-b6c4-cc7d5a750928"),
            //     PersonName = "Ursa",
            //     Email = "ushears1@globo.com",
            //     DateofBirth = Convert.ToDateTime("1990-10-05"),
            //     Gender = "Female",
            //     CountryId = Guid.Parse("14629847-905a-4a0e-9abe-80b61655c5cb"),
            //     Address = "6 Morningstar Circle",
            //     ReceivNewsLetters = false
            // }); 

            modelBuilder.Entity<Person>().Property(temp => temp.TIN).
                   HasColumnName("Tax_Identi_Num").
                   HasColumnType("varchar(8)").
                   HasDefaultValue("ABCDS");

            modelBuilder.Entity<Person>().HasCheckConstraint("CHK_TIN", "len([Tax_Identi_Num])=8");
              

        }
        public List<Person> Get_AllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }

        public int AddPerson_SP(Person person)
        {
            SqlParameter[] sqlParameters = new SqlParameter[] {
                new SqlParameter("@PersonId", person.PersonId),
                new SqlParameter("@PersonName", person.PersonName),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@DateofBirth", person.DateofBirth),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryId", person.CountryId),
                new SqlParameter("@Address", person.Address),
                 new SqlParameter("@ReceivNewsLetters", person.ReceivNewsLetters)
            };

            return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @PersonId, @PersonName, @Email, @DateofBirth, @Gender, @CountryId, @Address, @ReceivNewsLetters", sqlParameters);
        }
    }
}
