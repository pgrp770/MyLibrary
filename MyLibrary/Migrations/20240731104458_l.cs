using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyLibrary.Migrations
{
    /// <inheritdoc />
    public partial class l : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shelf_Library_LibraryId",
                table: "Shelf");

            migrationBuilder.AlterColumn<int>(
                name: "LibraryId",
                table: "Shelf",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "Shelf",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "Shelf",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Book",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    ShelfId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Book", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Book_Shelf_ShelfId",
                        column: x => x.ShelfId,
                        principalTable: "Shelf",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookSet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    ShelfId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookSet_Shelf_ShelfId",
                        column: x => x.ShelfId,
                        principalTable: "Shelf",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Book_ShelfId",
                table: "Book",
                column: "ShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_BookSet_ShelfId",
                table: "BookSet",
                column: "ShelfId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shelf_Library_LibraryId",
                table: "Shelf",
                column: "LibraryId",
                principalTable: "Library",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shelf_Library_LibraryId",
                table: "Shelf");

            migrationBuilder.DropTable(
                name: "Book");

            migrationBuilder.DropTable(
                name: "BookSet");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Shelf");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Shelf");

            migrationBuilder.AlterColumn<int>(
                name: "LibraryId",
                table: "Shelf",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Shelf_Library_LibraryId",
                table: "Shelf",
                column: "LibraryId",
                principalTable: "Library",
                principalColumn: "Id");
        }
    }
}
