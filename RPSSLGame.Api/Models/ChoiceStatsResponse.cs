namespace RPSSLGame.Api.Models;

public class ChoiceStatsResponse
{
    public ChoiceDto? Choice { get; init; }
    public int TimesPlayed { get; init; }
    public int Wins { get; init; }
    public int Losses { get; init; }
    public int Ties { get; init; }
}