using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.PriceModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Data.DataContext
{
    public class FoodOrderContext : DbContext
    {
        public DbSet<IngredientPriceModel> IngredientPrices { get; set; }
        public DbSet<StarterPriceModel> StarterPrices { get; set; }
        public DbSet<IngredientModel> Ingredients { get; set; }
        public DbSet<StarterModel> Starters { get; set; }
        public DbSet<PizzaModel> Pizzas { get; set; }

        public FoodOrderContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StarterPriceModel>(entity =>
            {
                entity.HasKey(k => k.Id);
                entity.Property(p => p.Price).HasColumnType("decimal(4,2)");
            });

            modelBuilder.Entity<IngredientPriceModel>(entity =>
            {
                entity.HasKey(k => k.Id);
                entity.Property(p => p.Price).HasColumnType("decimal(4,2)");
            });
           
            modelBuilder.Entity<IngredientModel>(entity =>
            {
                entity.HasKey(k => k.Id);
                entity.HasMany(p => p.Prices).WithOne(i => i.Ingredient);
            });

            modelBuilder.Entity<StarterModel>(entity =>
            {
                entity.HasKey(k => k.Id);
                entity.HasMany(p => p.Prices).WithOne(s => s.Starter);
            });
                

            modelBuilder.Entity<PizzaModel>(entity =>
            {
                entity.HasKey(k => k.Id);
                entity.HasOne(s => s.Starter).WithMany(p => p.Pizzas);
                entity.Property(p => p.TotalPrice).HasColumnType("decimal(4,2)");
            });

            modelBuilder.Entity<PizzaIngredientsModel>(entity =>
            {
                entity.HasKey(k => new { k.PizzaId, k.IngredientId });
                entity.HasOne(i => i.Ingredient).WithMany(pi => pi.PizzaIngredients).HasForeignKey(k => k.IngredientId);
                entity.HasOne(p => p.Pizza).WithMany(pi => pi.PizzaIngredients).HasForeignKey(k => k.PizzaId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
