using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddBlogOwnerFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedUtc",
                table: "Blogs",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryOwnerUserId",
                table: "Blogs",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryOwnerUserName",
                table: "Blogs",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_PrimaryOwnerUserId",
                table: "Blogs",
                column: "PrimaryOwnerUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Blogs_PrimaryOwnerUserId",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "CreatedUtc",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "PrimaryOwnerUserId",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "PrimaryOwnerUserName",
                table: "Blogs");
        }
    }
}
