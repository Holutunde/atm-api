using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FourthMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pin",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "PinHash",
                schema: "Identity",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PinHash",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "Pin",
                schema: "Identity",
                table: "AspNetUsers",
                type: "int",
                maxLength: 4,
                nullable: false,
                defaultValue: 0);
        }
    }
}
