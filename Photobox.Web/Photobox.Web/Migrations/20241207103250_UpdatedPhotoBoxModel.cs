using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Photobox.Web.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedPhotoBoxModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TempId",
                table: "PhotoBoxModels",
                type: "uuid",
                nullable: true
            );

            // Convert valid string-based UUIDs to UUID type
            migrationBuilder.Sql(
                @"UPDATE ""PhotoBoxModels"" 
                  SET ""TempId"" = ""Id""::uuid 
                  WHERE ""Id"" ~* '^[0-9a-fA-F-]{36}$'"
            );

            // Update the schema and drop the old column
            migrationBuilder.DropColumn(name: "Id", table: "PhotoBoxModels");

            migrationBuilder.RenameColumn(name: "TempId", table: "PhotoBoxModels", newName: "Id");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PhotoBoxModels",
                type: "uuid",
                nullable: false
            );

            migrationBuilder.DropForeignKey(
                name: "FK_PhotoBoxModels_AspNetUsers_UserId",
                table: "PhotoBoxModels"
            );

            migrationBuilder.DropIndex(name: "IX_PhotoBoxModels_UserId", table: "PhotoBoxModels");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PhotoBoxModels",
                newName: "SerialNumber"
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PhotoBoxModels",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "PhotoBoxModels",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PhotoBoxModels",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.CreateIndex(
                name: "IX_PhotoBoxModels_ApplicationUserId",
                table: "PhotoBoxModels",
                column: "ApplicationUserId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoBoxModels_AspNetUsers_ApplicationUserId",
                table: "PhotoBoxModels",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhotoBoxModels_AspNetUsers_ApplicationUserId",
                table: "PhotoBoxModels"
            );

            migrationBuilder.DropIndex(
                name: "IX_PhotoBoxModels_ApplicationUserId",
                table: "PhotoBoxModels"
            );

            migrationBuilder.DropColumn(name: "ApplicationUserId", table: "PhotoBoxModels");

            migrationBuilder.DropColumn(name: "Name", table: "PhotoBoxModels");

            migrationBuilder.RenameColumn(
                name: "SerialNumber",
                table: "PhotoBoxModels",
                newName: "UserId"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "PhotoBoxModels",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PhotoBoxModels_UserId",
                table: "PhotoBoxModels",
                column: "UserId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoBoxModels_AspNetUsers_UserId",
                table: "PhotoBoxModels",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
