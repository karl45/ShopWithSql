using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Models.ViewModels
{
    public class UserRolesViewModel
    {
        public UserAccount userAccount { set; get; }
        public IdentityRole IdentityRole { set; get; }
    }
}
