using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Photobox.Web.Migrations
{
    /// <inheritdoc />
    public partial class RemovedGalleryId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PhotoBoxModels_GalleryId",
                table: "PhotoBoxModels"
            );

            migrationBuilder.DropColumn(name: "GalleryId", table: "PhotoBoxModels");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GalleryId",
                table: "PhotoBoxModels",
                type: "char(6)",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.CreateIndex(
                name: "IX_PhotoBoxModels_GalleryId",
                table: "PhotoBoxModels",
                column: "GalleryId",
                unique: true
            );
        }
    }
}
