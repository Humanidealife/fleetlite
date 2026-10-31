using FleetLite.Api.Contrats;
using FleetLite.Api.Donnees;
using FleetLite.Domaine;

using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;

namespace FleetLite.Api.Endpoints;

public static class CoutsEndpoints
{
    private const decimal MontantMaximal = 1_000_000m;

    // NonRenseignee est la valeur zéro de l'énumération :
    // elle sert à détecter une absence, jamais à être acceptée.
    private static readonly string[] NaturesAcceptees =
        Enum.GetNames<NatureCout>()
            .Where(nom => nom != nameof(NatureCout.NonRenseignee))
            .ToArray();

    public static RouteGroupBuilder MapCouts(this RouteGroupBuilder groupe)
    {
        groupe.MapPost("/vehicules/{id:guid}/couts", CreerCout).WithName("CreerCout");
        groupe.MapGet("/vehicules/{id:guid}/couts/{coutId:guid}", LireCout).WithName("LireCout");
        return groupe;
    }

    /// <summary>Enregistre un coût (carburant, entretien, pneumatiques…) sur un véhicule.</summary>
    internal static Results<Created<CoutDto>, NotFound, ValidationProblem> CreerCout(
        Guid id,
        CreationCoutDto corps,
        IDepotFlotte depot,
        IHorloge horloge,
        ILogger<Program> journal)
    {
        if (depot.TrouverDossier(id) is not { } dossier)
            return TypedResults.NotFound();

        var erreurs = new Dictionary<string, string[]>();

        // --- date ---------------------------------------------------------
        if (corps.Date is not { } date)
        {
            erreurs["date"] = ["La date est obligatoire."];
        }
        else if (date > horloge.Aujourdhui())
        {
            erreurs["date"] = ["La date ne peut pas être dans le futur."];
        }
        else if (date < dossier.Contrat.DateDebut)
        {
            erreurs["date"] =
                [$"La date est antérieure au début du contrat ({dossier.Contrat.DateDebut:yyyy-MM-dd})."];
        }

        // --- nature -------------------------------------------------------
        if (!Enum.TryParse(corps.Nature, ignoreCase: true, out NatureCout nature)
            || !Enum.IsDefined(nature)
            || nature == NatureCout.NonRenseignee)
        {
            erreurs["nature"] =
                [$"Nature inconnue. Valeurs acceptées : {string.Join(", ", NaturesAcceptees)}."];
        }

        // --- montant ------------------------------------------------------
        if (corps.Montant is not { } montant)
        {
            erreurs["montant"] = ["Le montant est obligatoire."];
        }
        else if (montant <= 0m)
        {
            erreurs["montant"] = ["Le montant doit être strictement positif."];
        }
        else if (montant > MontantMaximal)
        {
            erreurs["montant"] = [$"Le montant dépasse la limite autorisée ({MontantMaximal:N0})."];
        }

        if (erreurs.Count > 0)
            return TypedResults.ValidationProblem(erreurs);

        var cout = new Cout(Guid.NewGuid(), id, corps.Date!.Value, nature, corps.Montant!.Value);
        depot.AjouterCout(cout);

        journal.LogInformation(
            "Coût {CoutId} ajouté au véhicule {VehiculeId} : {Nature} {Montant}",
            cout.Id, id, cout.Nature, cout.Montant);

        return TypedResults.Created(
            $"/api/v1/vehicules/{id}/couts/{cout.Id}",
            cout.VersDto());
    }

    /// <summary>Lit un coût précis d'un véhicule.</summary>
    internal static Results<Ok<CoutDto>, NotFound> LireCout(Guid id, Guid coutId, IDepotFlotte depot)
        => depot.TrouverCout(id, coutId) is { } cout
            ? TypedResults.Ok(cout.VersDto())
            : TypedResults.NotFound();
}