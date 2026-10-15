using System;
using System.Collections.Generic;
using System.Text;

namespace FleetLite.Domaine;

/// <summary>Contrat de location longue durée (LLD) rattaché à un véhicule.</summary>
public sealed record Contrat(
    Guid VehiculeId,
    string Loueur,
    DateOnly DateDebut,
    int DureeMois,
    decimal LoyerMensuel,
    int KilometrageContractuel)
{
    public DateOnly DateFin => DateDebut.AddMonths(DureeMois);
}