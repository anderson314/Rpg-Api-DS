using Microsoft.EntityFrameworkCore.Migrations;

namespace RpgApi.Migrations
{
    public partial class MigracaoPersonagemHabilidade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Habilidades",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(nullable: true),
                    Dano = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habilidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonagemHabilidades",
                columns: table => new
                {
                    PersonagemId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonagemId1 = table.Column<int>(nullable: false),
                    HabilidadeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonagemHabilidades", x => x.PersonagemId);
                    table.ForeignKey(
                        name: "FK_PersonagemHabilidades_Habilidades_HabilidadeId",
                        column: x => x.HabilidadeId,
                        principalTable: "Habilidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonagemHabilidades_Personagens_PersonagemId1",
                        column: x => x.PersonagemId1,
                        principalTable: "Personagens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonagemHabilidades_HabilidadeId",
                table: "PersonagemHabilidades",
                column: "HabilidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonagemHabilidades_PersonagemId1",
                table: "PersonagemHabilidades",
                column: "PersonagemId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonagemHabilidades");

            migrationBuilder.DropTable(
                name: "Habilidades");
        }
    }
}
