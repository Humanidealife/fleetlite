namespace FleetLite.Domaine;

public sealed record SyntheseTco(
    Guid VehiculeId,
    int MoisFactures,
    decimal LoyersEchus,
    decimal CoutsEngages,
    decimal Total,
    decimal? CoutParKilometre,
    int KilometrageProjeteFinContrat,
    int EcartKilometrique);