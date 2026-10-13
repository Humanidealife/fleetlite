using System;
using System.Collections.Generic;
using System.Text;

namespace FleetLite.Domaine;

public enum NiveauAlerte
{
    Information = 10,
    Vigilance = 20,
    Critique = 30
}

public sealed record Alerte(
    Guid VehiculeId,
    string Code,
    NiveauAlerte Niveau,
    string Message);
