using System.ComponentModel.DataAnnotations;

namespace NoteAppMVCPattern.Models.ViewModel
{
    public class AppUserVM
    {       
            [Required]
            public string UserName { get; set; }
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            public bool RememberMe { get; set; }

    }
}
