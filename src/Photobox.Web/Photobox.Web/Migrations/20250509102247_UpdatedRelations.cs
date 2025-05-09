using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Photobox.Web.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventModels_AspNetUsers_ApplicationUserId",
                table: "EventModels"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_ImageModels_EventModels_EventId",
                table: "ImageModels"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_ImageModels_PhotoBoxModels_PhotoboxId",
                table: "ImageModels"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_PhotoBoxModels_AspNetUsers_ApplicationUserId",
                table: "PhotoBoxModels"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_PhotoBoxModels_EventModels_EventId",
                table: "PhotoBoxModels"
            );

            migrationBuilder.DropPrimaryKey(name: "PK_PhotoBoxModels", table: "PhotoBoxModels");

            migrationBuilder.DropIndex(name: "IX_PhotoBoxModels_EventId", table: "PhotoBoxModels");

            migrationBuilder.DropPrimaryKey(name: "PK_ImageModels", table: "ImageModels");

            migrationBuilder.DropPrimaryKey(name: "PK_EventModels", table: "EventModels");

            migrationBuilder.RenameTable(name: "PhotoBoxModels", newName: "PhotoBoxes");

            migrationBuilder.RenameTable(name: "ImageModels", newName: "Images");

            migrationBuilder.RenameTable(name: "EventModels", newName: "Events");

            migrationBuilder.RenameColumn(
                name: "PhotoboxId",
                table: "PhotoBoxes",
                newName: "HardwareId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_PhotoBoxModels_PhotoboxId",
                table: "PhotoBoxes",
                newName: "IX_PhotoBoxes_HardwareId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_PhotoBoxModels_ApplicationUserId",
                table: "PhotoBoxes",
                newName: "IX_PhotoBoxes_ApplicationUserId"
            );

            migrationBuilder.RenameColumn(
                name: "PhotoboxId",
                table: "Images",
                newName: "PhotoBoxId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_ImageModels_PhotoboxId",
                table: "Images",
                newName: "IX_Images_PhotoBoxId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_ImageModels_ImageName",
                table: "Images",
                newName: "IX_Images_ImageName"
            );

            migrationBuilder.RenameIndex(
                name: "IX_ImageModels_EventId",
                table: "Images",
                newName: "IX_Images_EventId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_EventModels_ApplicationUserId",
                table: "Events",
                newName: "IX_Events_ApplicationUserId"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PhotoBoxes",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "PhotoBoxId",
                table: "Images",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "PhotoboxHardwareId",
                table: "Images",
                type: "character varying(52)",
                maxLength: 52,
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Events",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Events",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Events",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhotoBoxes",
                table: "PhotoBoxes",
                column: "Id"
            );

            migrationBuilder.AddPrimaryKey(name: "PK_Images", table: "Images", column: "Id");

            migrationBuilder.AddPrimaryKey(name: "PK_Events", table: "Events", column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoBoxes_EventId",
                table: "PhotoBoxes",
                column: "EventId",
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_ApplicationUserId",
                table: "Events",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Events_EventId",
                table: "Images",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
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
                name: "FK_PhotoBoxes_AspNetUsers_ApplicationUserId",
                table: "PhotoBoxes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_ApplicationUserId",
                table: "Events"
            );

            migrationBuilder.DropForeignKey(name: "FK_Images_Events_EventId", table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_PhotoBoxes_PhotoBoxId",
                table: "Images"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_PhotoBoxes_AspNetUsers_ApplicationUserId",
                table: "PhotoBoxes"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_PhotoBoxes_Events_EventId",
                table: "PhotoBoxes"
            );

            migrationBuilder.DropPrimaryKey(name: "PK_PhotoBoxes", table: "PhotoBoxes");

            migrationBuilder.DropIndex(name: "IX_PhotoBoxes_EventId", table: "PhotoBoxes");

            migrationBuilder.DropPrimaryKey(name: "PK_Images", table: "Images");

            migrationBuilder.DropPrimaryKey(name: "PK_Events", table: "Events");

            migrationBuilder.DropColumn(name: "PhotoboxHardwareId", table: "Images");

            migrationBuilder.DropColumn(name: "EndDate", table: "Events");

            migrationBuilder.DropColumn(name: "StartDate", table: "Events");

            migrationBuilder.RenameTable(name: "PhotoBoxes", newName: "PhotoBoxModels");

            migrationBuilder.RenameTable(name: "Images", newName: "ImageModels");

            migrationBuilder.RenameTable(name: "Events", newName: "EventModels");

            migrationBuilder.RenameColumn(
                name: "HardwareId",
                table: "PhotoBoxModels",
                newName: "PhotoboxId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_PhotoBoxes_HardwareId",
                table: "PhotoBoxModels",
                newName: "IX_PhotoBoxModels_PhotoboxId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_PhotoBoxes_ApplicationUserId",
                table: "PhotoBoxModels",
                newName: "IX_PhotoBoxModels_ApplicationUserId"
            );

            migrationBuilder.RenameColumn(
                name: "PhotoBoxId",
                table: "ImageModels",
                newName: "PhotoboxId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_Images_PhotoBoxId",
                table: "ImageModels",
                newName: "IX_ImageModels_PhotoboxId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_Images_ImageName",
                table: "ImageModels",
                newName: "IX_ImageModels_ImageName"
            );

            migrationBuilder.RenameIndex(
                name: "IX_Images_EventId",
                table: "ImageModels",
                newName: "IX_ImageModels_EventId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_Events_ApplicationUserId",
                table: "EventModels",
                newName: "IX_EventModels_ApplicationUserId"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PhotoBoxModels",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "PhotoboxId",
                table: "ImageModels",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EventModels",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhotoBoxModels",
                table: "PhotoBoxModels",
                column: "Id"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImageModels",
                table: "ImageModels",
                column: "Id"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventModels",
                table: "EventModels",
                column: "Id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PhotoBoxModels_EventId",
                table: "PhotoBoxModels",
                column: "EventId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_EventModels_AspNetUsers_ApplicationUserId",
                table: "EventModels",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ImageModels_EventModels_EventId",
                table: "ImageModels",
                column: "EventId",
                principalTable: "EventModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ImageModels_PhotoBoxModels_PhotoboxId",
                table: "ImageModels",
                column: "PhotoboxId",
                principalTable: "PhotoBoxModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoBoxModels_AspNetUsers_ApplicationUserId",
                table: "PhotoBoxModels",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoBoxModels_EventModels_EventId",
                table: "PhotoBoxModels",
                column: "EventId",
                principalTable: "EventModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );
        }
    }
}
