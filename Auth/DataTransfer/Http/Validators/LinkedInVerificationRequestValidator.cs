
using Auth.Dto;
using Auth.Linkedin;
using FluentValidation;
public class LinkedInVerificationRequestValidator: AbstractValidator<LinkedInVerificationRequest>
{
    public LinkedInVerificationRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotNull()
            .NotEmpty();
    }
}