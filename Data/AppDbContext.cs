using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TracAgriApi.Models;

namespace AgriTraceAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Produit → Variete (NO CASCADE)
            modelBuilder.Entity<Produit>()
                .HasOne(p => p.Variete)
                .WithMany()
                .HasForeignKey(p => p.VarieteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Produit → Parcelle (NO CASCADE)
            modelBuilder.Entity<Produit>()
                .HasOne(p => p.Parcelle)
                .WithMany()
                .HasForeignKey(p => p.ParcelleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Variete → Categorie (OK cascade simple)
            modelBuilder.Entity<Variete>()
                .HasOne(v => v.Categorie)
                .WithMany(c => c.Varietes)
                .HasForeignKey(v => v.CategorieId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<EtiquetteFerme>()
    .HasOne(e => e.Agriculteur)
    .WithMany()
    .HasForeignKey(e => e.AgriculteurId)
    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EtiquetteFerme>()
                .HasOne(e => e.Ferme)
                .WithMany()
                .HasForeignKey(e => e.FermeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EtiquetteFerme>()
                .HasOne(e => e.Parcelle)
                .WithMany()
                .HasForeignKey(e => e.ParcelleId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EtiquetteFerme>()
                .HasOne(e => e.Produit)
                .WithMany()
                .HasForeignKey(e => e.ProduitId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EtiquetteFerme>()
                .HasOne(e => e.Variete)
                .WithMany()
                .HasForeignKey(e => e.VarieteId)
                .OnDelete(DeleteBehavior.NoAction);


            // -------------RECEPTION - PALETTE----------------

            // Reception -> EtiquetteFerme
            modelBuilder.Entity<Reception>()
                .HasOne(r => r.EtiquetteFerme)
                .WithMany()
                .HasForeignKey(r => r.EtiquetteFermeId)
                .OnDelete(DeleteBehavior.NoAction);

            // Palette -> Produit
            modelBuilder.Entity<Palette>()
                .HasOne(p => p.Produit)
                .WithMany()
                .HasForeignKey(p => p.ProduitId)
                .OnDelete(DeleteBehavior.NoAction);

            // Palette -> Reception
            modelBuilder.Entity<Palette>()
                .HasOne(p => p.Reception)
                .WithOne(r => r.Palette)
                .HasForeignKey<Palette>(p => p.ReceptionId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Reception>()
    .Property(r => r.PoidsBrut)
    .HasPrecision(18, 2);

            modelBuilder.Entity<Reception>()
                .Property(r => r.Temperature)
                .HasPrecision(5, 2);


            modelBuilder.Entity<Palette>()
    .Property(p => p.PoidsBrut)
    .HasPrecision(18, 2);

            modelBuilder.Entity<Palette>()
                .Property(p => p.QuantiteDisponible)
                .HasPrecision(18, 2);



            modelBuilder.Entity<Stock>()
                .Property(s => s.QuantiteDisponible)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SortieStock>()
                .Property(s => s.QuantiteSortie)
                .HasPrecision(18, 2);


            //__________socite

            modelBuilder.Entity<Societe>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Nom).IsRequired().HasMaxLength(200);
                entity.Property(s => s.Email).HasMaxLength(200);
                entity.Property(s => s.Telephone).HasMaxLength(50);

                // Relations
                entity.HasMany(s => s.Utilisateurs)
                    .WithOne(u => u.Societe)
                    .HasForeignKey(u => u.SocieteId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
        // les entity sets (tables)
        public DbSet<Agriculteur> Agriculteurs { get; set; }
        public DbSet<Ferme> Fermes { get; set; }
        public DbSet<Parcelle> Parcelles { get; set; }
        public DbSet<Variete> Varietes { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Produit> Produites { get; set; }
        public DbSet<EtiquetteFerme> EtiquetteFermes { get; set; }
        public DbSet<Reception> Receptions { get; set; }
        public DbSet<Palette> Palettes { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<SortieStock> SortieStocks { get; set; }
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Societe> Societes { get; set; }
    }
}