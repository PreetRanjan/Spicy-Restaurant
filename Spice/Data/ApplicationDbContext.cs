using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Spice.Models;
using Spice.Utility;

namespace Spice.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Catagory> Catagories { get; set; }
        public DbSet<SubCatagory> SubCatagories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Address> Addresses { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Entity<IdentityRole>().HasData(
            //    new IdentityRole
            //    {
            //        Name = SD.ManagerUser,
            //        NormalizedName = SD.ManagerUser
            //    },
            //    new IdentityRole
            //    {
            //        Name = SD.KitchenUser,
            //        NormalizedName = SD.KitchenUser
            //    }, new IdentityRole
            //    {
            //        Name = SD.Customer,
            //        NormalizedName = SD.Customer
            //    }, new IdentityRole
            //    {
            //        Name = SD.FrontDeskUser,
            //        NormalizedName = SD.FrontDeskUser
            //    }
            //    );
            

        }
    }
}
