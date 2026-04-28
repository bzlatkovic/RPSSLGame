using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RPSSLGame.Api.Constants;
using RPSSLGame.Api.Extensions;
using RPSSLGame.Api.Models;
using RPSSLGame.Api.Services;

namespace RPSSLGame.Api.Controllers;

[Tags("Game")]
[ApiController]
[Route("/")]
[Produces("application/json")]
[EnableRateLimiting(RateLimitingPolicies.FixedPolicy)]
public class GameController(IGameService gameService, IValidator<PlayRequest> playRequestValidator) : ControllerBase
{
    /// <summary>
    ///     Get all game choices
    /// </summary>
    /// <returns>
    ///     A list of all five choices: Rock, Paper, Scissors, Lizard, and Spock, each with an id and name.
    /// </returns>
    /// <response code="200">Successfully returned all five choices.</response>
    /// <response code="500">An unexpected server error occurred.</response>
    [HttpGet("choices")]
    [ProducesResponseType(typeof(IEnumerable<ChoiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public IActionResult GetChoices()
    {
        var choices = gameService.GetChoices();
        return Ok(choices);
    }

    /// <summary>
    ///     Get a randomly generated choice
    /// </summary>
    /// <remarks>
    ///     The choice is determined by an external random number service.
    ///     The returned number is mapped to one of the five choices.
    /// </remarks>
    /// <returns>A randomly selected choice with an id and name.</returns>
    /// <response code="200">Successfully returned a random choice.</response>
    /// <response code="500">An unexpected server error occurred.</response>
    /// ///
    /// <response code="503">The external random number service is unavailable.</response>
    [HttpGet("choice")]
    [ProducesResponseType(typeof(ChoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetChoice(CancellationToken cancellationToken = default)
    {
        var choice = await gameService.GetRandomChoiceAsync(cancellationToken);
        return Ok(choice);
    }

    /// <summary>
    ///     Plays a round against the computer
    /// </summary>
    /// <remarks>
    ///     The computer choice is determined by an external random number service.
    ///     Game rules follow the extended RPSSL variant: Rock crushes Scissors and Lizard,
    ///     Paper covers Rock and disproves Spock, Scissors cuts Paper and decapitates Lizard,
    ///     Spock smashes Scissors and vaporizes Rock, Lizard eats Paper and poisons Spock.
    ///     Each round is persisted to the database.
    /// </remarks>
    /// <param name="request">The player's choice represented as an integer id between 1 and 5.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The round result including player choice, computer choice, and outcome (win, lose, tie).</returns>
    /// <response code="200">Round played successfully. Returns player choice, computer choice, and result.</response>
    /// <response code="400">Invalid request. Player choice is missing or outside the valid range of 1-5.</response>
    /// <response code="500">An unexpected server error occurred.</response>
    /// <response code="503">The external random number service is unavailable.</response>
    [HttpPost("play")]
    [ProducesResponseType(typeof(PlayResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [Consumes("application/json")]
    public async Task<IActionResult> PlayAsync([FromBody] PlayRequest? request, CancellationToken cancellationToken = default)
    {
        if (request is null)
            return BadRequest(new ErrorResponse(ErrorMessages.Game.ChoiceRequired.Message, ErrorMessages.Game.ChoiceRequired.Code));

        var validationResult = await playRequestValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.ToErrorResponse());

        var choice = await gameService.PlayAsync(request, cancellationToken);
        return Ok(choice);
    }

    /// <summary>
    ///     Get aggregated statistics
    /// </summary>
    /// <remarks>
    ///     Statistics are calculated via database aggregation and include overall win/loss/tie counts,
    ///     most frequently played choice, most winning choice,
    ///     and a per-choice breakdown of results.
    /// </remarks>
    /// <returns>Aggregated game statistics across all rounds.</returns>
    /// <response code="200">Successfully returned game statistics.</response>
    /// <response code="500">An unexpected server error occurred.</response>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(StatsDataResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken = default)
    {
        var stats = await gameService.GetStatsAsync(cancellationToken);
        return Ok(stats);
    }
}