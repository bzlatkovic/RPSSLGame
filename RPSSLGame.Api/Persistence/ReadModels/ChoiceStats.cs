using RPSSLGame.Api.Domain;

namespace RPSSLGame.Api.Persistence.ReadModels;

public class ChoiceStats
{
    public Choice Choice { get; init; }
    public int TimesPlayed { get; init; }
    public int Wins { get; init; }
    public int Losses { get; init; }
    public int Ties { get; init; }
}