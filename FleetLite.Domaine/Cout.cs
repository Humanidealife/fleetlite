namespace FleetLite.Domaine;

public sealed record Cout(
    Guid Id,
    Guid VehiculeId,
    DateOnly Date,
    NatureCout Nature,
    decimal Montant);