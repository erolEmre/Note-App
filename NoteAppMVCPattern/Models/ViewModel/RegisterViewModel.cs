using System.ComponentModel.DataAnnotations;

namespace NoteApp.WebUI.Models.ViewModel
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        //[Display(Name = "Şifre")]
        public string Password { get; set; }
    }
}
