using System;

namespace FleetLite.Api.Contrats;

public sealed record VehiculeDto(
    Guid Id,
    string Immatriculation,
    string Designation,
    string Marque,
    string Modele,
    string Motorisation,
    DateOnly DateMiseEnCirculation);

public sealed record AlerteDto(string Code, string Niveau, string Message);

public sealed record SyntheseTcoDto(
    Guid VehiculeId,
    string Immatriculation,
    string Designation,
    string Loueur,
    DateOnly DateDebutContrat,
    DateOnly DateFinContrat,
    int MoisFactures,
    decimal LoyersEchus,
    decimal CoutsEngages,
    decimal Total,
    decimal? CoutParKilometre,
    int KilometrageReleve,
    int KilometrageContractuel,
    int KilometrageProjeteFinContrat,
    int EcartKilometrique,
    IReadOnlyList<AlerteDto> Alertes);

/// <summary>Enveloppe de pagination commune à toutes les listes de l'API.</summary>
public sealed record PageDto<T>(int Total, int Page, int Taille, IReadOnlyList<T> Elements);