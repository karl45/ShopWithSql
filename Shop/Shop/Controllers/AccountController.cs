using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Shop.Models;
using Shop.Models.ViewModels;
using System.Collections.Generic;
using System;
using System.Linq;
using Shop.ContextDb;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CustomIdentityApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<UserAccount> _userManager;
        private readonly SignInManager<UserAccount> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        
        public AccountController(UserManager<UserAccount> userManager,
            SignInManager<UserAccount> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            else
                return View();
        }
        [HttpGet]
        public IActionResult ManageUsers()
        {
            ViewData["Roles"] = new SelectList(_roleManager.Roles, "Name", "Name");
            return View();
        }

        public IActionResult GetUsers() => Ok(_userManager.Users.Select(x => new { Email = x.Email,RoleName=_userManager.GetRolesAsync(x).Result.FirstOrDefault()}));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager
                        .PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);

                    if (result.Succeeded)
                        return Ok();
                }
            }
            catch(Exception e)
            {

            }
            return StatusCode(206,
                           new { Code = "Ошибка авторизации", Description = "Неверный логин или пароль" }
                       );
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            else
                return View();
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            if (User.IsInRole("User"))
                return RedirectToAction("Index", "Goods");
            else if (User.IsInRole("Manager"))
                return RedirectToAction("Index", "Home");


         return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login","Account");
        }

        [HttpPost]
        public async Task<IActionResult> Search([FromBody][Bind("Email,RoleName")] RegisterWithRoleViewModel model)
        {
            try
            {

                var list_Of_Searching = await _userManager.Users
                    .Where(x => EF.Functions.Like(x.Email, $"%{model.Email}%"))
                    .Select(x=>new { x.Email,RoleName=_userManager.GetRolesAsync(x).Result.FirstOrDefault()})
                    .ToListAsync();
                return Ok(list_Of_Searching);
            }
            catch (Exception e)
            {

            }
            return StatusCode(206);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterWithRole([FromBody] RegisterWithRoleViewModel model)
        {
            try
            {
                UserAccount user = new UserAccount
                {
                    Email = model.Email,
                    UserName = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);


                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.RoleName);
                    await _signInManager.SignInAsync(user, false);
                    return Ok(new { Email=model.Email,RoleName=model.RoleName});
                }

                return StatusCode(403, result.Errors.
                    Select(x => new { Code = x.Code, Description = x.Description }));

            }
            catch (Exception e)
            {
                return StatusCode(500);
            }

        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            try
            {
                UserAccount user = new UserAccount { Email = model.Email, 
                    UserName = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                
              
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, false);
                    return Ok();
                }

                return StatusCode(403, result.Errors.
                    Select(x => new { Code = x.Code,Description = x.Description }));
                
            }
            catch(Exception e)
            {
                return StatusCode(500);
            }

        }
    }
}