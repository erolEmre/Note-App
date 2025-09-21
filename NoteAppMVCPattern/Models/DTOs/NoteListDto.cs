namespace NoteAppMVCPattern.Models.DTOs
{
    public class NoteListDto
    {
        public List<NoteDto> Notes { get; set; } = new();
        public List<string?> Tags { get; set; } = new();
        public string? CurrentTag { get; set; }
    }
}
