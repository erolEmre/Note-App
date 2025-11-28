using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace NoteApp.Core.Entities
{
    public class AppUser : IdentityUser
    {      
        public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
        public virtual ICollection<Notebook> Notebook { get; set; } = new List<Notebook>();
    }

}
