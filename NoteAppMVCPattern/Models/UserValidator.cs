using FluentValidation;

namespace NoteAppMVCPattern.Models
{
    public class UserValidator : AbstractValidator<AppUser>
    {
        public UserValidator()
        {
            RuleFor(x => x.UserName).NotEmpty()
                .WithMessage("İsim boş geçilemez")
                .MinimumLength(3).WithMessage("İsimiz en az 3 harften oluşmalı")
                .MaximumLength(30).WithMessage("İsimiz en fazla 30 harf olabilir")
                ;
            //RuleFor(x => x.Id).NotEmpty()
            //    .GreaterThanOrEqualTo(0).WithMessage("Negatif sayı olamaz");

        }
    }
}
