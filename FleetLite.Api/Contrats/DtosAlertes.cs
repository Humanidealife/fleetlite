using System;

namespace FleetLite.Api.Contrats;

/// <summary>Une alerte du parc, avec le véhicule qu'elle concerne.</summary>
public sealed record AlerteVehiculeDto(
    Guid VehiculeId,
    string Immatriculation,
    string Designation,
    string Code,
    string Niveau,
    string Message);