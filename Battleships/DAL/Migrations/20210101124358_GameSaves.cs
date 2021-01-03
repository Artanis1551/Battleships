using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class GameSaves : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoveLog",
                table: "GameSaves");

            migrationBuilder.DropColumn(
                name: "ShipList1",
                table: "GameSaves");

            migrationBuilder.DropColumn(
                name: "ShipList2",
                table: "GameSaves");

            migrationBuilder.RenameColumn(
                name: "Player_2",
                table: "GameSaves",
                newName: "Player2");

            migrationBuilder.RenameColumn(
                name: "Player_1",
                table: "GameSaves",
                newName: "Player1");

            migrationBuilder.RenameColumn(
                name: "BoardSize",
                table: "GameSaves",
                newName: "Touch_");

            migrationBuilder.AddColumn<int>(
                name: "BoardSize_",
                table: "GameSaves",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MoveLog_",
                table: "GameSaves",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShipList1_",
                table: "GameSaves",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShipList2_",
                table: "GameSaves",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShipsReq_",
                table: "GameSaves",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoardSize_",
                table: "GameSaves");

            migrationBuilder.DropColumn(
                name: "MoveLog_",
                table: "GameSaves");

            migrationBuilder.DropColumn(
                name: "ShipList1_",
                table: "GameSaves");

            migrationBuilder.DropColumn(
                name: "ShipList2_",
                table: "GameSaves");

            migrationBuilder.DropColumn(
                name: "ShipsReq_",
                table: "GameSaves");

            migrationBuilder.RenameColumn(
                name: "Touch_",
                table: "GameSaves",
                newName: "BoardSize");

            migrationBuilder.RenameColumn(
                name: "Player2",
                table: "GameSaves",
                newName: "Player_2");

            migrationBuilder.RenameColumn(
                name: "Player1",
                table: "GameSaves",
                newName: "Player_1");

            migrationBuilder.AddColumn<string>(
                name: "MoveLog",
                table: "GameSaves",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipList1",
                table: "GameSaves",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipList2",
                table: "GameSaves",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
