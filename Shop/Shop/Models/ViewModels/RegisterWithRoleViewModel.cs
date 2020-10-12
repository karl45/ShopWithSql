using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Models.ViewModels
{
    public class RegisterWithRoleViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage ="Неверный формат Email")]
        public string Email { get; set; }
        [Required]

        public string Password { get; set; }
        [Required]

        public string RoleName { set; get; }
    }
}
