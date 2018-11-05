using System.ComponentModel.DataAnnotations;

namespace SportsStore.WebUI.Models
{
    //модель для передачи в представление
    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}