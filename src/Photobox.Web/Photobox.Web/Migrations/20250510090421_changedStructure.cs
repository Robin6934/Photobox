using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Photobox.Web.Migrations
{
    /// <inheritdoc />
    public partial class changedStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_PhotoBoxes_PhotoBoxId",
                table: "Images"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_PhotoBoxes_Events_EventId",
                table: "PhotoBoxes"
            );

            migrationBuilder.DropIndex(name: "IX_PhotoBoxes_EventId", table: "PhotoBoxes");

            migrationBuilder.DropIndex(name: "IX_Images_PhotoBoxId", table: "Images");

            migrationBuilder.DropColumn(name: "PhotoBoxId", table: "Images");

            migrationBuilder.DropColumn(name: "PhotoboxHardwareId", table: "Images");

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentEventId",
                table: "PhotoBoxes",
                type: "uuid",
                nullable: true
            );

            migrationBuilder.AddColumn<Guid>(
                name: "UsedPhotoBoxId",
                table: "Events",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );

            migrationBuilder.CreateIndex(
                name: "IX_PhotoBoxes_CurrentEventId",
                table: "PhotoBoxes",
                column: "CurrentEventId",
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoBoxes_Events_CurrentEventId",
                table: "PhotoBoxes",
                column: "CurrentEventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhotoBoxes_Events_CurrentEventId",
                table: "PhotoBoxes"
            );

            migrationBuilder.DropIndex(name: "IX_PhotoBoxes_CurrentEventId", table: "PhotoBoxes");

            migrationBuilder.DropColumn(name: "CurrentEventId", table: "PhotoBoxes");

            migrationBuilder.DropColumn(name: "UsedPhotoBoxId", table: "Events");

            migrationBuilder.AddColumn<Guid>(
                name: "PhotoBoxId",
                table: "Images",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );

            migrationBuilder.AddColumn<string>(
                name: "PhotoboxHardwareId",
                table: "Images",
                type: "character varying(52)",
                maxLength: 52,
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.CreateIndex(
                name: "IX_PhotoBoxes_EventId",
                table: "PhotoBoxes",
                column: "EventId",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Images_PhotoBoxId",
                table: "Images",
                column: "PhotoBoxId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Images_PhotoBoxes_PhotoBoxId",
                table: "Images",
                column: "PhotoBoxId",
                principalTable: "PhotoBoxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoBoxes_Events_EventId",
                table: "PhotoBoxes",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );
        }
    }
}
