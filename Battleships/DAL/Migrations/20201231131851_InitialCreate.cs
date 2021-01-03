using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameSaves",
                columns: table => new
                {
                    GameSaveId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Player_1 = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Player_2 = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    BoardSize = table.Column<int>(type: "int", nullable: false),
                    MoveLog = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipList1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipList2 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSaves", x => x.GameSaveId);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    SettingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Player_1 = table.Column<string>(type: "nvarchar(26)", maxLength: 26, nullable: false),
                    Player_2 = table.Column<string>(type: "nvarchar(26)", maxLength: 26, nullable: false),
                    BoardSize = table.Column<int>(type: "int", nullable: false),
                    Touch = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.SettingId);
                });

            migrationBuilder.CreateTable(
                name: "Boats",
                columns: table => new
                {
                    BoatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoatName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BoatLength = table.Column<int>(type: "int", nullable: false),
                    BoatCount = table.Column<int>(type: "int", nullable: false),
                    SettingId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boats", x => x.BoatId);
                    table.ForeignKey(
                        name: "FK_Boats_Settings_SettingId",
                        column: x => x.SettingId,
                        principalTable: "Settings",
                        principalColumn: "SettingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boats_SettingId",
                table: "Boats",
                column: "SettingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Boats");

            migrationBuilder.DropTable(
                name: "GameSaves");

            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
