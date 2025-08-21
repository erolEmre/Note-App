using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace NoteAppMVCPattern.Models
{
    public class AppUser : IdentityUser
    {      
        public virtual ICollection<Note> Notes { get; set; } = new List<Note>();        
    }

}
