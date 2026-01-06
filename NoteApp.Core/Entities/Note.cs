using NoteApp.Core.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoteApp.Core.Entities
{
    public class Note
    {
        [Key]
        public int Id { get; set; }
        //[Display(Name = "Not İçeriği")]
        public string? Content { get; set; }
        //[Display(Name = "Başlık")]
        public string? Title { get; set; }
        public NoteStatus Status { get; set; } = NoteStatus.None;
        public DateTime? PlannedDate { get; set; }
        [NotMapped]
        public string StatusName => Status.ToString();
        public DateTime CreateDate {  get; set; }
        public DateTime updatedDate { get; set; }      
        public string? UserId { get; set; } // foreign key
        public virtual AppUser? User { get; set; } // navigation property
        public Notebook Notebook { get; set; }
        public int NotebookId { get; set; }
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
