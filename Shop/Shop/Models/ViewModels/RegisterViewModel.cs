using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage ="Неверный формат Email")]
        public string Email { get; set; }
        [Required]

        public string Password { get; set; }
        [Required]

        public string PasswordConfirm { get; set; }
    }
}
