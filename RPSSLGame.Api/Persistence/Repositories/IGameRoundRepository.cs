using RPSSLGame.Api.Persistence.Entities;

namespace RPSSLGame.Api.Persistence.Repositories;

public interface IGameRoundRepository
{
    Task AddAsync(GameRound gameRound, CancellationToken cancellationToken = default);
}