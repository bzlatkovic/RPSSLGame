using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RPSSLGame.Api.Domain;
using RPSSLGame.Api.Models;
using RPSSLGame.Api.Persistence.Entities;
using RPSSLGame.Api.Persistence.ReadModels;
using RPSSLGame.Api.Persistence.Repositories;
using RPSSLGame.Api.Services;

namespace RPSSLGame.Tests.UnitTests.Services;

public class GameServiceTests
{
    private readonly IGameRoundRepository _gameRoundRepository;
    private readonly GameService _gameService;
    private readonly ILogger<GameService> _logger;
    private readonly IRandomNumberService _randomNumberService;

    public GameServiceTests()
    {
        _randomNumberService = Substitute.For<IRandomNumberService>();
        _gameRoundRepository = Substitute.For<IGameRoundRepository>();
        _logger = Substitute.For<ILogger<GameService>>();
        _gameService = new GameService(_randomNumberService, _gameRoundRepository, _logger);
    }

    [Fact]
    public void GetChoices_ShouldReturnAllEnumValues()
    {
        var results = _gameService.GetChoices();

        results.Should().NotBeNullOrEmpty();
        results.Should().HaveCount(5);

        var expectedValues = Enum.GetValues<Choice>().Select(choice => new ChoiceDto(choice));
        results.Should().BeEquivalentTo(expectedValues);
    }

    [Theory]
    [InlineData(1, Choice.Paper)]
    [InlineData(3, Choice.Spock)]
    [InlineData(10, Choice.Rock)]
    [InlineData(54, Choice.Lizard)]
    [InlineData(72, Choice.Scissors)]
    public async Task GetRandomChoiceAsync_ShouldReturnCorrectChoice_BasedOnRandomNumber(int randomNumber, Choice expectedChoice)
    {
        _randomNumberService.GetRandomNumberAsync(Arg.Any<CancellationToken>()).Returns(randomNumber);

        var result = await _gameService.GetRandomChoiceAsync();

        result.Id.Should().Be((int)expectedChoice);
    }

    [Fact]
    public async Task PlayAsync_ReturnCorrectResponse_WhenPlayerWins()
    {
        var request = new PlayRequest((int)Choice.Rock);
        _randomNumberService.GetRandomNumberAsync(Arg.Any<CancellationToken>()).Returns(2);

        var response = await _gameService.PlayAsync(request);

        response.Result.Should().Be(nameof(GameResult.Win).ToLower());
        response.Player.Should().Be((int)Choice.Rock);
        response.Computer.Should().Be((int)Choice.Scissors);

        await _gameRoundRepository.Received(1).AddAsync(
            Arg.Is<GameRound>(gr =>
                gr.PlayerChoice == Choice.Rock &&
                gr.ComputerChoice == Choice.Scissors &&
                gr.Result == GameResult.Win),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PlayAsync_ShouldHandleTieScenarios()
    {
        var request = new PlayRequest((int)Choice.Paper);
        _randomNumberService.GetRandomNumberAsync(Arg.Any<CancellationToken>()).Returns(1);

        var response = await _gameService.PlayAsync(request);

        response.Result.Should().Be(nameof(GameResult.Tie).ToLower());
        await _gameRoundRepository.Received(1).AddAsync(
            Arg.Is<GameRound>(gr =>
                gr.PlayerChoice == Choice.Paper &&
                gr.ComputerChoice == Choice.Paper &&
                gr.Result == GameResult.Tie),
            Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task GetStatsAsync_ShouldReturnEmptyResponse_WhenNoStatsExist()
    {
        _gameRoundRepository.GetStatsAsync(Arg.Any<CancellationToken>()) .Returns([]);
        
        var result = await _gameService.GetStatsAsync(Arg.Any<CancellationToken>());
        
        result.Should().NotBeNull();
        result.TotalRounds.Should().Be(0);
        result.ChoiceStats.Should().BeNullOrEmpty();
    }
    
    [Fact]
    public async Task GetStatsAsync_ShouldCalculateAggregatesCorrectly()
    {
        // Arrange
        var stats = new List<ChoiceStats>
        {
            new() { Choice = Choice.Rock, TimesPlayed = 10, Wins = 5, Losses = 3, Ties = 2 },
            new() { Choice = Choice.Paper, TimesPlayed = 20, Wins = 2, Losses = 15, Ties = 3 }
        };

        _gameRoundRepository.GetStatsAsync(Arg.Any<CancellationToken>()).Returns(stats);

        // Act
        var result = await _gameService.GetStatsAsync(CancellationToken.None);
        result.TotalRounds.Should().Be(30); 
        result.TotalWins.Should().Be(7);    
        result.TotalLosses.Should().Be(18); 
        result.TotalTies.Should().Be(5);    
        result.MostPlayedChoice.Should().BeEquivalentTo(new ChoiceDto(Choice.Paper)); 
        result.MostWinningChoice.Should().BeEquivalentTo(new ChoiceDto(Choice.Rock));
        result.ChoiceStats.Should().NotBeNullOrEmpty();
        result.ChoiceStats.Should().HaveCount(2);
        var rock = result.ChoiceStats.Should().Contain(x=> x.Choice.Id == (int)Choice.Rock).Which;
        rock.TimesPlayed.Should().Be(10);
        rock.Wins.Should().Be(5);
        rock.Losses.Should().Be(3);
        rock.Ties.Should().Be(2);
        var paper = result.ChoiceStats.Should().Contain(x=> x.Choice.Id == (int)Choice.Paper).Which;
        paper.TimesPlayed.Should().Be(20);
        paper.Wins.Should().Be(2);
        paper.Losses.Should().Be(15);
        paper.Ties.Should().Be(3);
    }
}