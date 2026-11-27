using FleetLite.Domaine;
using System;

namespace FleetLite.Api.Donnees;

/// <summary>Vue complète d'un véhicule : le véhicule, son contrat, ses coûts, son kilométrage.</summary>
public interface IDepotFlotte
{
    Task<IReadOnlyList<Vehicule>> ListerVehiculesAsync(CancellationToken ct);
    Task<IReadOnlyList<DossierVehicule>> ListerDossiersAsync(CancellationToken ct);
    Task<DossierVehicule?> TrouverDossierAsync(Guid vehiculeId, CancellationToken ct);
    Task<Cout?> TrouverCoutAsync(Guid vehiculeId, Guid coutId, CancellationToken ct);
    Task AjouterCoutAsync(Cout cout, CancellationToken ct);
}