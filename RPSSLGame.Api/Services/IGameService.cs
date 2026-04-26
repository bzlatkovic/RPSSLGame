using RPSSLGame.Api.Models;

namespace RPSSLGame.Api.Services;

public interface IGameService
{
    IEnumerable<ChoiceDto> GetChoices();
    Task<ChoiceDto> GetRandomChoiceAsync(CancellationToken cancellationToken);
    Task<PlayResponse> PlayAsync(PlayRequest request, CancellationToken cancellationToken);
}