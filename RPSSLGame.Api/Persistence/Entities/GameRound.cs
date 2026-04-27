using RPSSLGame.Api.Domain;

namespace RPSSLGame.Api.Persistence.Entities;

public class GameRound
{
    private GameRound() { }

    public Guid Id { get; private set; }
    public Choice PlayerChoice { get; private set; }
    public Choice ComputerChoice { get; private set; }
    public GameResult Result { get; private set; }
    public DateTime PlayedAt { get; private set; }

    public static GameRound Create(Choice playerChoice, Choice computerChoice, GameResult result)
    {
        return new GameRound
        {
            Id = Guid.CreateVersion7(),
            PlayerChoice = playerChoice,
            ComputerChoice = computerChoice,
            Result = result,
            PlayedAt = DateTime.UtcNow
        };
    }
}