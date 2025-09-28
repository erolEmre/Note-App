namespace NoteAppMVCPattern.Models;
using Microsoft.AspNetCore.Identity;
using FluentValidation;

public class FluentUserValidator : IUserValidator<AppUser>
{
    private readonly IValidator<AppUser> _validator;

    public FluentUserValidator(IValidator<AppUser> validator)
    {
        _validator = validator;
    }

    public async Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
    {
        var result = await _validator.ValidateAsync(user);

        if (result.IsValid)
            return IdentityResult.Success;

        var errors = result.Errors
            .Select(e => new IdentityError
            {
                Code = "ValidationError",
                Description = e.ErrorMessage
            });

        return IdentityResult.Failed(errors.ToArray());
    }
}
