using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pet_Adoption_Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateACcount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountCity",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "AccountState",
                table: "Accounts");

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_LocationId",
                table: "Accounts",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Locations_LocationId",
                table: "Accounts",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Locations_LocationId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_LocationId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Accounts");

            migrationBuilder.AddColumn<string>(
                name: "AccountCity",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AccountState",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
