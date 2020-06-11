using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.PhotoModels;
using FoodOrderApp.Models.PizzaModels.PriceModels;
using FoodOrderApp.Models.UserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderApp.Data.DataContext
{
    public class FoodOrderContext : IdentityDbContext<UserModel, IdentityRole<int>, int, IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DbSet<IngredientModel> Ingredients { get; set; }
        public DbSet<StarterModel> Starters { get; set; }
        public DbSet<PizzaModel> Pizzas { get; set; }
        public DbSet<PhotoModel> Photos { get; set; }

        public FoodOrderContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PizzaStarterModel>(entity =>
            {
                entity.HasKey(ps => new { ps.PizzaId, ps.StarterId });
                entity.HasOne(ps => ps.Pizza).WithMany(s => s.PizzaStarters).HasForeignKey(k => k.PizzaId);
                entity.HasOne(ps => ps.Starter).WithMany(s => s.PizzaStarters).HasForeignKey(k => k.StarterId);
                entity.ToTable("PizzaStarters");
            });

            modelBuilder.Entity<IngredientPriceModel>(entity =>
            {
                entity.HasKey(k => k.Id);
                entity.Property(p => p.Price).HasColumnType("decimal(4,2)");
                entity.ToTable("IngredientPrices");
            });
           
            modelBuilder.Entity<IngredientModel>(entity =>
            {
                entity.HasKey(k => k.Id);
                entity.HasMany(p => p.Prices).WithOne(i => i.Ingredient);
                entity.ToTable("Ingredients");
            });

            modelBuilder.Entity<StarterModel>(entity =>
            {
                entity.HasKey(k => k.Id);
                entity.ToTable("Starters");
            });
                
            modelBuilder.Entity<PizzaModel>(entity =>
            {
                entity.HasKey(k => k.Id);
                entity.HasMany(t => t.TotalPrices).WithOne(p => p.Pizza);
                entity.HasOne(p => p.Photo).WithOne(p => p.Pizza);
                entity.ToTable("Pizzas");
            });

            modelBuilder.Entity<PizzaIngredientsModel>(entity =>
            {
                entity.HasKey(k => new { k.PizzaId, k.IngredientId });
                entity.HasOne(i => i.Ingredient).WithMany(pi => pi.PizzaIngredients).HasForeignKey(k => k.IngredientId);
                entity.HasOne(p => p.Pizza).WithMany(pi => pi.PizzaIngredients).HasForeignKey(k => k.PizzaId);
                entity.ToTable("PizzaIngredients");
            });

            // change default names of identity tables
            modelBuilder.Entity<UserModel>(entity => entity.ToTable("Users"));
            modelBuilder.Entity<IdentityRole<int>>(entity => entity.ToTable("Roles"));
            modelBuilder.Entity<IdentityUserClaim<int>>(entity => entity.ToTable("UserClaims"));
            modelBuilder.Entity<IdentityUserRole<int>>(entity => entity.ToTable("UserRoles"));
            modelBuilder.Entity<IdentityUserLogin<int>>(entity => entity.ToTable("UserLogins"));
            modelBuilder.Entity<IdentityRoleClaim<int>>(entity => entity.ToTable("RoleClaims"));
            modelBuilder.Entity<IdentityUserToken<int>>(entity => entity.ToTable("UserTokens"));
            modelBuilder.Entity<PhotoModel>(entity => entity.ToTable("Photos"));
        }
    }
}
