using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetLite.Api.Migrations
{
    /// <inheritdoc />
    public partial class SchemaInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vehicules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Immatriculation = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Marque = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Modele = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    DateMiseEnCirculation = table.Column<DateOnly>(type: "date", nullable: false),
                    Motorisation = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicules", x => x.Id);
                    table.CheckConstraint("CK_Vehicules_Motorisation", "[Motorisation] IN (10, 20, 30, 40, 50)");
                });

            migrationBuilder.CreateTable(
                name: "Contrats",
                columns: table => new
                {
                    VehiculeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Loueur = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    DateDebut = table.Column<DateOnly>(type: "date", nullable: false),
                    DureeMois = table.Column<short>(type: "smallint", nullable: false),
                    LoyerMensuel = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    KilometrageContractuel = table.Column<int>(type: "int", nullable: false),
                    DateFin = table.Column<DateOnly>(type: "date", nullable: false, computedColumnSql: "DATEADD(MONTH, [DureeMois], [DateDebut])", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contrats", x => x.VehiculeId);
                    table.CheckConstraint("CK_Contrats_DureeMois", "[DureeMois] BETWEEN 1 AND 120");
                    table.CheckConstraint("CK_Contrats_Loyer", "[LoyerMensuel] > 0");
                    table.ForeignKey(
                        name: "FK_Contrats_Vehicules_VehiculeId",
                        column: x => x.VehiculeId,
                        principalTable: "Vehicules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Couts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehiculeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateCout = table.Column<DateOnly>(type: "date", nullable: false),
                    Nature = table.Column<byte>(type: "tinyint", nullable: false),
                    Montant = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Couts", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.CheckConstraint("CK_Couts_Montant", "[Montant] > 0");
                    table.CheckConstraint("CK_Couts_Nature", "[Nature] IN (10, 20, 30, 40, 50)");
                    table.ForeignKey(
                        name: "FK_Couts_Vehicules_VehiculeId",
                        column: x => x.VehiculeId,
                        principalTable: "Vehicules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RelevesKilometriques",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehiculeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateReleve = table.Column<DateOnly>(type: "date", nullable: false),
                    Kilometrage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelevesKilometriques", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.CheckConstraint("CK_Releves_Kilometrage", "[Kilometrage] >= 0");
                    table.ForeignKey(
                        name: "FK_RelevesKilometriques_Vehicules_VehiculeId",
                        column: x => x.VehiculeId,
                        principalTable: "Vehicules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contrats_DateFin",
                table: "Contrats",
                column: "DateFin")
                .Annotation("SqlServer:Include", new[] { "Loueur", "KilometrageContractuel" });

            migrationBuilder.CreateIndex(
                name: "IX_Couts_VehiculeId_DateCout_Id",
                table: "Couts",
                columns: new[] { "VehiculeId", "DateCout", "Id" },
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_RelevesKilometriques_VehiculeId_DateReleve_Id",
                table: "RelevesKilometriques",
                columns: new[] { "VehiculeId", "DateReleve", "Id" },
                unique: true,
                descending: new[] { false, true, false })
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicules_Immatriculation",
                table: "Vehicules",
                column: "Immatriculation",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contrats");

            migrationBuilder.DropTable(
                name: "Couts");

            migrationBuilder.DropTable(
                name: "RelevesKilometriques");

            migrationBuilder.DropTable(
                name: "Vehicules");
        }
    }
}
