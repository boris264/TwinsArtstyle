﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TwinsArtstyle.Infrastructure.Models;

namespace TwinsArtstyle.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Address> Addresses { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Message> Messages{ get; set; }

        public DbSet<CartProductCount> CartsProductsCount { get; set; }

        public DbSet<OrderProductCount> OrdersProductsCount { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CartProductCount>()
                .HasKey(k => new { k.CartId, k.ProductId });

            builder.Entity<OrderProductCount>()
                .HasKey(k => new { k.OrderId, k.ProductId });

            builder.Entity<Address>()
                .HasOne(u => u.User)
                .WithMany(a => a.Addresses)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(builder);
        }
    }
}