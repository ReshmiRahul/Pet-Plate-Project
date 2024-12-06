using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pet_Adoption_Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class createproject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountState = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationId);
                });

            migrationBuilder.CreateTable(
                name: "Pets",
                columns: table => new
                {
                    PetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PetName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetBreed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetAge = table.Column<int>(type: "int", nullable: false),
                    PetDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pets", x => x.PetId);
                });

            migrationBuilder.CreateTable(
                name: "FoodTrucks",
                columns: table => new
                {
                    FoodTruckId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contact = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodTrucks", x => x.FoodTruckId);
                    table.ForeignKey(
                        name: "FK_FoodTrucks_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountPet",
                columns: table => new
                {
                    AccountsAccountId = table.Column<int>(type: "int", nullable: false),
                    PetsPetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountPet", x => new { x.AccountsAccountId, x.PetsPetId });
                    table.ForeignKey(
                        name: "FK_AccountPet_Accounts_AccountsAccountId",
                        column: x => x.AccountsAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountPet_Pets_PetsPetId",
                        column: x => x.PetsPetId,
                        principalTable: "Pets",
                        principalColumn: "PetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    ApplicationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    PetId = table.Column<int>(type: "int", nullable: false),
                    FoodTruckId = table.Column<int>(type: "int", nullable: true),
                    ApplicationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicationStatus = table.Column<int>(type: "int", nullable: false),
                    ApplicationReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationExperience = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationComments = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.ApplicationID);
                    table.ForeignKey(
                        name: "FK_Applications_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applications_FoodTrucks_FoodTruckId",
                        column: x => x.FoodTruckId,
                        principalTable: "FoodTrucks",
                        principalColumn: "FoodTruckId");
                    table.ForeignKey(
                        name: "FK_Applications_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "PetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    MenuItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FoodTruckId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.MenuItemId);
                    table.ForeignKey(
                        name: "FK_MenuItems_FoodTrucks_FoodTruckId",
                        column: x => x.FoodTruckId,
                        principalTable: "FoodTrucks",
                        principalColumn: "FoodTruckId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountPet_PetsPetId",
                table: "AccountPet",
                column: "PetsPetId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_AccountId",
                table: "Applications",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_FoodTruckId",
                table: "Applications",
                column: "FoodTruckId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_PetId",
                table: "Applications",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodTrucks_LocationId",
                table: "FoodTrucks",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_FoodTruckId",
                table: "MenuItems",
                column: "FoodTruckId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountPet");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Pets");

            migrationBuilder.DropTable(
                name: "FoodTrucks");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
