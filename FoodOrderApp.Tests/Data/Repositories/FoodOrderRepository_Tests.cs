using FoodOrderApp.Data.DataContext;
using FoodOrderApp.Data.Repositories;
using FoodOrderApp.Interfaces.Repositories;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.PriceModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Tests.Data.Repositories
{
    [TestClass]
    public class FoodOrderRepository_Tests
    {
        private readonly DbContextOptions _dbContextOptions = new DbContextOptionsBuilder<FoodOrderContext>().UseInMemoryDatabase(databaseName: "foodOrderDb_fake").Options;
        private ICollection<IngredientModel> ingredients;

        [TestInitialize]
        public void InitializeObjects()
        {
            this.ingredients = new List<IngredientModel>
            {
                new IngredientModel
                {
                    Id = 1,
                    Name = "ham",
                },
                new IngredientModel
                {
                    Id = 2,
                    Name = "mushrooms",
                },
                new IngredientModel
                {
                    Id = 3,
                    Name = "cheese",
                },
                new IngredientModel
                {
                    Id = 4,
                    Name = "tomatoes",
                }
            };
        }

        [TestMethod]
        public async Task AddObjectsToDatabase()
        {            
            using(var context = new FoodOrderContext(_dbContextOptions))
            {
                IFoodOrderRepository<IngredientModel> repo = new FoodOrderRepository<IngredientModel>(context);

                bool actualCreateResult = false;

                foreach (var ingredient in ingredients)
                {
                    actualCreateResult = await repo.CreateAsync(ingredient);

                    if(actualCreateResult == false)
                    {
                        break;
                    }
                }

                await context.SaveChangesAsync();
                
                Assert.AreEqual(true, actualCreateResult);

                var objectsFromRepo = (await repo.GetByExpressionAsync(x => x.Id > 0)).ToList();

                Assert.AreEqual(ingredients.Count, objectsFromRepo.Count);
            }
        }

        [TestMethod]
        public async Task DeleteObjectFromDatabase()
        {
            using (var context = new FoodOrderContext(_dbContextOptions))
            {
                IFoodOrderRepository<IngredientModel> repo = new FoodOrderRepository<IngredientModel>(context);

                IngredientModel ingredientToDelete = (await repo.GetByExpressionAsync(x => x.Id == ingredients.Count)).SingleOrDefault();

                bool deleteResult = await repo.DeleteAsync(ingredientToDelete);
                
                await context.SaveChangesAsync();

                Assert.AreEqual(true, deleteResult);

                List<IngredientModel> remianIngredients = (await repo.GetByExpressionAsync(x => x.Id > 0)).ToList();
                Assert.AreEqual(ingredients.Count - 1, remianIngredients.Count);
            }
        }


        [TestMethod]
        public async Task GetCollectionOfObjects()
        {
            using (var context = new FoodOrderContext(_dbContextOptions))
            {
                IFoodOrderRepository<IngredientModel> repo = new FoodOrderRepository<IngredientModel>(context);

                var result = await repo.GetByExpressionAsync(x => x.Id > 0);

                var resultsList = result.ToList();

                Assert.AreEqual(ingredients.Count - 1, resultsList.Count);

                for (int i = 1; i <= resultsList.Count; i++)
                {
                    Assert.AreEqual(ingredients.Single(x => x.Id == i).Id, resultsList[i - 1].Id);
                    Assert.AreEqual(ingredients.Single(x => x.Id == i).Name, resultsList[i - 1].Name);
                }
            }
        }

    }
}
