using NoteApp.Application.NewFolder.DTOs;

namespace NoteApp.Application.Models.DTOs
{
    public class NoteDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateDate { get; set; }
        public List<TagDto> Tags { get; set; } = new();
    }
}
