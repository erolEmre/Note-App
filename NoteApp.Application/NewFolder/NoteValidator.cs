using FluentValidation;
using NoteApp.Core.Entities;

namespace NoteApp.Core.Entities;

public class NoteValidator : AbstractValidator<Note>
{
    public NoteValidator() 
    {
        RuleFor(t => t.Title).NotEmpty().WithMessage("Başlık boş geçilemez.Lütfen doldurun.")
            .MinimumLength(1).WithMessage("Başlık, en az 1 harf içermeli")
            .MaximumLength(45).WithMessage("Başlık,45 harfen daha uzun olamaz");

        RuleFor(c => c.Content).NotEmpty().WithMessage("İçerik boş olamaz.")
            .MaximumLength(3000).WithMessage("Harf sınırı aşıldı.Maksimum 3000 harf kullanabilirsiniz.");
        
    }
}
