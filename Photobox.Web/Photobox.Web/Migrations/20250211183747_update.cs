using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Photobox.Web.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "SerialNumber", table: "PhotoBoxModels");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PhotoBoxModels",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "PhotoBoxModels",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AddColumn<string>(
                name: "PhotoboxId",
                table: "PhotoBoxModels",
                type: "character varying(52)",
                maxLength: 52,
                nullable: false,
                defaultValue: ""
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "PhotoboxId", table: "PhotoBoxModels");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PhotoBoxModels",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50
            );

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "PhotoBoxModels",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50
            );

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "PhotoBoxModels",
                type: "text",
                nullable: false,
                defaultValue: ""
            );
        }
    }
}
