namespace FleetLite.Domaine;

public sealed class MoteurAlertes(IHorloge horloge)
{
    public const int SeuilEcheanceVigilanceJours = 90;
    public const int SeuilEcheanceCritiqueJours = 30;
    public const decimal SeuilDepassementCritique = 0.10m;

    public IReadOnlyList<Alerte> Evaluer(Contrat contrat, SyntheseTco synthese)
    {
        ArgumentNullException.ThrowIfNull(contrat);
        ArgumentNullException.ThrowIfNull(synthese);

        var alertes = new List<Alerte>();
        DateOnly aujourdhui = horloge.Aujourdhui();

        // --- Règle 1 : échéance du contrat -------------------------------
        int joursAvantEcheance = contrat.DateFin.DayNumber - aujourdhui.DayNumber;

        if (joursAvantEcheance < 0)
        {
            alertes.Add(new Alerte(
                contrat.VehiculeId,
                "CONTRAT_ECHU",
                NiveauAlerte.Critique,
                $"Contrat echu depuis le {contrat.DateFin:dd/MM/yyyy}."));
        }
        else if (joursAvantEcheance <= SeuilEcheanceVigilanceJours)
        {
            NiveauAlerte niveau = joursAvantEcheance <= SeuilEcheanceCritiqueJours
                ? NiveauAlerte.Critique
                : NiveauAlerte.Vigilance;

            alertes.Add(new Alerte(
                contrat.VehiculeId,
                "ECHEANCE_CONTRAT",
                niveau,
                $"Fin de contrat le {contrat.DateFin:dd/MM/yyyy}, dans {joursAvantEcheance} jours."));
        }

        // --- Règle 2 : depassement kilometrique projete -------------------
        if (synthese.EcartKilometrique > 0 && contrat.KilometrageContractuel > 0)
        {
            decimal depassementRelatif =
                (decimal)synthese.EcartKilometrique / contrat.KilometrageContractuel;

            NiveauAlerte niveau = depassementRelatif > SeuilDepassementCritique
                ? NiveauAlerte.Critique
                : NiveauAlerte.Vigilance;

            alertes.Add(new Alerte(
                contrat.VehiculeId,
                "DEPASSEMENT_KM",
                niveau,
                $"Depassement projete de {synthese.EcartKilometrique:N0} km "
                + $"sur {contrat.KilometrageContractuel:N0} km contractuels "
                + $"({depassementRelatif:P1})."));
        }

        return alertes;
    }
}