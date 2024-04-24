using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dokana.Migrations
{
    public partial class AddProfileImageSrcToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImageSrc",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImageSrc",
                table: "AspNetUsers");
        }
    }
}
