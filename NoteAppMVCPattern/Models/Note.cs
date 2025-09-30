using System.ComponentModel.DataAnnotations;

namespace NoteAppMVCPattern.Models
{
    public class Note
    {
        [Key]
        public int Id { get; set; }
        //[Display(Name = "Not İçeriği")]
        public string Content { get; set; }
        //[Display(Name = "Başlık")]
        public string Title { get; set; }
        public DateTime CreateDate {  get; set; }
        public string? Tag { get; set; }
        public DateTime updatedDate { get; set; }

        public string? UserId { get; set; } // foreign key
        public virtual AppUser? User { get; set; } // navigation property
    }
}
