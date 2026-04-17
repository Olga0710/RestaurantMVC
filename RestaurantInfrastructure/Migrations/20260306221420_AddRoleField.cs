using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Employers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Employers");
        }
    }
}
