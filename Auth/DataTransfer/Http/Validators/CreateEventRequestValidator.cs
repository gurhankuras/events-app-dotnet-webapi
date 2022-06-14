
using Auth.Dto;
using Auth.Linkedin;
using FluentValidation;
/*
public class CreateEventRequestValidator: AbstractValidator<CreateEventRequest>
{
    public CreateEventRequestValidator()
    {
        setRuleForDesciption();
        setRuleForTitle();
        //setRuleForStartingDate();

        RuleFor(x => x.Image)
            .NotNull().WithName("image");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitute)
            .InclusiveBetween(-180, 180);
        
    }

    private void setRuleForDesciption() 
    {
        var minLength = 20;
        var maxLength = 1500;
        var fieldName = "description";

        RuleFor(x => x.Description)
            .NotNull()
            .MinimumLength(minLength).WithName(fieldName)
            .WithMessage(ValidatorUtils.MinLengthMessage(fieldName, minLength))
            .MaximumLength(maxLength).WithName(fieldName)
            .WithMessage(ValidatorUtils.MaxLengthMessage(fieldName, maxLength));
    }

    private void setRuleForTitle() 
    {
        var minLength = 5;
        var maxLength = 120;
        var fieldName = "title";

        RuleFor(x => x.Title)
            .NotNull()
            .MinimumLength(minLength).WithName(fieldName)
            .WithMessage(ValidatorUtils.MinLengthMessage(fieldName, minLength))
            .MaximumLength(maxLength).WithName(fieldName)
            .WithMessage(ValidatorUtils.MaxLengthMessage(fieldName, maxLength));
    }


    private void setRuleForStartingDate() 
    {
        RuleFor(x => x.At)
        .Must(BeAValidDateAndAfterNow)
        .WithMessage("Starting date can't be after now");
    }



public static class ValidatorUtils 
{
    public static string MinLengthMessage(string fieldName, int length)
    {
        return $"{fieldName} length must be at least {length}";
    }

     public static string MaxLengthMessage(string fieldName, int length)
    {
        return $"{fieldName} length must not exceed {length}";
    }
}
*/