using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaludsaActas.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accesorios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PurchaseCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Observation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accesorios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "acta_drafts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UsuarioJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EquiposJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MarcarFirmada = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acta_drafts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "activos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Hostname = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PurchaseCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Observation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "empleados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_empleados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "actas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SincronizadoSaludsa = table.Column<bool>(type: "bit", nullable: false),
                    EstadoSincronizacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TimestampSincronizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    TienePagare = table.Column<bool>(type: "bit", nullable: false),
                    ArchivoActa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ArchivoPagare = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_actas_empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "acta_accesorios",
                columns: table => new
                {
                    AccesoriosId = table.Column<int>(type: "int", nullable: false),
                    ActasId = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acta_accesorios", x => new { x.AccesoriosId, x.ActasId });
                    table.ForeignKey(
                        name: "FK_acta_accesorios_accesorios_AccesoriosId",
                        column: x => x.AccesoriosId,
                        principalTable: "accesorios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_acta_accesorios_actas_ActasId",
                        column: x => x.ActasId,
                        principalTable: "actas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "acta_activos",
                columns: table => new
                {
                    ActasId = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ActivosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acta_activos", x => new { x.ActasId, x.ActivosId });
                    table.ForeignKey(
                        name: "FK_acta_activos_actas_ActasId",
                        column: x => x.ActasId,
                        principalTable: "actas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_acta_activos_activos_ActivosId",
                        column: x => x.ActivosId,
                        principalTable: "activos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_acta_accesorios_ActasId",
                table: "acta_accesorios",
                column: "ActasId");

            migrationBuilder.CreateIndex(
                name: "IX_acta_activos_ActivosId",
                table: "acta_activos",
                column: "ActivosId");

            migrationBuilder.CreateIndex(
                name: "IX_actas_EmpleadoId",
                table: "actas",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_activos_Hostname",
                table: "activos",
                column: "Hostname");

            migrationBuilder.CreateIndex(
                name: "IX_activos_SerialNumber",
                table: "activos",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_empleados_NationalId",
                table: "empleados",
                column: "NationalId");

            migrationBuilder.CreateIndex(
                name: "IX_empleados_Username",
                table: "empleados",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "acta_accesorios");

            migrationBuilder.DropTable(
                name: "acta_activos");

            migrationBuilder.DropTable(
                name: "acta_drafts");

            migrationBuilder.DropTable(
                name: "accesorios");

            migrationBuilder.DropTable(
                name: "actas");

            migrationBuilder.DropTable(
                name: "activos");

            migrationBuilder.DropTable(
                name: "empleados");
        }
    }
}
