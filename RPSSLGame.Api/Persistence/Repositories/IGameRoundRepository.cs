using RPSSLGame.Api.Persistence.Entities;
using RPSSLGame.Api.Persistence.ReadModels;

namespace RPSSLGame.Api.Persistence.Repositories;

public interface IGameRoundRepository
{
    Task AddAsync(GameRound gameRound, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChoiceStats>> GetStatsAsync(CancellationToken cancellationToken);
}