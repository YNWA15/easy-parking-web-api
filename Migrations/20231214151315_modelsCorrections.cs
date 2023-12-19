using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PublicParkingsSofiaWebAPI.Migrations
{
    public partial class modelsCorrections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Parkings_ParkingId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ParkingId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HoursPerMonth",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ParkingId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SalaryPerHour",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SignDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsSecured",
                table: "Parkings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "HoursPerMonth",
                table: "Users",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParkingId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SalaryPerHour",
                table: "Users",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SignDate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSecured",
                table: "Parkings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ParkingId",
                table: "Users",
                column: "ParkingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Parkings_ParkingId",
                table: "Users",
                column: "ParkingId",
                principalTable: "Parkings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
