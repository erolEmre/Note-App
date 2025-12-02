using NoteApp.Application.NewFolder.DTOs;

namespace NoteApp.Application.Models.DTOs
{
    public class NoteListDto
    {
        public List<NoteDto> Notes { get; set; } = new();
        public List<TagDto> Tags { get; set; } = new();
    }
}
