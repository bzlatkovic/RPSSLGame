using System.ComponentModel;

namespace RPSSLGame.Api.Models;

public class StatsDataResponse
{
    [DefaultValue(0)] public int TotalRounds { get; init; }

    [DefaultValue(0)] public int TotalWins { get; init; }

    [DefaultValue(0)] public int TotalLosses { get; init; }

    [DefaultValue(0)] public int TotalTies { get; init; }

    public ChoiceDto? MostPlayedChoice { get; init; }
    public ChoiceDto? MostWinningChoice { get; init; }
    public IEnumerable<ChoiceStatsResponse>? ChoiceStats { get; init; } = [];
}