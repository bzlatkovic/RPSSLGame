using Microsoft.EntityFrameworkCore;
using RPSSLGame.Api.Domain;
using RPSSLGame.Api.Persistence.Entities;
using RPSSLGame.Api.Persistence.ReadModels;

namespace RPSSLGame.Api.Persistence.Repositories;

public class GameRoundRepository(AppDbContext dbContext) : IGameRoundRepository
{
    public async Task AddAsync(GameRound gameRound, CancellationToken cancellationToken = default)
    {
        await dbContext.GameRounds.AddAsync(gameRound, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<ChoiceStats>> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var stats = await dbContext.GameRounds
            .GroupBy(x => x.PlayerChoice)
            .Select(g => new ChoiceStats
            {
                Choice = g.Key,
                TimesPlayed = g.Count(),
                Wins = g.Count(x => x.Result == GameResult.Win),
                Losses = g.Count(x => x.Result == GameResult.Lose),
                Ties = g.Count(x => x.Result == GameResult.Tie)
            }).ToListAsync(cancellationToken);

        return stats.OrderBy(x => x.Choice);
    }
}