using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RPSSLGame.Api.Constants;
using RPSSLGame.Api.Extensions;
using RPSSLGame.Api.Models;
using RPSSLGame.Api.Services;

namespace RPSSLGame.Api.Controllers;

[ApiController]
[Route("/")]
[Produces("application/json")]
public class GameController(IGameService gameService, IValidator<PlayRequest> playRequestValidator) : ControllerBase
{
    [HttpGet("choices")]
    [ProducesResponseType(typeof(IEnumerable<ChoiceDto>), StatusCodes.Status200OK)]
    public IActionResult GetChoices()
    {
        var choices = gameService.GetChoices();
        return Ok(choices);
    }

    [HttpGet("choice")]
    [ProducesResponseType(typeof(ChoiceDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChoice(CancellationToken cancellationToken = default)
    {
        var choice = await gameService.GetRandomChoiceAsync(cancellationToken);
        return Ok(choice);
    }

    [HttpPost("play")]
    [ProducesResponseType(typeof(PlayResponse), StatusCodes.Status200OK)]
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
}