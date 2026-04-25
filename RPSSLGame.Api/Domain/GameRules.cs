namespace RPSSLGame.Api.Domain;

public static class GameRules
{
    private static readonly Dictionary<Choice, HashSet<Choice>> WinningMoves = new()
    {
        { Choice.Rock, [Choice.Scissors, Choice.Lizard] },
        { Choice.Paper, [Choice.Rock, Choice.Spock] },
        { Choice.Scissors, [Choice.Paper, Choice.Lizard] },
        { Choice.Spock, [Choice.Rock, Choice.Scissors] },
        { Choice.Lizard, [Choice.Paper, Choice.Spock] }
    };

    public static GameResult DetermineResult(Choice playerChoice, Choice computerChoice)
    {
        if (playerChoice == computerChoice)
            return GameResult.Tie;

        return WinningMoves[playerChoice].Contains(computerChoice) ? GameResult.Win : GameResult.Lose;
    }
}