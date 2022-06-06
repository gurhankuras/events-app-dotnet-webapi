
using Auth.Dto;
using FluentValidation;
public class SignInRequestValidator: AbstractValidator<SignInRequest>
{
    public SignInRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotNull()
            .EmailAddress();

       var passwordMinLength = 9;
       var passwordMaxLength  = 16;  
       RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Your password cannot be empty")
            .MinimumLength(passwordMinLength).WithMessage($"Your password length must be at least {passwordMinLength}.")
            .MaximumLength(passwordMaxLength).WithMessage($"Your password length must not exceed {passwordMaxLength}.")
            .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
            .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
            .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
            .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");
    }
}
