using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class BosqueProtector : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Etiqueta_Audios_AudioId",
                table: "Etiqueta");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Etiqueta",
                table: "Etiqueta");

            migrationBuilder.RenameTable(
                name: "Etiqueta",
                newName: "Etiquetas");

            migrationBuilder.RenameIndex(
                name: "IX_Etiqueta_AudioId",
                table: "Etiquetas",
                newName: "IX_Etiquetas_AudioId");

            migrationBuilder.AlterColumn<int>(
                name: "SensorId",
                table: "Sensores",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "EtiquetaId",
                table: "Etiquetas",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "DispositivoId",
                table: "Dispositivos",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "AudioId",
                table: "Audios",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "AlertaConfiguracionId",
                table: "AlertasConfiguracion",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "AlertaId",
                table: "Alertas",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Etiquetas",
                table: "Etiquetas",
                column: "EtiquetaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Etiquetas_Audios_AudioId",
                table: "Etiquetas",
                column: "AudioId",
                principalTable: "Audios",
                principalColumn: "AudioId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Etiquetas_Audios_AudioId",
                table: "Etiquetas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Etiquetas",
                table: "Etiquetas");

            migrationBuilder.RenameTable(
                name: "Etiquetas",
                newName: "Etiqueta");

            migrationBuilder.RenameIndex(
                name: "IX_Etiquetas_AudioId",
                table: "Etiqueta",
                newName: "IX_Etiqueta_AudioId");

            migrationBuilder.AlterColumn<int>(
                name: "SensorId",
                table: "Sensores",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "EtiquetaId",
                table: "Etiqueta",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "DispositivoId",
                table: "Dispositivos",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "AudioId",
                table: "Audios",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "AlertaConfiguracionId",
                table: "AlertasConfiguracion",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "AlertaId",
                table: "Alertas",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Etiqueta",
                table: "Etiqueta",
                column: "EtiquetaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Etiqueta_Audios_AudioId",
                table: "Etiqueta",
                column: "AudioId",
                principalTable: "Audios",
                principalColumn: "AudioId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
