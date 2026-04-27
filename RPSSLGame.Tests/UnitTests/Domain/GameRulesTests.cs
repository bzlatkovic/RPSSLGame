using FluentAssertions;
using RPSSLGame.Api.Domain;

namespace RPSSLGame.Tests.UnitTests.Domain;

public class GameRulesTests
{
    [Theory]
    [InlineData(Choice.Rock, Choice.Rock)]
    [InlineData(Choice.Paper, Choice.Paper)]
    [InlineData(Choice.Scissors, Choice.Scissors)]
    [InlineData(Choice.Spock, Choice.Spock)]
    [InlineData(Choice.Lizard, Choice.Lizard)]
    public void DetermineResult_ShouldReturnTie_WhenChoicesAreSame(Choice player, Choice computer)
    {
        // Act
        var result = GameRules.DetermineResult(player, computer);

        // Assert
        result.Should().Be(GameResult.Tie);
    }

    [Theory]
    [InlineData(Choice.Rock, Choice.Scissors)]
    [InlineData(Choice.Rock, Choice.Lizard)]
    [InlineData(Choice.Paper, Choice.Rock)]
    [InlineData(Choice.Paper, Choice.Spock)]
    [InlineData(Choice.Scissors, Choice.Paper)]
    [InlineData(Choice.Scissors, Choice.Lizard)]
    [InlineData(Choice.Spock, Choice.Scissors)]
    [InlineData(Choice.Spock, Choice.Rock)]
    [InlineData(Choice.Lizard, Choice.Spock)]
    [InlineData(Choice.Lizard, Choice.Paper)]
    public void DetermineResult_ShouldReturnWin_WhenPlayerBeatsComputer(Choice player, Choice computer)
    {
        // Act
        var result = GameRules.DetermineResult(player, computer);

        // Assert
        result.Should().Be(GameResult.Win);
    }

    [Theory]
    [InlineData(Choice.Rock, Choice.Paper)]
    [InlineData(Choice.Rock, Choice.Spock)]
    [InlineData(Choice.Paper, Choice.Scissors)]
    [InlineData(Choice.Paper, Choice.Lizard)]
    [InlineData(Choice.Scissors, Choice.Rock)]
    [InlineData(Choice.Scissors, Choice.Spock)]
    [InlineData(Choice.Spock, Choice.Paper)]
    [InlineData(Choice.Spock, Choice.Lizard)]
    [InlineData(Choice.Lizard, Choice.Rock)]
    [InlineData(Choice.Lizard, Choice.Scissors)]
    public void DetermineResult_ShouldReturnLose_WhenComputerBeatsPlayer(Choice player, Choice computer)
    {
        var result = GameRules.DetermineResult(player, computer);

        result.Should().Be(GameResult.Lose);
    }
}