using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Photobox.Web.Migrations
{
    /// <inheritdoc />
    public partial class ChangedEventPhotoboxRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhotoBoxes_Events_CurrentEventId",
                table: "PhotoBoxes"
            );

            migrationBuilder.DropIndex(name: "IX_PhotoBoxes_CurrentEventId", table: "PhotoBoxes");

            migrationBuilder.DropColumn(name: "CurrentEventId", table: "PhotoBoxes");

            migrationBuilder.AlterColumn<Guid>(
                name: "EventId",
                table: "Images",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid"
            );

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Events",
                type: "boolean",
                nullable: false,
                defaultValue: false
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "IsActive", table: "Events");

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentEventId",
                table: "PhotoBoxes",
                type: "uuid",
                nullable: true
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "EventId",
                table: "Images",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true
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
    }
}
