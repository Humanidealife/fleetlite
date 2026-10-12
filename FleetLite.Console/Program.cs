using System.Globalization;

using FleetLite.Domaine;

// Culture invariante : la sortie doit être identique sur toutes les machines.
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

var horloge = new HorlogeFigee(new DateOnly(2026, 9, 4));
var calculateur = new CalculTco(horloge);

var idVehicule = Guid.Parse("11111111-1111-1111-1111-111111111111");

var vehicule = new Vehicule(
    idVehicule, "EF-123-GH", "Peugeot", "308",
    new DateOnly(2024, 3, 15), Energie.Hybride);

var contrat = new Contrat(
    idVehicule, "Loueur Alpha",
    DateDebut: new DateOnly(2024, 4, 1),
    DureeMois: 48,
    LoyerMensuel: 420.00m,
    KilometrageContractuel: 120_000);

List<Cout> couts =
[
    new(idVehicule, new DateOnly(2026,  8, 12), NatureCout.Carburant,    3_220.45m),
    new(idVehicule, new DateOnly(2026,  5, 20), NatureCout.Entretien,      890.00m),
    new(idVehicule, new DateOnly(2025, 11,  3), NatureCout.Pneumatiques,   640.00m),
    new(idVehicule, new DateOnly(2026,  1, 15), NatureCout.Assurance,    1_150.00m),
];

SyntheseTco synthese = calculateur.Calculer(contrat, couts, kilometrageReleve: 78_500);

Console.WriteLine(vehicule.Designation);
Console.WriteLine($"Contrat {contrat.Loueur} : {contrat.DateDebut:dd/MM/yyyy} -> {contrat.DateFin:dd/MM/yyyy}");
Console.WriteLine($"Mois factures          : {synthese.MoisFactures}");
Console.WriteLine($"Loyers echus           : {synthese.LoyersEchus:N2}");
Console.WriteLine($"Couts engages          : {synthese.CoutsEngages:N2}");
Console.WriteLine($"TCO a date             : {synthese.Total:N2}");
Console.WriteLine($"Cout au kilometre      : {synthese.CoutParKilometre:F4}");
Console.WriteLine($"Km projete fin contrat : {synthese.KilometrageProjeteFinContrat:N0}");
Console.WriteLine($"Ecart kilometrique     : {synthese.EcartKilometrique:N0}");