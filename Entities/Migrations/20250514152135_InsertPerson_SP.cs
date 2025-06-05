using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class InsertPerson_SP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string SpInsertPerson = @"CREATE PROCEDURE [dbo].[InsertPerson] 
                   (@PersonId UNIQUEIDENTIFIER, @PersonName NVARCHAR (40), @Email NVARCHAR (40), @DateofBirth  DATETIME2 (7), @Gender NVARCHAR (10), @CountryId UNIQUEIDENTIFIER, @Address NVARCHAR (200), @ReceivNewsLetters BIT)  " +
                "AS BEGIN " +
                "INSERT INTO [dbo].[Persons] (PersonId, PersonName, Email, DateofBirth, Gender, CountryId, Address, ReceivNewsLetters)" +
                " VALUES(@PersonId, @PersonName, @Email, @DateofBirth, @Gender, @CountryId, @Address, @ReceivNewsLetters)" +
                "END";

            migrationBuilder.Sql(SpInsertPerson);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string DelteSPforPerson = @"DROP PROCEDURE [dbo].[InsertPerson]";

            migrationBuilder.Sql(DelteSPforPerson);
        }
    }
}
