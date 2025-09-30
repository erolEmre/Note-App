using System.ComponentModel.DataAnnotations;

namespace NoteAppMVCPattern.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name ="Username")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        //[Display(Name = "Şifre")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
