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
        private readonly DbContextOptions _dbContextOptions = new DbContextOptionsBuilder<FoodOrderContext>().UseInMemoryDatabase(databaseName: "foodOrderDb_stub").Options;
        private ICollection<IngredientModel> ingredients;
        private readonly int _idOfObject = 1;

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

        //[TestMethod]
        //public void CreateObject()
        //{            
        //    using(var context = new FoodOrderContext(_dbContextOptions))
        //    {
        //        IFoodOrderRepository<IngredientModel> repo = new FoodOrderRepository<IngredientModel>(context);

        //        bool createResult = repo.CreateAsync(ingredients.SingleOrDefault(id => id.Id == _idOfObject)).Result;

        //        Assert.AreEqual(true, createResult);
        //    }
        //}

        //[TestMethod]
        //public void DeleteObject()
        //{
        //    using (var context = new FoodOrderContext(_dbContextOptions))
        //    {
        //        IFoodOrderRepository<IngredientModel> repo = new FoodOrderRepository<IngredientModel>(context);

        //        bool deleteResult = repo.DeleteAsync(_idOfObject).Result;

        //        Assert.AreEqual(true, deleteResult);
        //    }
        //}

        //[TestMethod]
        //public void GetCollectionOfObjects()
        //{
        //    using(var context = new FoodOrderContext(_dbContextOptions))
        //    {
        //        IFoodOrderRepository<IngredientModel> repo = new FoodOrderRepository<IngredientModel>(context);

        //        foreach (IngredientModel ingredient in ingredients)
        //        {
        //           repo.CreateAsync(ingredient);
        //        }

        //        var result = repo.GetByExpressionAsync(x => x.Id > 0, null);
        //        var resultsList = result.Result.ToList();

        //        Assert.AreEqual(ingredients.Count, resultsList.Count);

        //        for(int i = 1; i <= resultsList.Count; i++)
        //        {
        //            Assert.AreSame(ingredients.Single(x => x.Id == i), resultsList[i-1]);
        //        }
        //    }
        //}
    }
}
