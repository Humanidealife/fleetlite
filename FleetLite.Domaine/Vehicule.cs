namespace FleetLite.Domaine;

public sealed record Vehicule(
    Guid Id,
    string Immatriculation,
    string Marque,
    string Modele,
    DateOnly DateMiseEnCirculation,
    Energie Motorisation)
{
    public string Designation => $"{Marque} {Modele} ({Immatriculation})";
}