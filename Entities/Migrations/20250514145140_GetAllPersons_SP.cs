using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class GetAllPersons_SP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string SpforAllPersons = @"CREATE PROCEDURE [dbo].[GetAllPersons] " +
                "AS BEGIN " +
                "SELECT PersonId, PersonName, Email, DateofBirth, Gender, CountryId, Address, ReceivNewsLetters FROM [dbo].[Persons]" +
                "END";
            migrationBuilder.Sql(SpforAllPersons);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string DelteSPforPersons = @"DROP PROCEDURE [dbo].[GetAllPersons]";

            migrationBuilder.Sql(DelteSPforPersons);
        }
    }
}
