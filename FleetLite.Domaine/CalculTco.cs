namespace FleetLite.Domaine;

public sealed class CalculTco(IHorloge horloge)
{
    public SyntheseTco Calculer(Contrat contrat, IEnumerable<Cout> couts, int kilometrageReleve)
    {
        ArgumentNullException.ThrowIfNull(contrat);
        ArgumentNullException.ThrowIfNull(couts);
        ArgumentOutOfRangeException.ThrowIfNegative(kilometrageReleve);

        DateOnly aujourdhui = horloge.Aujourdhui();

        int moisFactures = Math.Clamp(
            MoisEcoules(contrat.DateDebut, aujourdhui), 0, contrat.DureeMois);

        decimal loyersEchus = moisFactures * contrat.LoyerMensuel;

        decimal coutsEngages = couts
            .Where(c => c.VehiculeId == contrat.VehiculeId)
            .Sum(c => c.Montant);

        decimal total = loyersEchus + coutsEngages;

        decimal? coutParKilometre = kilometrageReleve > 0
            ? total / kilometrageReleve
            : null;

        int kilometrageProjete = moisFactures > 0
            ? (int)Math.Round((double)kilometrageReleve / moisFactures * contrat.DureeMois)
            : 0;

        return new SyntheseTco(
            contrat.VehiculeId,
            moisFactures,
            loyersEchus,
            coutsEngages,
            total,
            coutParKilometre,
            kilometrageProjete,
            kilometrageProjete - contrat.KilometrageContractuel);
    }

    /// <summary>Nombre de mois pleins écoulés entre deux dates.</summary>
    internal static int MoisEcoules(DateOnly debut, DateOnly reference)
    {
        if (reference < debut) return 0;

        int mois = (reference.Year - debut.Year) * 12 + reference.Month - debut.Month;
        if (reference.Day < debut.Day) mois--;

        return Math.Max(mois, 0);
    }
}