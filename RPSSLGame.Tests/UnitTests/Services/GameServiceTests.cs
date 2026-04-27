using FluentAssertions;
using NSubstitute;
using RPSSLGame.Api.Domain;
using RPSSLGame.Api.Models;
using RPSSLGame.Api.Persistence.Entities;
using RPSSLGame.Api.Persistence.Repositories;
using RPSSLGame.Api.Services;

namespace RPSSLGame.Tests.UnitTests.Services;

public class GameServiceTests
{
    private readonly IGameRoundRepository _gameRoundRepository;
    private readonly GameService _gameService;
    private readonly IRandomNumberService _randomNumberService;

    public GameServiceTests()
    {
        _randomNumberService = Substitute.For<IRandomNumberService>();
        _gameRoundRepository = Substitute.For<IGameRoundRepository>();
        _gameService = new GameService(_randomNumberService, _gameRoundRepository);
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
}