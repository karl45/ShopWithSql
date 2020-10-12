using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shop.ContextDb;
using Shop.Models;

namespace Shop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews() ;
            
            services.AddDbContext<ShopContext>(options => 
            options.UseSqlServer(Configuration.GetConnectionString("ShopConnection"))
            );

            services.AddIdentity<UserAccount, IdentityRole>(
                options => {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.User.RequireUniqueEmail = true;
                    options.Lockout.AllowedForNewUsers = false;
                    }
                ).AddEntityFrameworkStores<ShopContext>();

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
                              IWebHostEnvironment env,
                              IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");
            });
            SeedRoles(serviceProvider).Wait();
            Task.WhenAll(SeedUsers(serviceProvider), SeedGood(serviceProvider)).Wait();
        }
        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            RoleManager<IdentityRole> roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            if (!roleManager.RoleExistsAsync("User").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "User";
                var result = await roleManager.CreateAsync(role);
            }
            if (!roleManager.RoleExistsAsync("Manager").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Manager";
                var roleResult = await roleManager.CreateAsync(role);
            }
        }
        public static async Task SeedUsers(IServiceProvider serviceProvider)
        {
            UserManager<UserAccount> userManager =
                serviceProvider.GetRequiredService<UserManager<UserAccount>>();
            if (userManager.FindByNameAsync("userTemp").Result == null)
            {
                UserAccount user = new UserAccount();
                user.UserName = "userTemp";
                user.Email = "Javani23@mail.ru";

                IdentityResult result = await userManager.CreateAsync(user, "Temp24$");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
            }


            if (userManager.FindByNameAsync("ShopUser").Result == null)
            {
                UserAccount user = new UserAccount();
                user.UserName = "ShopUser";
                user.Email = "Bronciani34@mail.ru";
                IdentityResult result = await userManager.CreateAsync(user, "Shop34%");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Manager");
                }
            }
        }
        public static async Task SeedGood(IServiceProvider serviceProvider)
        {
            ShopContext shopContext =
                serviceProvider.GetRequiredService<ShopContext>();

            await shopContext.Good.AddRangeAsync(new List<Good>() {
                new Good() { GoodName = "Pen",BrandName="AllOk" },
                new Good() { GoodName = "Table",BrandName="Yahis" }
            });
        }


    }
}
