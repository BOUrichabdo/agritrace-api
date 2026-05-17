using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TracAgriApi.Migrations
{
    /// <inheritdoc />
    public partial class InitCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Agriculteurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adresse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SocieteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agriculteurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Intitule = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SocieteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Societes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomCommercial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatriculeFiscal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ICE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adresse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ville = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Plan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Devise = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SocieteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Societes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fermes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomFerme = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgriculteurId = table.Column<int>(type: "int", nullable: false),
                    SocieteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fermes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fermes_Agriculteurs_AgriculteurId",
                        column: x => x.AgriculteurId,
                        principalTable: "Agriculteurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Varietes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Intitule = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategorieId = table.Column<int>(type: "int", nullable: false),
                    SocieteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Varietes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Varietes_Categories_CategorieId",
                        column: x => x.CategorieId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotDePasse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SocieteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Utilisateurs_Societes_SocieteId",
                        column: x => x.SocieteId,
                        principalTable: "Societes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parcelles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomParcelle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FermeId = table.Column<int>(type: "int", nullable: false),
                    SocieteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parcelles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parcelles_Fermes_FermeId",
                        column: x => x.FermeId,
                        principalTable: "Fermes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Produites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParcelleId = table.Column<int>(type: "int", nullable: false),
                    VarieteId = table.Column<int>(type: "int", nullable: false),
                    SocieteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produites_Parcelles_ParcelleId",
                        column: x => x.ParcelleId,
                        principalTable: "Parcelles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Produites_Varietes_VarieteId",
                        column: x => x.VarieteId,
                        principalTable: "Varietes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EtiquetteFermes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeEtiquette = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgriculteurId = table.Column<int>(type: "int", nullable: false),
                    FermeId = table.Column<int>(type: "int", nullable: false),
                    ParcelleId = table.Column<int>(type: "int", nullable: false),
                    ProduitId = table.Column<int>(type: "int", nullable: false),
                    VarieteId = table.Column<int>(type: "int", nullable: false),
                    DateGeneration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Receptionne = table.Column<bool>(type: "bit", nullable: false),
                    DateReception = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SocieteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EtiquetteFermes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EtiquetteFermes_Agriculteurs_AgriculteurId",
                        column: x => x.AgriculteurId,
                        principalTable: "Agriculteurs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EtiquetteFermes_Fermes_FermeId",
                        column: x => x.FermeId,
                        principalTable: "Fermes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EtiquetteFermes_Parcelles_ParcelleId",
                        column: x => x.ParcelleId,
                        principalTable: "Parcelles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EtiquetteFermes_Produites_ProduitId",
                        column: x => x.ProduitId,
                        principalTable: "Produites",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EtiquetteFermes_Varietes_VarieteId",
                        column: x => x.VarieteId,
                        principalTable: "Varietes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Receptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EtiquetteFermeId = table.Column<int>(type: "int", nullable: false),
                    PoidsBrut = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Temperature = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    EtatProduit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeProduit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaletteId = table.Column<int>(type: "int", nullable: true),
                    DateReception = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SocieteId = table.Column<int>(type: "int", nullable: false),
                    Utilisateur = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Receptions_EtiquetteFermes_EtiquetteFermeId",
                        column: x => x.EtiquetteFermeId,
                        principalTable: "EtiquetteFermes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Palettes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodePalette = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProduitId = table.Column<int>(type: "int", nullable: false),
                    PoidsBrut = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    QuantiteDisponible = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EtatPalette = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatutStock = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Emplacement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceptionId = table.Column<int>(type: "int", nullable: false),
                    SocieteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Palettes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Palettes_Produites_ProduitId",
                        column: x => x.ProduitId,
                        principalTable: "Produites",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Palettes_Receptions_ReceptionId",
                        column: x => x.ReceptionId,
                        principalTable: "Receptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SortieStocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceptionId = table.Column<int>(type: "int", nullable: false),
                    QuantiteSortie = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DateSortie = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Utilisateur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SocieteId = table.Column<int>(type: "int", nullable: false),
                    Observation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SortieStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SortieStocks_Receptions_ReceptionId",
                        column: x => x.ReceptionId,
                        principalTable: "Receptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceptionId = table.Column<int>(type: "int", nullable: false),
                    ProduitId = table.Column<int>(type: "int", nullable: false),
                    VarieteId = table.Column<int>(type: "int", nullable: false),
                    QuantiteDisponible = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DateEntree = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Emplacement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SocieteId = table.Column<int>(type: "int", nullable: false),
                    EtatStock = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stocks_Produites_ProduitId",
                        column: x => x.ProduitId,
                        principalTable: "Produites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stocks_Receptions_ReceptionId",
                        column: x => x.ReceptionId,
                        principalTable: "Receptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stocks_Varietes_VarieteId",
                        column: x => x.VarieteId,
                        principalTable: "Varietes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EtiquetteFermes_AgriculteurId",
                table: "EtiquetteFermes",
                column: "AgriculteurId");

            migrationBuilder.CreateIndex(
                name: "IX_EtiquetteFermes_FermeId",
                table: "EtiquetteFermes",
                column: "FermeId");

            migrationBuilder.CreateIndex(
                name: "IX_EtiquetteFermes_ParcelleId",
                table: "EtiquetteFermes",
                column: "ParcelleId");

            migrationBuilder.CreateIndex(
                name: "IX_EtiquetteFermes_ProduitId",
                table: "EtiquetteFermes",
                column: "ProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_EtiquetteFermes_VarieteId",
                table: "EtiquetteFermes",
                column: "VarieteId");

            migrationBuilder.CreateIndex(
                name: "IX_Fermes_AgriculteurId",
                table: "Fermes",
                column: "AgriculteurId");

            migrationBuilder.CreateIndex(
                name: "IX_Palettes_ProduitId",
                table: "Palettes",
                column: "ProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_Palettes_ReceptionId",
                table: "Palettes",
                column: "ReceptionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parcelles_FermeId",
                table: "Parcelles",
                column: "FermeId");

            migrationBuilder.CreateIndex(
                name: "IX_Produites_ParcelleId",
                table: "Produites",
                column: "ParcelleId");

            migrationBuilder.CreateIndex(
                name: "IX_Produites_VarieteId",
                table: "Produites",
                column: "VarieteId");

            migrationBuilder.CreateIndex(
                name: "IX_Receptions_EtiquetteFermeId",
                table: "Receptions",
                column: "EtiquetteFermeId");

            migrationBuilder.CreateIndex(
                name: "IX_SortieStocks_ReceptionId",
                table: "SortieStocks",
                column: "ReceptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ProduitId",
                table: "Stocks",
                column: "ProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ReceptionId",
                table: "Stocks",
                column: "ReceptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_VarieteId",
                table: "Stocks",
                column: "VarieteId");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_SocieteId",
                table: "Utilisateurs",
                column: "SocieteId");

            migrationBuilder.CreateIndex(
                name: "IX_Varietes_CategorieId",
                table: "Varietes",
                column: "CategorieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Palettes");

            migrationBuilder.DropTable(
                name: "SortieStocks");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "Utilisateurs");

            migrationBuilder.DropTable(
                name: "Receptions");

            migrationBuilder.DropTable(
                name: "Societes");

            migrationBuilder.DropTable(
                name: "EtiquetteFermes");

            migrationBuilder.DropTable(
                name: "Produites");

            migrationBuilder.DropTable(
                name: "Parcelles");

            migrationBuilder.DropTable(
                name: "Varietes");

            migrationBuilder.DropTable(
                name: "Fermes");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Agriculteurs");
        }
    }
}
