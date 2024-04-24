using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dokana.Migrations
{
    public partial class AddOwnerUserToOwnersRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"
                                    INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0bcfb0d8-68af-4b00-a52b-8b2d0e4dd0ac', N'79dfc762-d9ec-497e-833c-b7ed6caa4fe4')
                                ");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
