﻿using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Auth.Handlers.Commands.ForgetPassword
{
    public class Validator : AbstractValidator<Commend>
    {
        public Validator(IStringLocalizer<Commend> _localization)
        {
            RuleFor(x => x.Model!.Email).NotEmpty().WithMessage(x => _localization.GetString("EmailRequired"))
                                .EmailAddress().WithMessage(x => _localization.GetString("EmailvalidRequired"));

            RuleFor(x => x.Model!.Code).NotEmpty().WithMessage(x => _localization.GetString("CodeRequired"));

            RuleFor(x => x.Model!.NewPassword).NotEmpty().WithMessage(x => _localization.GetString("PasswordRequired"))
                    .MinimumLength(8).WithMessage(x => _localization.GetString("PasswordMinimumRequired"))
                    .MaximumLength(16).WithMessage(x => _localization.GetString("PasswordMaximumRequired"))
                    .Matches(@"[A-Z]+").WithMessage(x => _localization.GetString("PasswordUppercaseRequired"))
                    .Matches(@"[a-z]+").WithMessage(x => _localization.GetString("PasswordlowercaseRequired"))
                    .Matches(@"[0-9]+").WithMessage(x => _localization.GetString("PasswordLeastOneNumberRequired"))
                    .Matches(@"[\!\?\*\.]+").WithMessage(x => _localization.GetString("PasswordSpaiealCharactersRequired"));
        }
    }
}
