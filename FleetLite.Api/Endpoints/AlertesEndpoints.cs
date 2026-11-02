using FleetLite.Api.Contrats;
using FleetLite.Api.Donnees;
using FleetLite.Domaine;

using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;

namespace FleetLite.Api.Endpoints;

public static class AlertesEndpoints
{
    private const int TailleParDefaut = 20;
    private const int TailleMaximale = 100;

    // Même règle que pour la nature d'un coût : la valeur zéro n'est jamais acceptée.
    private static readonly string[] NiveauxAcceptes =
        Enum.GetNames<NiveauAlerte>()
            .Where(nom => nom != nameof(NiveauAlerte.NonRenseigne))
            .ToArray();

    public static RouteGroupBuilder MapAlertes(this RouteGroupBuilder groupe)
    {
        groupe.MapGet("/alertes", ListerAlertes).WithName("ListerAlertes");
        return groupe;
    }

    /// <summary>Liste les alertes de tout le parc, les plus graves d'abord.</summary>
    internal static Results<Ok<PageDto<AlerteVehiculeDto>>, ValidationProblem> ListerAlertes(
        IDepotFlotte depot,
        CalculTco calcul,
        MoteurAlertes moteur,
        string? niveau = null,
        int page = 1,
        int taille = TailleParDefaut)
    {
        var erreurs = new Dictionary<string, string[]>();

        NiveauAlerte? filtre = null;
        if (!string.IsNullOrWhiteSpace(niveau))
        {
            if (Enum.TryParse(niveau, ignoreCase: true, out NiveauAlerte valeur)
                && Enum.IsDefined(valeur)
                && valeur != NiveauAlerte.NonRenseigne)
                filtre = valeur;
            else
                erreurs["niveau"] =
                    [$"Niveau inconnu. Valeurs acceptées : {string.Join(", ", NiveauxAcceptes)}."];
        }

        if (page < 1)
            erreurs["page"] = ["Le numéro de page commence à 1."];

        if (taille is < 1 or > TailleMaximale)
            erreurs["taille"] = [$"La taille de page doit être comprise entre 1 et {TailleMaximale}."];

        if (erreurs.Count > 0)
            return TypedResults.ValidationProblem(erreurs);

        // ToList() : la séquence sert deux fois (Count, puis Skip/Take).
        var lignes = depot.ListerDossiers()
            .SelectMany(dossier =>
            {
                SyntheseTco synthese =
                    calcul.Calculer(dossier.Contrat, dossier.Couts, dossier.KilometrageReleve);

                return moteur.Evaluer(dossier.Contrat, synthese)
                             .Select(alerte => (Dossier: dossier, Alerte: alerte));
            })
            .Where(x => filtre is null || x.Alerte.Niveau == filtre.Value)
            .OrderByDescending(x => x.Alerte.Niveau)                                  // Critique d'abord
            .ThenBy(x => x.Dossier.Vehicule.Immatriculation, StringComparer.Ordinal)
            .ThenBy(x => x.Alerte.Code, StringComparer.Ordinal)
            .ToList();

        var elements = lignes
            .Skip((page - 1) * taille)
            .Take(taille)
            .Select(x => new AlerteVehiculeDto(
                x.Dossier.Vehicule.Id,
                x.Dossier.Vehicule.Immatriculation,
                x.Dossier.Vehicule.Designation,
                x.Alerte.Code,
                x.Alerte.Niveau.ToString(),
                x.Alerte.Message))
            .ToList();

        return TypedResults.Ok(
            new PageDto<AlerteVehiculeDto>(lignes.Count, page, taille, elements));
    }
}