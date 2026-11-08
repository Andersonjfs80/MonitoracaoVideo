using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app_backend_monitoracao_video.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cameras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "varchar(150)", nullable: false),
                    UrlRtsp = table.Column<string>(type: "varchar(500)", nullable: false),
                    Usuario = table.Column<string>(type: "varchar(100)", nullable: false),
                    Senha = table.Column<string>(type: "varchar(255)", nullable: false),
                    Fabricante = table.Column<string>(type: "varchar(100)", nullable: false),
                    Ativa = table.Column<bool>(type: "bit", nullable: false),
                    IpEndereco = table.Column<string>(type: "varchar(50)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cameras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gravacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CameraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaminhoArquivo = table.Column<string>(type: "varchar(1000)", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                    TipoEvento = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gravacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gravacoes_Cameras_CameraId",
                        column: x => x.CameraId,
                        principalTable: "Cameras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReconhecimentosPlaca",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CameraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Placa = table.Column<string>(type: "varchar(20)", nullable: false),
                    Confianca = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CaminhoSnapshot = table.Column<string>(type: "varchar(1000)", nullable: false),
                    DataCaptura = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReconhecimentosPlaca", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReconhecimentosPlaca_Cameras_CameraId",
                        column: x => x.CameraId,
                        principalTable: "Cameras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gravacoes_CameraId",
                table: "Gravacoes",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_ReconhecimentosPlaca_CameraId",
                table: "ReconhecimentosPlaca",
                column: "CameraId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Gravacoes");

            migrationBuilder.DropTable(
                name: "ReconhecimentosPlaca");

            migrationBuilder.DropTable(
                name: "Cameras");
        }
    }
}
