namespace FleetLite.Domaine;

/// <summary>Source de la date courante, injectée pour rendre les calculs testables.</summary>
public interface IHorloge
{
    DateOnly Aujourdhui();
}

public sealed class HorlogeSysteme : IHorloge
{
    public DateOnly Aujourdhui() => DateOnly.FromDateTime(DateTime.Today);
}

/// <summary>Horloge figée, utilisée par les tests et les démonstrations reproductibles.</summary>
public sealed class HorlogeFigee(DateOnly date) : IHorloge
{
    public DateOnly Aujourdhui() => date;
}