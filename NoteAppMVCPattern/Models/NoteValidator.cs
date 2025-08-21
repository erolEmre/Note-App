using FluentValidation;

namespace NoteAppMVCPattern.Models
{
    public class NoteValidator : AbstractValidator<Note>
    {
        public NoteValidator() 
        {
            RuleFor(x => x.Title).NotEmpty()
                .MinimumLength(1).WithMessage("Başlık en az 1 harf içermeli")
                .MaximumLength(45).WithMessage("45 karakterden daha uzun başlık olamaz");

            RuleFor(x => x.Content).NotEmpty()
                .MaximumLength(3000).WithErrorCode("Mesaj sınır aşıldı hata kodu: 0001");
            
        }
    }
}
