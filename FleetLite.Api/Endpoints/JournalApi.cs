using FleetLite.Domaine;

namespace FleetLite.Api.Endpoints;

/// <summary>Messages de journalisation de l'API, générés à la compilation (CA1848, CA1873).</summary>
internal static partial class JournalApi
{
    [LoggerMessage(EventId = 1001, Level = LogLevel.Warning,
        Message = "TCO demandé pour un véhicule inconnu : {VehiculeId}")]
    public static partial void TcoVehiculeInconnu(this ILogger journal, Guid vehiculeId);

    [LoggerMessage(EventId = 1002, Level = LogLevel.Information,
        Message = "TCO calculé pour {VehiculeId} : total {Total}, {NombreAlertes} alerte(s)")]
    public static partial void TcoCalcule(
        this ILogger journal, Guid vehiculeId, decimal total, int nombreAlertes);

    [LoggerMessage(EventId = 2001, Level = LogLevel.Information,
        Message = "Coût {CoutId} ajouté au véhicule {VehiculeId} : {Nature} {Montant}")]
    public static partial void CoutAjoute(
        this ILogger journal, Guid coutId, Guid vehiculeId, NatureCout nature, decimal montant);
}