using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Photobox.Web.Migrations
{
    /// <inheritdoc />
    public partial class changedimageIdtouniqeImageName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "ImageModels");

            migrationBuilder.AddColumn<string>(
                name: "UniqeImageName",
                table: "ImageModels",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UniqeImageName",
                table: "ImageModels");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageId",
                table: "ImageModels",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");
        }
    }
}
