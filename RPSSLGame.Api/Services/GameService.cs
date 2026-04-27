using RPSSLGame.Api.Domain;
using RPSSLGame.Api.Models;
using RPSSLGame.Api.Persistence.Entities;
using RPSSLGame.Api.Persistence.Repositories;

namespace RPSSLGame.Api.Services;

public class GameService(IRandomNumberService randomNumberService, IGameRoundRepository gameRoundRepository) : IGameService
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

        return new PlayResponse(
            result.ToString().ToLower(),
            (int)playerChoice,
            (int)computerChoice);
    }

    private async Task<Choice> GetRandomChoiceInternalAsync(CancellationToken cancellationToken = default)
    {
        var randomNumber = await randomNumberService.GetRandomNumberAsync(cancellationToken);
        return (Choice)(randomNumber % 5 + 1);
    }
}