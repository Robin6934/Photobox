using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

#nullable disable

namespace Photobox.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddedPhotoBoxModelWithRelationToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("UniqeImageName", "ImageModels", "UniqueImageName");

            migrationBuilder.AlterColumn<string>(
                name: "UniqueImageName",
                table: "ImageModels",
                type: "character varying(45)",
                maxLength: 45,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AlterColumn<string>(
                name: "ImageName",
                table: "ImageModels",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AlterColumn<string>(
                name: "DownscaledImageName",
                table: "ImageModels",
                type: "character varying(45)",
                maxLength: 45,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.CreateTable(
                name: "PhotoBoxModels",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoBoxModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotoBoxModels_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_ImageModels_ImageName",
                table: "ImageModels",
                column: "ImageName"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PhotoBoxModels_UserId",
                table: "PhotoBoxModels",
                column: "UserId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PhotoBoxModels");

            migrationBuilder.DropIndex(name: "IX_ImageModels_ImageName", table: "ImageModels");

            migrationBuilder.AlterColumn<string>(
                name: "UniqueImageName",
                table: "ImageModels",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(45)",
                oldMaxLength: 45
            );

            migrationBuilder.AlterColumn<string>(
                name: "ImageName",
                table: "ImageModels",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64
            );

            migrationBuilder.AlterColumn<string>(
                name: "DownscaledImageName",
                table: "ImageModels",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(45)",
                oldMaxLength: 45
            );
        }
    }
}
