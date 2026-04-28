using System.ComponentModel;

namespace RPSSLGame.Api.Models;

public class ChoiceStatsResponse
{
    public ChoiceDto? Choice { get; init; }

    [DefaultValue(0)] public int TimesPlayed { get; init; }

    [DefaultValue(0)] public int Wins { get; init; }

    [DefaultValue(0)] public int Losses { get; init; }

    [DefaultValue(0)] public int Ties { get; init; }
}