using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealWorld.Migrations
{
    /// <inheritdoc />
    public partial class MakeSlugUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserUser_Users_FollowersId",
                table: "UserUser");

            migrationBuilder.DropForeignKey(
                name: "FK_UserUser_Users_FollowingId",
                table: "UserUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserUser",
                table: "UserUser");

            migrationBuilder.RenameTable(
                name: "UserUser",
                newName: "UserFollows");

            migrationBuilder.RenameIndex(
                name: "IX_UserUser_FollowingId",
                table: "UserFollows",
                newName: "IX_UserFollows_FollowingId");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Articles",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFollows",
                table: "UserFollows",
                columns: new[] { "FollowersId", "FollowingId" });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Slug",
                table: "Articles",
                column: "Slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollows_Users_FollowersId",
                table: "UserFollows",
                column: "FollowersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollows_Users_FollowingId",
                table: "UserFollows",
                column: "FollowingId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollows_Users_FollowersId",
                table: "UserFollows");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFollows_Users_FollowingId",
                table: "UserFollows");

            migrationBuilder.DropIndex(
                name: "IX_Articles_Slug",
                table: "Articles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFollows",
                table: "UserFollows");

            migrationBuilder.RenameTable(
                name: "UserFollows",
                newName: "UserUser");

            migrationBuilder.RenameIndex(
                name: "IX_UserFollows_FollowingId",
                table: "UserUser",
                newName: "IX_UserUser_FollowingId");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Articles",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserUser",
                table: "UserUser",
                columns: new[] { "FollowersId", "FollowingId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserUser_Users_FollowersId",
                table: "UserUser",
                column: "FollowersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserUser_Users_FollowingId",
                table: "UserUser",
                column: "FollowingId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
