using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dokana.Migrations
{
    public partial class SeedOwnerUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[AspNetUsers] ([Id], [RefreshToken], [RefreshTokenExpireDateTime], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FullName], [Gender], [ProfileImageSrc], [JoinDate]) VALUES (N'0bcfb0d8-68af-4b00-a52b-8b2d0e4dd0ac', N'VgvPoSSc8ZhGnNlK7GUri18KuQWpbE/5gKgHQnNVlWI=', N'2022-11-13 23:11:27', N'farouk', N'FAROUK', N'farouk@site.com', N'FAROUK@SITE.COM', 0, N'AQAAAAEAACcQAAAAEFt/0NUV1E7P5D665mUZ7h4Y8+THiViz5KNpBEnnkpTnV432P3dzX0VV83womGe1LA==', N'A3QMMGQ353EBXHTD3B5KEE3WXDEXJVW2', N'5e79eb32-3ad0-489e-9553-dca8ed7afeb9', NULL, 0, 0, NULL, 1, 0, N'farouk Abdelhamid', N'Male', N'/Uploads/Users/user.png', N'2022-11-13 18:11:26')"
            );

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
