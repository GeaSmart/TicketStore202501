using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicStore.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class customersale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer",
                schema: "Musicales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sale",
                schema: "Musicales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ConcertId = table.Column<int>(type: "int", nullable: false),
                    SaleDate = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "GETDATE()"),
                    OperationNumber = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Quantity = table.Column<short>(type: "smallint", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sale", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sale_Concert_ConcertId",
                        column: x => x.ConcertId,
                        principalSchema: "Musicales",
                        principalTable: "Concert",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sale_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "Musicales",
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sale_ConcertId",
                schema: "Musicales",
                table: "Sale",
                column: "ConcertId");

            migrationBuilder.CreateIndex(
                name: "IX_Sale_CustomerId",
                schema: "Musicales",
                table: "Sale",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sale",
                schema: "Musicales");

            migrationBuilder.DropTable(
                name: "Customer",
                schema: "Musicales");
        }
    }
}
