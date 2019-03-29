using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Repositories.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManageItem",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Isdelete = table.Column<bool>(nullable: false),
                    Creator = table.Column<string>(maxLength: 20, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManageItem", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Creator = table.Column<string>(maxLength: 20, nullable: false),
                    IDCard = table.Column<string>(type: "char(18)", nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: false),
                    BankCard = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Uname = table.Column<string>(maxLength: 100, nullable: false),
                    Password = table.Column<string>(maxLength: 100, nullable: false),
                    Phone = table.Column<string>(maxLength: 100, nullable: false),
                    UNickname = table.Column<string>(maxLength: 100, nullable: false),
                    Sub = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManageItem");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
