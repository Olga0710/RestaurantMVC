using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RestaurantInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStationToTraining : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmpoyeeID",
                table: "Workers",
                newName: "EmployeeID");

            migrationBuilder.RenameColumn(
                name: "EmpoyeeID",
                table: "Shifts",
                newName: "EmployeeID");

            migrationBuilder.RenameIndex(
                name: "IX_Shifts_EmpoyeeID",
                table: "Shifts",
                newName: "IX_Shifts_EmployeeID");

            migrationBuilder.RenameColumn(
                name: "EmloyeeID",
                table: "Managers",
                newName: "EmployeeID");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "TrainingProgress",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Station",
                table: "TrainingProgress",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "StartTime",
                table: "Shifts",
                type: "time",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "time with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EndTime",
                table: "Shifts",
                type: "time",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "time with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "Shifts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DataGranted",
                table: "Bonuses",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "time with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "Bonuses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Station",
                table: "TrainingProgress");

            migrationBuilder.RenameColumn(
                name: "EmployeeID",
                table: "Workers",
                newName: "EmpoyeeID");

            migrationBuilder.RenameColumn(
                name: "EmployeeID",
                table: "Shifts",
                newName: "EmpoyeeID");

            migrationBuilder.RenameIndex(
                name: "IX_Shifts_EmployeeID",
                table: "Shifts",
                newName: "IX_Shifts_EmpoyeeID");

            migrationBuilder.RenameColumn(
                name: "EmployeeID",
                table: "Managers",
                newName: "EmloyeeID");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "TrainingProgress",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "StartTime",
                table: "Shifts",
                type: "time with time zone",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "EndTime",
                table: "Shifts",
                type: "time with time zone",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "Shifts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DataGranted",
                table: "Bonuses",
                type: "time with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "Bonuses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
