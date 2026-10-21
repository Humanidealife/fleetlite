using FleetLite.Api.Donnees;
using FleetLite.Domaine;
using System;

namespace FleetLite.Api.Contrats;

public static class Correspondances
{
    public static VehiculeDto VersDto(this Vehicule v) => new(
        v.Id,
        v.Immatriculation,
        v.Designation,
        v.Marque,
        v.Modele,
        v.Motorisation.ToString(),          // "Hybride", jamais 30
        v.DateMiseEnCirculation);

    public static AlerteDto VersDto(this Alerte a) =>
        new(a.Code, a.Niveau.ToString(), a.Message);

    public static SyntheseTcoDto VersDto(
        this SyntheseTco s,
        DossierVehicule dossier,
        IReadOnlyList<Alerte> alertes) => new(
            s.VehiculeId,
            dossier.Vehicule.Immatriculation,
            dossier.Vehicule.Designation,
            dossier.Contrat.Loueur,
            dossier.Contrat.DateDebut,
            dossier.Contrat.DateFin,
            s.MoisFactures,
            s.LoyersEchus,
            s.CoutsEngages,
            s.Total,
            // un contrat public ne renvoie pas 28 décimales : on arrondit ici, une fois
            s.CoutParKilometre is { } c ? Math.Round(c, 4, MidpointRounding.AwayFromZero) : null,
            dossier.KilometrageReleve,
            dossier.Contrat.KilometrageContractuel,
            s.KilometrageProjeteFinContrat,
            s.EcartKilometrique,
            alertes.Select(a => a.VersDto()).ToList());
}