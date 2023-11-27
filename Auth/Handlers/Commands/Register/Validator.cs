using Auth.Handlers.Commands.Register.Dto;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Auth.Handlers.Commands.Register
{
    public class Validator : AbstractValidator<Commend>
    {
        public Validator(IStringLocalizer<Commend> _localization)
        {
            RuleFor(x => x.Model!.Email).NotEmpty().WithMessage(x => _localization.GetString("EmailRequired"))
                                  .EmailAddress().WithMessage(x => _localization.GetString("EmailvalidRequired"));

            RuleFor(x => x.Model!.FirstName).NotEmpty().WithMessage(x => _localization.GetString("FirstNameRequired"));
            RuleFor(x => x.Model!.LastName).NotEmpty().WithMessage(x => _localization.GetString("LastNameRequired"));
            RuleFor(x => x.Model!.Username).NotEmpty().WithMessage(x => _localization.GetString("UsernameRequired"));


            RuleFor(x => x.Model!.Password).NotEmpty().WithMessage(x => _localization.GetString("PasswordRequired"))
                    .MinimumLength(8).WithMessage(x => _localization.GetString("PasswordMinimumRequired"))
                    .MaximumLength(16).WithMessage(x => _localization.GetString("PasswordMaximumRequired"))
                    .Matches(@"[A-Z]+").WithMessage(x => _localization.GetString("PasswordUppercaseRequired"))
                    .Matches(@"[a-z]+").WithMessage(x => _localization.GetString("PasswordlowercaseRequired"))
                    .Matches(@"[0-9]+").WithMessage(x => _localization.GetString("PasswordLeastOneNumberRequired"))
                    .Matches(@"[\!\?\*\.]+").WithMessage(x => _localization.GetString("PasswordSpaiealCharactersRequired"));
        }
    }
}
