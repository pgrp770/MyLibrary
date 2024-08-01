using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyLibrary.Migrations
{
    /// <inheritdoc />
    public partial class library : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Library_Genre_GenreId",
                table: "Library");

            migrationBuilder.AlterColumn<int>(
                name: "GenreId",
                table: "Library",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Library",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Shelf",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LibraryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shelf", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shelf_Library_LibraryId",
                        column: x => x.LibraryId,
                        principalTable: "Library",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shelf_LibraryId",
                table: "Shelf",
                column: "LibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Library_Genre_GenreId",
                table: "Library",
                column: "GenreId",
                principalTable: "Genre",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Library_Genre_GenreId",
                table: "Library");

            migrationBuilder.DropTable(
                name: "Shelf");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Library");

            migrationBuilder.AlterColumn<int>(
                name: "GenreId",
                table: "Library",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Library_Genre_GenreId",
                table: "Library",
                column: "GenreId",
                principalTable: "Genre",
                principalColumn: "Id");
        }
    }
}
