using NoteApp.Core.Entities.Enums;
using NoteApp.Core.Entities;
namespace NoteApp.WebUI.Models.ViewModel
{
    public class NoteFooterVM
    {
        public Note Note { get; set; }
        public List<Tag> Tags { get; set; }
        public string IconClass => Status switch
        {
            NoteStatus.None => "bi bi-calendar",
            NoteStatus.Planned => "bi bi-clock-history",
            NoteStatus.Done => "bi bi-check-lg",
            NoteStatus.Todo => "bi bi-hourglass-split",
            _ => "bi bi-question-circle"
        };

        public NoteStatus Status => Note.Status;

        public string NoteStatusTurkce => Note.Status switch
        {
            NoteStatus.None => "Boş",
            NoteStatus.Planned => "planlandı",
            NoteStatus.Done => "yapıldı",
            NoteStatus.Todo => "yapılacak",
            _ => ""
        };
    }
}
