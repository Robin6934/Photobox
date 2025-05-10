using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Photobox.Web.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedModelBuilder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "EventId", table: "PhotoBoxes");

            migrationBuilder.CreateIndex(
                name: "IX_Events_UsedPhotoBoxId",
                table: "Events",
                column: "UsedPhotoBoxId",
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Events_PhotoBoxes_UsedPhotoBoxId",
                table: "Events",
                column: "UsedPhotoBoxId",
                principalTable: "PhotoBoxes",
                principalColumn: "Id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_PhotoBoxes_UsedPhotoBoxId",
                table: "Events"
            );

            migrationBuilder.DropIndex(name: "IX_Events_UsedPhotoBoxId", table: "Events");

            migrationBuilder.AddColumn<Guid>(
                name: "EventId",
                table: "PhotoBoxes",
                type: "uuid",
                nullable: true
            );
        }
    }
}
