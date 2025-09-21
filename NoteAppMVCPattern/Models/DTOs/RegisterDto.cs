using System.ComponentModel.DataAnnotations;

namespace NoteAppMVCPattern.Models.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
        
    }
}
