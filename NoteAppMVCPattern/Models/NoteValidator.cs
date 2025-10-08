using FluentValidation;

namespace NoteAppMVCPattern.Models
{
    public class NoteValidator : AbstractValidator<Note>
    {
        public NoteValidator() 
        {
            RuleFor(t => t.Title).NotEmpty().WithMessage("Başlık boş geçilemez.Lütfen doldurun.")
                .MinimumLength(1).WithMessage("Başlık en az 1 harf içermeli")
                .MaximumLength(45).WithMessage("45 karakterden daha uzun başlık olamaz");

            RuleFor(c => c.Content).NotEmpty().WithMessage("İçerik boş olamaz.")
                .MaximumLength(3000).WithErrorCode("Mesaj sınır aşıldı hata kodu: 0001");
            
        }
    }
}
