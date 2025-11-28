using System.ComponentModel.DataAnnotations;

namespace NoteApp.Application.Models.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
        
    }
}
