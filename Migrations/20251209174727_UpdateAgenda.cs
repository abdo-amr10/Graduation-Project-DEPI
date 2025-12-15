using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace My_Uni_Hub.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAgenda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventDate",
                table: "UniversityAgendas",
                newName: "StartDate");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "UniversityAgendas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "UniversityAgendas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "UniversityAgendas");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "UniversityAgendas",
                newName: "EventDate");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "UniversityAgendas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
