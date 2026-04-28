using RPSSLGame.Api.Domain;
using RPSSLGame.Api.Models;
using RPSSLGame.Api.Persistence.Entities;
using RPSSLGame.Api.Persistence.Repositories;

namespace RPSSLGame.Api.Services;

public class GameService(
    IRandomNumberService randomNumberService,
    IGameRoundRepository gameRoundRepository,
    ILogger<GameService> logger) : IGameService
{
    private static readonly IReadOnlyList<ChoiceDto> _choices = Enum
        .GetValues<Choice>()
        .Select(choice => new ChoiceDto(choice))
        .ToList()
        .AsReadOnly();

    public IEnumerable<ChoiceDto> GetChoices()
    {
        return _choices;
    }

    public async Task<ChoiceDto> GetRandomChoiceAsync(CancellationToken cancellationToken = default)
    {
        var choice = await GetRandomChoiceInternalAsync(cancellationToken);
        return new ChoiceDto(choice);
    }

    public async Task<PlayResponse> PlayAsync(PlayRequest request, CancellationToken cancellationToken = default)
    {
        var playerChoice = (Choice)request.Player!;
        var computerChoice = await GetRandomChoiceInternalAsync(cancellationToken);
        var result = GameRules.DetermineResult(playerChoice, computerChoice);

        var gameRound = GameRound.Create(playerChoice, computerChoice, result);
        await gameRoundRepository.AddAsync(gameRound, cancellationToken);

        logger.LogInformation("Game played - Player: {PlayerChoice}, Computer: {ComputerChoice}, Result: {Result}", playerChoice, computerChoice, result);

        return new PlayResponse(
            result.ToString().ToLower(),
            (int)playerChoice,
            (int)computerChoice);
    }

    public async Task<StatsDataResponse> GetStatsAsync(CancellationToken cancellationToken)
    {
        var choiceStats = await gameRoundRepository.GetStatsAsync(cancellationToken);
        if (choiceStats is null || !choiceStats.Any())
        {
            logger.LogInformation("Stats requested but no rounds have been played yet");
            return new StatsDataResponse();
        }

        var mostPlayedChoice = choiceStats?.MaxBy(x => x.TimesPlayed)?.Choice;
        var mostWinningChoice = choiceStats?.MaxBy(x => x.Wins)?.Choice;

        return new StatsDataResponse
        {
            TotalRounds = choiceStats.Sum(x => x.TimesPlayed),
            TotalWins = choiceStats.Sum(x => x.Wins),
            TotalLosses = choiceStats.Sum(x => x.Losses),
            TotalTies = choiceStats.Sum(x => x.Ties),
            MostPlayedChoice = mostPlayedChoice.HasValue ? new ChoiceDto(mostPlayedChoice.Value) : null,
            MostWinningChoice = mostWinningChoice.HasValue ? new ChoiceDto(mostWinningChoice.Value) : null,
            ChoiceStats = choiceStats?.Select(c => new ChoiceStatsResponse
            {
                Choice = new ChoiceDto(c.Choice),
                TimesPlayed = c.TimesPlayed,
                Wins = c.Wins,
                Losses = c.Losses,
                Ties = c.Ties
            })
        };
    }

    private async Task<Choice> GetRandomChoiceInternalAsync(CancellationToken cancellationToken = default)
    {
        var randomNumber = await randomNumberService.GetRandomNumberAsync(cancellationToken);
        return (Choice)(randomNumber % 5 + 1);
    }
}