using FluentValidation;
using StarterKit.Application.DTOs.Users;

namespace StarterKit.Application.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Role).Must(r => r == "user" || r == "admin")
            .WithMessage("Role must be 'user' or 'admin'");
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"[a-zA-Z]")
            .Matches(@"[0-9]");
    }
}

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        // At least one field must be provided (Managed in Controller/Service logic usually, 
        // but here we validate formats if fields are present)

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Role)
            .Must(r => r == "user" || r == "admin")
            .When(x => !string.IsNullOrEmpty(x.Role))
            .WithMessage("Role must be 'user' or 'admin'");

        RuleFor(x => x.Password)
            .MinimumLength(8)
            .Matches(@"[a-zA-Z]")
            .Matches(@"[0-9]")
            .When(x => !string.IsNullOrEmpty(x.Password));
    }
}