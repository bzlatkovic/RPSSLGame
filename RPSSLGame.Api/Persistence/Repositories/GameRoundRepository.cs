using RPSSLGame.Api.Persistence.Entities;

namespace RPSSLGame.Api.Persistence.Repositories;

public class GameRoundRepository(AppDbContext dbContext) : IGameRoundRepository
{
    public async Task AddAsync(GameRound gameRound, CancellationToken cancellationToken = default)
    {
        await dbContext.GameRounds.AddAsync(gameRound, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}