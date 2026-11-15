using FleetLite.Domaine;

namespace FleetLite.Api.Donnees.Entites;

public sealed class VehiculeEntite
{
    public Guid Id { get; set; }
    public string Immatriculation { get; set; } = string.Empty;
    public string Marque { get; set; } = string.Empty;
    public string Modele { get; set; } = string.Empty;
    public DateOnly DateMiseEnCirculation { get; set; }
    public Energie Motorisation { get; set; }
}

public sealed class ContratEntite
{
    public Guid VehiculeId { get; set; }
    public string Loueur { get; set; } = string.Empty;
    public DateOnly DateDebut { get; set; }
    public short DureeMois { get; set; }
    public decimal LoyerMensuel { get; set; }
    public int KilometrageContractuel { get; set; }
    public DateOnly DateFin { get; set; }   // calculée par le serveur
}

public sealed class CoutEntite
{
    public Guid Id { get; set; }
    public Guid VehiculeId { get; set; }
    public DateOnly DateCout { get; set; }
    public NatureCout Nature { get; set; }
    public decimal Montant { get; set; }
}

public sealed class ReleveKilometriqueEntite
{
    public Guid Id { get; set; }
    public Guid VehiculeId { get; set; }
    public DateOnly DateReleve { get; set; }
    public int Kilometrage { get; set; }
}