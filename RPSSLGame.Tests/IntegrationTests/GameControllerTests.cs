using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RPSSLGame.Api.Constants;
using RPSSLGame.Api.Domain;
using RPSSLGame.Api.Models;
using RPSSLGame.Api.Persistence;
using RPSSLGame.Tests.Common;

namespace RPSSLGame.Tests.IntegrationTests;

public class GameControllerTests(GameApiFactory factory) : IClassFixture<GameApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.GameRounds.RemoveRange(db.GameRounds);
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task GetChoices_ReturnsAll()
    {
        var response = await _client.GetAsync("/choices");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var choices = await response.Content.ReadFromJsonAsync<List<ChoiceDto>>();
        choices.Should().NotBeNullOrEmpty();
        choices.Should().HaveCount(5);
        var expectedValues = Enum.GetValues<Choice>().Select(choice => new ChoiceDto(choice));
        choices.Should().BeEquivalentTo(expectedValues);
    }

    [Fact]
    public async Task GetChoice_ReturnsValidChoice()
    {
        factory.SetupValidResponse(5);

        var response = await _client.GetAsync("/choice");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var choice = await response.Content.ReadFromJsonAsync<ChoiceDto>();
        choice.Should().NotBeNull();
        choice.Id.Should().Be(1);
        choice.Name.Should().Be(nameof(Choice.Rock).ToLower());
    }

    [Fact]
    public async Task Play_WithValidChoice_ReturnsResult_SavesRoundToDatabase()
    {
        factory.SetupValidResponse(1);

        var response = await _client.PostAsJsonAsync("/play", new PlayRequest((int)Choice.Scissors));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PlayResponse>();
        result.Should().NotBeNull();
        result.Player.Should().Be((int)Choice.Scissors);
        result.Computer.Should().Be((int)Choice.Paper);
        result.Result.Should().Be(nameof(GameResult.Win).ToLower());

        var gameRounds = await factory.GetAllGameRounds();
        gameRounds.Should().NotBeNullOrEmpty();
        gameRounds.Should().HaveCount(1);

        var gameRound = gameRounds.First();
        gameRound.PlayerChoice.Should().Be(Choice.Scissors);
        gameRound.ComputerChoice.Should().Be(Choice.Paper);
        gameRound.Result.Should().Be(GameResult.Win);
    }

    [Fact]
    public async Task Play_WithNullRequest_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/play", (object?)null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error.Code.Should().Be(ErrorMessages.Game.ChoiceRequired.Code);
        error.Message.Should().Be(ErrorMessages.Game.ChoiceRequired.Message);
    }

    [Fact]
    public async Task Play_WithNullChoice_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/play", new PlayRequest(null));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error.Code.Should().Be(ErrorMessages.Game.ChoiceRequired.Code);
        error.Message.Should().Be(ErrorMessages.Game.ChoiceRequired.Message);
    }

    [Fact]
    public async Task Play_WithInvalidChoice_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/play", new PlayRequest(99));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error.Code.Should().Be(ErrorMessages.Game.InvalidChoice.Code);
        error.Message.Should().Be(ErrorMessages.Game.InvalidChoice.Message);
    }

    [Fact]
    public async Task Play_WithChoiceBelowRange_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/play", new PlayRequest(0));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error.Code.Should().Be(ErrorMessages.Game.InvalidChoice.Code);
        error.Message.Should().Be(ErrorMessages.Game.InvalidChoice.Message);
    }

    [Fact]
    public async Task GetChoice_WhenExternalServiceUnavailable_ReturnsServiceUnavailable()
    {
        factory.SetupServiceUnavailable();

        var response = await _client.GetAsync("/choice");

        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error.Code.Should().Be(ErrorMessages.Game.ExternalServiceUnavailable.Code);
        error.Message.Should().Be(ErrorMessages.Game.ExternalServiceUnavailable.Message);
    }
}