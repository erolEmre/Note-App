using FluentValidation;
using NoteAppMVCPattern.Repo.User;

namespace NoteAppMVCPattern.Models
{
    public class UserValidator : AbstractValidator<AppUser>
    {
     
        public UserValidator()
        {
           
            
            RuleFor(x => x.UserName).NotEmpty()
                .WithMessage("İsim boş geçilemez")
                .MinimumLength(3).WithMessage("İsiminiz en az 3 harften oluşmalı")
                .MaximumLength(30).WithMessage("İsiminiz en fazla 30 harf olabilir")
                ;         

            //RuleFor(x => x.UserName)
            //    .Must(IsUserNameUnique)
            //    .WithMessage("Bu isim zaten kullanılıyor, lütfen başka bir isim seçin.");
        }
        //private bool IsUserNameUnique(string userName)
        //{
        //    var existingUser = _userRepository.GetAll()
        //        .FirstOrDefault(u => u.UserName == userName);
        //    return existingUser == null;
        //}
    }
}
