using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bike_matrix_tech_int.Server.Migrations
{
    /// <inheritdoc />
    public partial class UseIdAsKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Bikes",
                table: "Bikes");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Bikes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bikes",
                table: "Bikes",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Bikes",
                table: "Bikes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Bikes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bikes",
                table: "Bikes",
                column: "Email");
        }
    }
}
