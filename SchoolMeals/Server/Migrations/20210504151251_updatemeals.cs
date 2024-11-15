using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolMeals.Server.Migrations
{
    public partial class updatemeals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Dislike",
                table: "Meal",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Like",
                table: "Meal",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dislike",
                table: "Meal");

            migrationBuilder.DropColumn(
                name: "Like",
                table: "Meal");
        }
    }
}
