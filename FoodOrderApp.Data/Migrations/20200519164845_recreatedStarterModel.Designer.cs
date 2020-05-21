﻿// <auto-generated />
using FoodOrderApp.Data.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FoodOrderApp.Data.Migrations
{
    [DbContext(typeof(FoodOrderContext))]
    [Migration("20200519164845_recreatedStarterModel")]
    partial class recreatedStarterModel
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FoodOrderApp.Models.PizzaModels.IngredientModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Ingredients");
                });

            modelBuilder.Entity("FoodOrderApp.Models.PizzaModels.PizzaIngredientsModel", b =>
                {
                    b.Property<int>("PizzaId")
                        .HasColumnType("int");

                    b.Property<int>("IngredientId")
                        .HasColumnType("int");

                    b.HasKey("PizzaId", "IngredientId");

                    b.HasIndex("IngredientId");

                    b.ToTable("PizzaIngredientsModel");
                });

            modelBuilder.Entity("FoodOrderApp.Models.PizzaModels.PizzaModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Size")
                        .HasColumnType("int");

                    b.Property<int>("StarterId")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(4,2)");

                    b.HasKey("Id");

                    b.HasIndex("StarterId");

                    b.ToTable("Pizzas");
                });

            modelBuilder.Entity("FoodOrderApp.Models.PizzaModels.PriceModels.IngredientPriceModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("IngredientId")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(4,2)");

                    b.Property<int>("Size")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IngredientId");

                    b.ToTable("IngredientPrices");
                });

            modelBuilder.Entity("FoodOrderApp.Models.PizzaModels.StarterModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Size")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Starters");
                });

            modelBuilder.Entity("FoodOrderApp.Models.PizzaModels.PizzaIngredientsModel", b =>
                {
                    b.HasOne("FoodOrderApp.Models.PizzaModels.IngredientModel", "Ingredient")
                        .WithMany("PizzaIngredients")
                        .HasForeignKey("IngredientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FoodOrderApp.Models.PizzaModels.PizzaModel", "Pizza")
                        .WithMany("PizzaIngredients")
                        .HasForeignKey("PizzaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FoodOrderApp.Models.PizzaModels.PizzaModel", b =>
                {
                    b.HasOne("FoodOrderApp.Models.PizzaModels.StarterModel", "Starter")
                        .WithMany("Pizzas")
                        .HasForeignKey("StarterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FoodOrderApp.Models.PizzaModels.PriceModels.IngredientPriceModel", b =>
                {
                    b.HasOne("FoodOrderApp.Models.PizzaModels.IngredientModel", "Ingredient")
                        .WithMany("Prices")
                        .HasForeignKey("IngredientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
