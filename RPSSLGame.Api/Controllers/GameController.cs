using Microsoft.AspNetCore.Mvc;
using RPSSLGame.Api.Models;
using RPSSLGame.Api.Services;

namespace RPSSLGame.Api.Controllers;

[ApiController]
[Route("/")]
public class GameController(IGameService gameService) : ControllerBase
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
    public async Task<IActionResult> PlayAsync([FromBody] PlayRequest request, CancellationToken cancellationToken = default)
    {
        var choice = await gameService.PlayAsync(request, cancellationToken);
        return Ok(choice);
    }
}