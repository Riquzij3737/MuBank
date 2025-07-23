using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mubank.Migrations
{
    /// <inheritdoc />
    public partial class initial41 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transations_Accounts_DestinatarioId",
                table: "Transations");

            migrationBuilder.DropForeignKey(
                name: "FK_Transations_Accounts_RemetenteId",
                table: "Transations");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Transations_DestinatarioId",
                table: "Transations");

            migrationBuilder.DropIndex(
                name: "IX_Transations_RemetenteId",
                table: "Transations");

            migrationBuilder.RenameColumn(
                name: "RemetenteId",
                table: "Transations",
                newName: "IDDequemrecebeu");

            migrationBuilder.RenameColumn(
                name: "DestinatarioId",
                table: "Transations",
                newName: "IDDequemfez");

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "Users",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IPsBlocked");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "IDDequemrecebeu",
                table: "Transations",
                newName: "RemetenteId");

            migrationBuilder.RenameColumn(
                name: "IDDequemfez",
                table: "Transations",
                newName: "DestinatarioId");

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transations_DestinatarioId",
                table: "Transations",
                column: "DestinatarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Transations_RemetenteId",
                table: "Transations",
                column: "RemetenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserId",
                table: "Accounts",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transations_Accounts_DestinatarioId",
                table: "Transations",
                column: "DestinatarioId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transations_Accounts_RemetenteId",
                table: "Transations",
                column: "RemetenteId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
