using FleetLite.Api.Contrats;
using FleetLite.Api.Donnees;
using FleetLite.Domaine;

using Microsoft.AspNetCore.Http.HttpResults;

namespace FleetLite.Api.Endpoints;

public static class VehiculesEndpoints
{
    public static RouteGroupBuilder MapVehicules(this RouteGroupBuilder groupe)
    {
        groupe.MapGet("/vehicules", ListerVehicules).WithName("ListerVehicules");
        groupe.MapGet("/vehicules/{id:guid}", LireVehicule).WithName("LireVehicule");
        groupe.MapGet("/vehicules/{id:guid}/tco", LireTco).WithName("LireTcoVehicule");
        return groupe;
    }

    /// <summary>Liste les véhicules du parc.</summary>
    internal static Ok<IReadOnlyList<VehiculeDto>> ListerVehicules(IDepotFlotte depot)
        => TypedResults.Ok<IReadOnlyList<VehiculeDto>>(
               depot.ListerVehicules().Select(v => v.VersDto()).ToList());

    /// <summary>Lit un véhicule par son identifiant.</summary>
    internal static Results<Ok<VehiculeDto>, NotFound> LireVehicule(Guid id, IDepotFlotte depot)
        => depot.TrouverDossier(id) is { } dossier
            ? TypedResults.Ok(dossier.Vehicule.VersDto())
            : TypedResults.NotFound();

    /// <summary>Calcule la synthèse de coût total de détention d'un véhicule, alertes comprises.</summary>
    internal static Results<Ok<SyntheseTcoDto>, NotFound> LireTco(
        Guid id,
        IDepotFlotte depot,
        CalculTco calcul,
        MoteurAlertes moteur,
        ILogger<Program> journal)
    {
        if (depot.TrouverDossier(id) is not { } dossier)
        {
            journal.TcoVehiculeInconnu(id);
            return TypedResults.NotFound();
        }

        SyntheseTco synthese = calcul.Calculer(dossier.Contrat, dossier.Couts, dossier.KilometrageReleve);
        IReadOnlyList<Alerte> alertes = moteur.Evaluer(dossier.Contrat, synthese);

        journal.TcoCalcule(id, synthese.Total, alertes.Count);

        return TypedResults.Ok(synthese.VersDto(dossier, alertes));
    }
}