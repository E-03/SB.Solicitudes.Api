using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SB.Solicitudes.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEntidadesGubernamentales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntidadesGubernamentales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PoderDelEstado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Sector = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntidadesGubernamentales", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntidadesGubernamentales_Nombre",
                table: "EntidadesGubernamentales",
                column: "Nombre");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntidadesGubernamentales");
        }
    }
}
