using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class category : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "uObjectCategoryUUID",
                table: "t_wiki_item",
                newName: "uCategoryUUID");

            migrationBuilder.CreateTable(
                name: "t_wiki_category",
                columns: table => new
                {
                    uObjectUUID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    uOwnerUUID = table.Column<int>(nullable: false),
                    strImage = table.Column<string>(nullable: true),
                    strTitle = table.Column<string>(nullable: true),
                    nFlag = table.Column<int>(nullable: false),
                    nDelFlag = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_wiki_category", x => x.uObjectUUID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_wiki_category");

            migrationBuilder.RenameColumn(
                name: "uCategoryUUID",
                table: "t_wiki_item",
                newName: "uObjectCategoryUUID");
        }
    }
}
