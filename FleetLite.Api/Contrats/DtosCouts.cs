using System;

namespace FleetLite.Api.Contrats;

/// <summary>Corps attendu pour la création d'un coût.</summary>
public sealed record CreationCoutDto
{
    public DateOnly? Date { get; init; }
    public string? Nature { get; init; }
    public decimal? Montant { get; init; }
}

public sealed record CoutDto(
    Guid Id,
    Guid VehiculeId,
    DateOnly Date,
    string Nature,
    decimal Montant);