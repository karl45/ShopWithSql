using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.ContextDb
{
    public class ShopContext : IdentityDbContext<UserAccount>
    {
        public ShopContext(DbContextOptions options) : base(options)
        {
            Database.Migrate();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Good>()
                .HasOne(x => x.UserAccount)
                .WithMany(x => x.Goods)
                .HasForeignKey(x => x.UserAccountId);
            base.OnModelCreating(builder);
        }

  
  
        public DbSet<Shop.Models.Good> Good { get; set; }
    }
}
