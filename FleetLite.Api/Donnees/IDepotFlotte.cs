using FleetLite.Domaine;
using System;

namespace FleetLite.Api.Donnees;

/// <summary>Vue complète d'un véhicule : le véhicule, son contrat, ses coûts, son kilométrage.</summary>
public sealed record DossierVehicule(
    Vehicule Vehicule,
    Contrat Contrat,
    IReadOnlyList<Cout> Couts,
    int KilometrageReleve);

public interface IDepotFlotte
{
    IReadOnlyList<Vehicule> ListerVehicules();
    IReadOnlyList<DossierVehicule> ListerDossiers();
    DossierVehicule? TrouverDossier(Guid vehiculeId);
    Cout? TrouverCout(Guid vehiculeId, Guid coutId);
    void AjouterCout(Cout cout);
}