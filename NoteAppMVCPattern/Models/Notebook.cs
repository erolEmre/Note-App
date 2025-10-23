namespace NoteAppMVCPattern.Models
{
    public class Notebook
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public string? Color { get; set; }

        // Tag alanı eklenebilir.. Çoka çok ilişki
        public ICollection<Note> Notes { get; set; }
        public string? UserId { get; set; } // foreign key
        public virtual AppUser? User { get; set; } // navigation property

    }
}
