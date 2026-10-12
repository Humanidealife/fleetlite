using System;
using System.Collections.Generic;
using System.Text;

namespace FleetLite.Domaine;

public sealed record Cout(
    Guid VehiculeId,
    DateOnly Date,
    NatureCout Nature,
    decimal Montant);
