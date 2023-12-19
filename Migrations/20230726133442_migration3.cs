using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PublicParkingsSofiaWebAPI.Migrations
{
    public partial class migration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is1hrAdded",
                table: "Reservations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is1hrAdded",
                table: "Reservations");
        }
    }
}
