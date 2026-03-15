using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnet_api_tutorial.Migrations
{
    /// <inheritdoc />
    public partial class Definekeysonfollowing_followerstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollows_Users_FollowersId",
                table: "UserFollows");

            migrationBuilder.RenameColumn(
                name: "FollowersId",
                table: "UserFollows",
                newName: "FollowerId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollows_Users_FollowerId",
                table: "UserFollows",
                column: "FollowerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollows_Users_FollowerId",
                table: "UserFollows");

            migrationBuilder.RenameColumn(
                name: "FollowerId",
                table: "UserFollows",
                newName: "FollowersId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollows_Users_FollowersId",
                table: "UserFollows",
                column: "FollowersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
