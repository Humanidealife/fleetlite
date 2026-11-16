using FleetLite.Api.Donnees.Entites;

using Microsoft.EntityFrameworkCore;

namespace FleetLite.Api.Donnees;

public sealed class FleetLiteContext(DbContextOptions<FleetLiteContext> options) : DbContext(options)
{
    public DbSet<VehiculeEntite> Vehicules => Set<VehiculeEntite>();
    public DbSet<ContratEntite> Contrats => Set<ContratEntite>();
    public DbSet<CoutEntite> Couts => Set<CoutEntite>();
    public DbSet<ReleveKilometriqueEntite> Releves => Set<ReleveKilometriqueEntite>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VehiculeEntite>(e =>
        {
            e.ToTable("Vehicules", t => t.HasCheckConstraint(
                "CK_Vehicules_Motorisation", "[Motorisation] IN (10, 20, 30, 40, 50)"));
            e.HasKey(v => v.Id);
            e.Property(v => v.Immatriculation).HasMaxLength(15).IsRequired();
            e.Property(v => v.Marque).HasMaxLength(60).IsRequired();
            e.Property(v => v.Modele).HasMaxLength(60).IsRequired();
            e.Property(v => v.Motorisation).HasConversion<byte>();
            e.HasIndex(v => v.Immatriculation).IsUnique();
        });

        modelBuilder.Entity<ContratEntite>(e =>
        {
            e.ToTable("Contrats", t =>
            {
                t.HasCheckConstraint("CK_Contrats_DureeMois", "[DureeMois] BETWEEN 1 AND 120");
                t.HasCheckConstraint("CK_Contrats_Loyer", "[LoyerMensuel] > 0");
            });
            e.HasKey(c => c.VehiculeId);
            e.Property(c => c.Loueur).HasMaxLength(80).IsRequired();
            e.Property(c => c.LoyerMensuel).HasPrecision(10, 2);
            e.Property(c => c.DateFin)
             .HasComputedColumnSql("DATEADD(MONTH, [DureeMois], [DateDebut])", stored: true);
            // Index couvrant de la requête d'échéances : le INCLUDE du schéma de référence.
            e.HasIndex(c => c.DateFin)
             .HasDatabaseName("IX_Contrats_DateFin")
             .IncludeProperties(c => new { c.Loueur, c.KilometrageContractuel });
            e.HasOne<VehiculeEntite>()
             .WithOne()
             .HasForeignKey<ContratEntite>(c => c.VehiculeId)
             .OnDelete(DeleteBehavior.Restrict);   // pas de suppression en cascade
        });

        modelBuilder.Entity<CoutEntite>(e =>
        {
            e.ToTable("Couts", t =>
            {
                t.HasCheckConstraint("CK_Couts_Montant", "[Montant] > 0");
                t.HasCheckConstraint("CK_Couts_Nature", "[Nature] IN (10, 20, 30, 40, 50)");
            });
            e.HasKey(c => c.Id).IsClustered(false);
            e.Property(c => c.Nature).HasConversion<byte>();
            e.Property(c => c.Montant).HasPrecision(12, 2);
            e.HasIndex(c => new { c.VehiculeId, c.DateCout, c.Id }).IsClustered().IsUnique();
            e.HasOne<VehiculeEntite>().WithMany().HasForeignKey(c => c.VehiculeId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ReleveKilometriqueEntite>(e =>
        {
            e.ToTable("RelevesKilometriques", t => t.HasCheckConstraint(
                "CK_Releves_Kilometrage", "[Kilometrage] >= 0"));
            e.HasKey(r => r.Id).IsClustered(false);
            e.HasIndex(r => new { r.VehiculeId, r.DateReleve, r.Id })
             .IsClustered().IsUnique()
             .IsDescending(false, true, false);
            e.HasOne<VehiculeEntite>().WithMany().HasForeignKey(r => r.VehiculeId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}