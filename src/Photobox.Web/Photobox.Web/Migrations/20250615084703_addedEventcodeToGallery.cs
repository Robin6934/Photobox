using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Photobox.Web.Migrations
{
    /// <inheritdoc />
    public partial class addedEventcodeToGallery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "EventCode",
                table: "Events",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventCode",
                table: "Events",
                column: "EventCode",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Events_EventCode", table: "Events");

            migrationBuilder.DropColumn(name: "EventCode", table: "Events");

            migrationBuilder.AlterColumn<Guid>(
                name: "EventId",
                table: "Images",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid"
            );
        }
    }
}
