using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.ContextDb;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using Shop.Models;
using Shop.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Shop.Controllers
{
    [Authorize(Roles = "User,Manager")]
    public class GoodsController : Controller
    {
        private readonly ShopContext _context;
        private readonly UserManager<UserAccount> _userManager;
        public GoodsController(ShopContext context,UserManager<UserAccount> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Goods
        public IActionResult Index()
        {
           ViewData["UserAccountId"] = new SelectList(_context.Users, "Id", "UserName");            
           return View();
        }
        public async Task<IActionResult> GetGoodsUserName()
        {
            if (User.IsInRole("User"))
            {
                var shopContext = _context.Good
               .Include(g => g.UserAccount)
               .Where(x => x.UserAccount.UserName.Equals(User.Identity.Name))
               .Select(x => new { x.GoodName,x.BrandName});
                return StatusCode(200, await shopContext.ToListAsync());
            }
            else
            {
                 var shopContext = _context.Good
                .Include(g => g.UserAccount)
                .Select(x => new { x.GoodName, x.BrandName, x.UserAccount.UserName });
                return StatusCode(200, await shopContext.ToListAsync());
            }
        }

   
        // POST: Goods/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody][Bind("GoodName,BrandName,UserAccountId")] Good good)
        {
            //if (ModelState.IsValid)
            //{
            try
            {
                if (string.IsNullOrEmpty(good.UserAccountId))
                    good.UserAccountId = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;

                _context.Add(good);
                await _context.SaveChangesAsync();
                var x = _context.Good.Include(X => X.UserAccount).Where(x => x.UserAccountId.Equals(good.UserAccountId)).FirstOrDefault().UserAccount.UserName;
                return Ok(new { UserName=x});
            }
            catch (Exception e)
            {

            }
            return StatusCode(206);
        }

        

        [HttpPost]
        public async Task<IActionResult> Search([FromBody][Bind("GoodName,BrandName,UserName")] GoodUserNameViewModel good)
        {
            var list_Of_Searching = await _context.Good.
                Include(x => x.UserAccount)
                
                .Where(x => 
                EF.Functions.Like(x.GoodName, $"%{good.GoodName}%")&& 
                (good.UserName.Length == 0 && User.IsInRole("Manager") ? 
                (x.UserAccount.UserName == null || EF.Functions.Like(x.UserAccount.UserName, $"%{good.UserName}%")) :
                EF.Functions.Like(x.UserAccount.UserName, $"%{good.UserName}%"))&& 
                EF.Functions.Like(x.BrandName, $"%{good.BrandName}%"))
                .Select(x=>new {GoodName=x.GoodName,BrandName=x.BrandName,UserName=x.UserAccount.UserName })
                .ToListAsync();

            return Ok(list_Of_Searching);
        }
   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GoodId,GoodName,UserAccountId")] Good good)
        {
            if (id != good.GoodId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(good);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GoodExists(good.GoodId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserAccountId"] = new SelectList(_context.Users, "Id", "Id", good.UserAccountId);
            return View(good);
        }

        // GET: Goods/Delete/5
       
        // POST: Goods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var good = await _context.Good.FindAsync(id);
            _context.Good.Remove(good);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GoodExists(int id)
        {
            return _context.Good.Any(e => e.GoodId == id);
        }
    }
}
