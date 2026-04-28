namespace RPSSLGame.Api.Models;

public class StatsDataResponse
{
    public int TotalRounds { get; init; }
    public int TotalWins { get; init; }
    public int TotalLosses { get; init; }
    public int TotalTies { get; init; }
    public ChoiceDto? MostPlayedChoice { get; init; }
    public ChoiceDto? MostWinningChoice { get; init; }
    public IEnumerable<ChoiceStatsResponse>? ChoiceStats { get; init; } = [];
}