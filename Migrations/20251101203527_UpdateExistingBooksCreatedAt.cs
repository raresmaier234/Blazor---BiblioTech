using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorLibraryApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExistingBooksCreatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Actualizează cărțile existente care au CreatedAt la valoarea default (0001-01-01)
            // Le setăm la data curentă
            migrationBuilder.Sql(
                @"UPDATE Books 
                  SET CreatedAt = datetime('now', 'localtime') 
                  WHERE CreatedAt < '0002-01-01'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Nu este nevoie de rollback pentru această migrație
        }
    }
}
