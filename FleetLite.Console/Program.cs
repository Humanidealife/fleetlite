using System.Globalization;

using FleetLite.Domaine;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

var horloge = new HorlogeFigee(new DateOnly(2026, 9, 4));
var calculateur = new CalculTco(horloge);
var moteur = new MoteurAlertes(horloge);

var id1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
var id2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
var id3 = Guid.Parse("33333333-3333-3333-3333-333333333333");

Vehicule[] vehicules =
[
    new(id1, "EF-123-GH", "Peugeot", "308",     new DateOnly(2024, 3, 15), Energie.Hybride),
    new(id2, "BX-884-KD", "Renault", "Clio",    new DateOnly(2022,10,  5), Energie.Essence),
    new(id3, "CV-207-QM", "Ford",    "Transit", new DateOnly(2023, 9, 10), Energie.Diesel),
];

Contrat[] contrats =
[
    new(id1, "Loueur Alpha", new DateOnly(2024,  4,  1), 48, 420.00m, 120_000),
    new(id2, "Loueur Beta",  new DateOnly(2022, 10, 20), 48, 310.00m, 100_000),
    new(id3, "Loueur Alpha", new DateOnly(2023,  9, 25), 36, 545.00m,  90_000),
];

var kilometrages = new Dictionary<Guid, int>
{
    [id1] = 78_500,
    [id2] = 88_000,
    [id3] = 98_500,
};

List<Cout> couts =
[
    new(Guid.NewGuid(), id1, new DateOnly(2026,  8, 12), NatureCout.Carburant,    3_220.45m),
    new(Guid.NewGuid(), id1, new DateOnly(2026,  5, 20), NatureCout.Entretien,      890.00m),
    new(Guid.NewGuid(), id1, new DateOnly(2025,  11, 3), NatureCout.Pneumatiques,   640.00m),
    new(Guid.NewGuid(), id1, new DateOnly(2026,  1, 15), NatureCout.Assurance,    1_150.00m),

    new(Guid.NewGuid(), id2, new DateOnly(2026,  7,  9), NatureCout.Carburant,    2_480.30m),
    new(Guid.NewGuid(), id2, new DateOnly(2026,  3, 22), NatureCout.Entretien,      705.00m),
    new(Guid.NewGuid(), id2, new DateOnly(2025,  6, 14), NatureCout.Pneumatiques,   480.00m),
    new(Guid.NewGuid(), id2, new DateOnly(2026,  2,  1), NatureCout.Assurance,      455.30m),

    new(Guid.NewGuid(), id3, new DateOnly(2026,  8, 28), NatureCout.Carburant,    5_310.75m),
    new(Guid.NewGuid(), id3, new DateOnly(2026,  4, 11), NatureCout.Entretien,    1_240.00m),
    new(Guid.NewGuid(), id3, new DateOnly(2025, 12,  2), NatureCout.Pneumatiques,   620.00m),
    new(Guid.NewGuid(), id3, new DateOnly(2026,  6, 30), NatureCout.Sinistre,       309.50m),
];

int totalAlertes = 0;

foreach (var vehicule in vehicules)
{
    Contrat contrat = contrats.First(c => c.VehiculeId == vehicule.Id);
    int kmReleve = kilometrages[vehicule.Id];

    SyntheseTco synthese = calculateur.Calculer(contrat, couts, kmReleve);
    IReadOnlyList<Alerte> alertes = moteur.Evaluer(contrat, synthese);
    totalAlertes += alertes.Count;

    Console.WriteLine(new string('-', 64));
    Console.WriteLine($"{vehicule.Designation} | {contrat.Loueur}");
    Console.WriteLine($"  Contrat        : {contrat.DateDebut:dd/MM/yyyy} -> {contrat.DateFin:dd/MM/yyyy} ({contrat.DureeMois} mois)");
    Console.WriteLine($"  Mois factures  : {synthese.MoisFactures}");
    Console.WriteLine($"  Loyers echus   : {synthese.LoyersEchus:N2}");
    Console.WriteLine($"  Couts engages  : {synthese.CoutsEngages:N2}");
    Console.WriteLine($"  TCO a date     : {synthese.Total:N2}");
    Console.WriteLine($"  Cout au km     : {synthese.CoutParKilometre:F4}");
    Console.WriteLine($"  Km projete     : {synthese.KilometrageProjeteFinContrat:N0} (contractuel {contrat.KilometrageContractuel:N0})");

    if (alertes.Count == 0)
    {
        Console.WriteLine("  Alertes        : aucune");
    }
    else
    {
        foreach (Alerte alerte in alertes)
            Console.WriteLine($"  [{alerte.Niveau,-9}] {alerte.Code,-17} {alerte.Message}");
    }
}

Console.WriteLine(new string('-', 64));
Console.WriteLine($"Total alertes : {totalAlertes}");