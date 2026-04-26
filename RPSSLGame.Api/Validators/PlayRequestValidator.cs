using FluentValidation;
using RPSSLGame.Api.Constants;
using RPSSLGame.Api.Models;

namespace RPSSLGame.Api.Validators;

public class PlayRequestValidator : AbstractValidator<PlayRequest>
{
    public PlayRequestValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Player)
            .NotNull()
            .WithErrorCode(ErrorMessages.Game.ChoiceRequired.Code)
            .WithMessage(ErrorMessages.Game.ChoiceRequired.Message)
            .InclusiveBetween(1, 5)
            .WithErrorCode(ErrorMessages.Game.InvalidChoice.Code)
            .WithMessage(ErrorMessages.Game.InvalidChoice.Message);
    }
}