using FluentValidation;
using StarterKit.Application.DTOs.Auth;

namespace StarterKit.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"[a-zA-Z]").WithMessage("Password must contain at least one letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number");
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"[a-zA-Z]")
            .Matches(@"[0-9]");
    }
}