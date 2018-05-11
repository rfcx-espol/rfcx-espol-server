using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alertas",
                columns: table => new
                {
                    AlertaId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Algoritmo = table.Column<string>(nullable: true),
                    AudioId = table.Column<int>(nullable: false),
                    Clasificado = table.Column<bool>(nullable: false),
                    Estado = table.Column<bool>(nullable: false),
                    Tipo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alertas", x => x.AlertaId);
                });

            migrationBuilder.CreateTable(
                name: "AlertasConfiguracion",
                columns: table => new
                {
                    AlertaConfiguracionId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Correo = table.Column<string>(nullable: true),
                    Frecuencia = table.Column<string>(nullable: true),
                    Repeticiones = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertasConfiguracion", x => x.AlertaConfiguracionId);
                });

            migrationBuilder.CreateTable(
                name: "Audios",
                columns: table => new
                {
                    AudioId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BitRate = table.Column<int>(nullable: false),
                    DispositivoId = table.Column<int>(nullable: false),
                    Duracion = table.Column<string>(nullable: true),
                    FechaGrabacion = table.Column<string>(nullable: true),
                    FechaLlegada = table.Column<string>(nullable: true),
                    Formato = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audios", x => x.AudioId);
                });

            migrationBuilder.CreateTable(
                name: "Dispositivos",
                columns: table => new
                {
                    DispositivoId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Latitud = table.Column<string>(nullable: true),
                    Longitud = table.Column<string>(nullable: true),
                    VersionAndroid = table.Column<string>(nullable: true),
                    VersionServicios = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispositivos", x => x.DispositivoId);
                });

            migrationBuilder.CreateTable(
                name: "Sensores",
                columns: table => new
                {
                    SensorId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Funcion = table.Column<string>(nullable: true),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensores", x => x.SensorId);
                });

            migrationBuilder.CreateTable(
                name: "Etiqueta",
                columns: table => new
                {
                    EtiquetaId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AudioId = table.Column<int>(nullable: false),
                    Descripcion = table.Column<string>(nullable: true),
                    TiempoFin = table.Column<string>(nullable: true),
                    TiempoInicio = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etiqueta", x => x.EtiquetaId);
                    table.ForeignKey(
                        name: "FK_Etiqueta_Audios_AudioId",
                        column: x => x.AudioId,
                        principalTable: "Audios",
                        principalColumn: "AudioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Etiqueta_AudioId",
                table: "Etiqueta",
                column: "AudioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alertas");

            migrationBuilder.DropTable(
                name: "AlertasConfiguracion");

            migrationBuilder.DropTable(
                name: "Dispositivos");

            migrationBuilder.DropTable(
                name: "Etiqueta");

            migrationBuilder.DropTable(
                name: "Sensores");

            migrationBuilder.DropTable(
                name: "Audios");
        }
    }
}
