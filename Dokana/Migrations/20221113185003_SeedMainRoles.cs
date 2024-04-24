using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dokana.Migrations
{
    public partial class SeedMainRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql
                (@"
                    INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'5fcfd9ec-2c1c-4508-ab57-5e8f528f922e', N'Sellers', N'SELLERS', N'4d452a51-bb96-40cd-a304-f2b5821a9e63')
                    INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'7066480a-2fc6-4d72-b4be-4ecb7845df56', N'ShippingStaff', N'SHIPPINGSTAFF', N'78686553-ada6-4a52-b626-10ea24940664')
                    INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'79dfc762-d9ec-497e-833c-b7ed6caa4fe4', N'Owners', N'OWNERS', N'25c57c46-d003-47b7-a89d-1f5959e6435f')
                    INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'bf03969c-b51b-4c00-b821-0b22b251f5d6', N'Admins', N'ADMINS', N'a4850d99-c94d-4998-9405-ba6733dc714a')
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
