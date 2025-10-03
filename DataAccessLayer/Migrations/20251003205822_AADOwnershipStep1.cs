using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AADOwnershipStep1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogComments_BlogComments_ParentCommentID",
                table: "BlogComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tags",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "IX_BlogComments_BlogPostID",
                table: "BlogComments");

            migrationBuilder.RenameTable(
                name: "tags",
                newName: "Tags");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tags",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerUserName",
                table: "BlogComments",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerUserId",
                table: "BlogComments",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "TagID");

            migrationBuilder.CreateIndex(
                name: "IX_BlogComments_BlogPostID_BlogCommentID",
                table: "BlogComments",
                columns: new[] { "BlogPostID", "BlogCommentID" });

            migrationBuilder.CreateIndex(
                name: "IX_BlogComments_OwnerUserId",
                table: "BlogComments",
                column: "OwnerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogComments_BlogComments_ParentCommentID",
                table: "BlogComments",
                column: "ParentCommentID",
                principalTable: "BlogComments",
                principalColumn: "BlogCommentID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogComments_BlogComments_ParentCommentID",
                table: "BlogComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_BlogComments_BlogPostID_BlogCommentID",
                table: "BlogComments");

            migrationBuilder.DropIndex(
                name: "IX_BlogComments_OwnerUserId",
                table: "BlogComments");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "tags");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "tags",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerUserName",
                table: "BlogComments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerUserId",
                table: "BlogComments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tags",
                table: "tags",
                column: "TagID");

            migrationBuilder.CreateIndex(
                name: "IX_BlogComments_BlogPostID",
                table: "BlogComments",
                column: "BlogPostID");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogComments_BlogComments_ParentCommentID",
                table: "BlogComments",
                column: "ParentCommentID",
                principalTable: "BlogComments",
                principalColumn: "BlogCommentID");
        }
    }
}
