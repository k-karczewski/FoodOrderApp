using FoodOrderApp.Models.OrderModels;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.DetailModels;
using FoodOrderApp.Models.PizzaModels.PhotoModels;
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
        public DbSet<PizzaOrderModel> PizzaOrders { get; set; }
        public DbSet<OrderModel> Orders { get; set; }

        public FoodOrderContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PizzaModel>(entity =>
            {
               // entity.HasMany(pd => pd.PizzaDetails).WithOne(d => d.Pizza).HasForeignKey(k => k.pi);
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

            modelBuilder.Entity<PizzaDetailsModel>(entity =>
            {
                entity.HasOne(p => p.Pizza).WithMany(pd => pd.PizzaDetails).HasForeignKey(k => k.PizzaId);
                entity.HasOne(s => s.Starter).WithMany(pd => pd.Pizzas).HasForeignKey(k => k.StarterId);
                entity.Property(p => p.TotalPrice).HasColumnType("decimal(4,2)");
                entity.ToTable("PizzaDetails");
            });

            modelBuilder.Entity<PizzaOrderModel>(entity =>
            {
                entity.HasKey(po => new { po.PizzaDetailId, po.OrderId, po.PizzaId });
            });

            modelBuilder.Entity<OrderModel>(entity =>
            {
                entity.Property(p => p.TotalPrice).HasColumnType("decimal(4,2)");
                entity.HasOne(u => u.User).WithMany(o => o.Orders).HasForeignKey(k => k.UserId);
            });

            modelBuilder.Entity<IngredientDetailsModel>(entity =>
            {
                entity.HasOne(i => i.Ingredient).WithMany(id => id.IngredientDetails).HasForeignKey(k => k.IngredientId);
                entity.Property(p => p.Price).HasColumnType("decimal(4,2)");
                entity.ToTable("IngredientDetails");
            });
           
            modelBuilder.Entity<IngredientModel>(entity =>
            {
                entity.ToTable("Ingredients");
            });

            modelBuilder.Entity<StarterModel>(entity =>
            {
                entity.HasKey(k => k.Id);
                entity.Property(p => p.Price).HasColumnType("decimal(4,2)");
                entity.ToTable("Starters");
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
            modelBuilder.Entity<OrderModel>(entity => entity.ToTable("Orders"));
        }
    }
}
