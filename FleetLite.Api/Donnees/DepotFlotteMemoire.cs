using FleetLite.Domaine;
using System;
using System.Collections.Generic;

namespace FleetLite.Api.Donnees;

/// <summary>Implémentation en mémoire, remplacée par SQL Server en séance 3.</summary>
public sealed class DepotFlotteMemoire : IDepotFlotte
{
    private static readonly Guid Id1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid Id2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid Id3 = Guid.Parse("33333333-3333-3333-3333-333333333333");

    private readonly object _verrou = new();

    private readonly List<Vehicule> _vehicules =
    [
        new(Id1, "EF-123-GH", "Peugeot", "308",     new DateOnly(2024, 3, 15), Energie.Hybride),
        new(Id2, "BX-884-KD", "Renault", "Clio",    new DateOnly(2022,10,  5), Energie.Essence),
        new(Id3, "CV-207-QM", "Ford",    "Transit", new DateOnly(2023, 9, 10), Energie.Diesel),
    ];

    private readonly List<Contrat> _contrats =
    [
        new(Id1, "Loueur Alpha", new DateOnly(2024,  4,  1), 48, 420.00m, 120_000),
        new(Id2, "Loueur Beta",  new DateOnly(2022, 10, 20), 48, 310.00m, 100_000),
        new(Id3, "Loueur Alpha", new DateOnly(2023,  9, 25), 36, 545.00m,  90_000),
    ];

    private readonly List<Cout> _couts =
    [
        new(Guid.NewGuid(), Id1, new DateOnly(2026,  8, 12), NatureCout.Carburant,    3_220.45m),
        new(Guid.NewGuid(), Id1, new DateOnly(2026,  5, 20), NatureCout.Entretien,      890.00m),
        new(Guid.NewGuid(), Id1, new DateOnly(2025, 11,  3), NatureCout.Pneumatiques,   640.00m),
        new(Guid.NewGuid(), Id1, new DateOnly(2026,  1, 15), NatureCout.Assurance,    1_150.00m),

        new(Guid.NewGuid(), Id2, new DateOnly(2026,  7,  9), NatureCout.Carburant,    2_480.30m),
        new(Guid.NewGuid(), Id2, new DateOnly(2026,  3, 22), NatureCout.Entretien,      705.00m),
        new(Guid.NewGuid(), Id2, new DateOnly(2025,  6, 14), NatureCout.Pneumatiques,   480.00m),
        new(Guid.NewGuid(), Id2, new DateOnly(2026,  2,  1), NatureCout.Assurance,      455.30m),

        new(Guid.NewGuid(), Id3, new DateOnly(2026,  8, 28), NatureCout.Carburant,    5_310.75m),
        new(Guid.NewGuid(), Id3, new DateOnly(2026,  4, 11), NatureCout.Entretien,    1_240.00m),
        new(Guid.NewGuid(), Id3, new DateOnly(2025, 12,  2), NatureCout.Pneumatiques,   620.00m),
        new(Guid.NewGuid(), Id3, new DateOnly(2026,  6, 30), NatureCout.Sinistre,       309.50m),
    ];

    private readonly Dictionary<Guid, int> _kilometrages = new()
    {
        [Id1] = 78_500,
        [Id2] = 88_000,
        [Id3] = 98_500,
    };

    public IReadOnlyList<Vehicule> ListerVehicules()
    {
        lock (_verrou) return _vehicules.ToList();
    }

    public DossierVehicule? TrouverDossier(Guid vehiculeId)
    {
        lock (_verrou)
        {
            Vehicule? vehicule = _vehicules.FirstOrDefault(v => v.Id == vehiculeId);
            if (vehicule is null) return null;

            Contrat? contrat = _contrats.FirstOrDefault(c => c.VehiculeId == vehiculeId);
            if (contrat is null) return null;

            return new DossierVehicule(
                vehicule,
                contrat,
                _couts.Where(c => c.VehiculeId == vehiculeId).ToList(),
                _kilometrages.GetValueOrDefault(vehiculeId));
        }
    }

    public IReadOnlyList<DossierVehicule> ListerDossiers()
    {
        lock (_verrou)   // le verrou de .NET est réentrant : l'appel imbriqué ci-dessous est sûr
        {
            var dossiers = new List<DossierVehicule>();
            foreach (Vehicule v in _vehicules)
            {
                DossierVehicule? dossier = TrouverDossier(v.Id);
                if (dossier is not null) dossiers.Add(dossier);
            }
            return dossiers;
        }
    }

    public Cout? TrouverCout(Guid vehiculeId, Guid coutId)
    {
        lock (_verrou)
            return _couts.FirstOrDefault(c => c.VehiculeId == vehiculeId && c.Id == coutId);
    }

    public void AjouterCout(Cout cout)
    {
        ArgumentNullException.ThrowIfNull(cout);
        lock (_verrou) _couts.Add(cout);
    }
}