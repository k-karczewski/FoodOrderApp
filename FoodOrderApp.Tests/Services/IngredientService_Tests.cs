using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Interfaces.UnitOfWork;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.PriceModels;
using FoodOrderApp.Services;
using FoodOrderApp.Tests._Fakes.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Tests.Services
{
    [TestClass]
    public class IngredientService_Tests
    {
        private readonly IUnitOfWork _unitOfWorkFake;
        private readonly IIngredientService _service;

        public int Id;
        List<IngredientModel> ingredients;

        public IngredientService_Tests()
        {
            _unitOfWorkFake = new UnitOfWork_Fake();
            _service = new IngredientService(_unitOfWorkFake);
        }

        [TestInitialize]
        public void Initialize()
        {
            Id = 1;
            InitializeRepo();
        }

        private void InitializeRepo()
        {
            ingredients = new List<IngredientModel>
            {
                new IngredientModel
                {
                    Id = 1,
                    Name = "Cheese",
                    Prices = new List<IngredientPriceModel>
                    {
                        new IngredientPriceModel
                        {
                            Id = 1,
                            Price = 1.00M,
                            Size = SizeEnum.Small
                        },
                        new IngredientPriceModel
                        {
                            Id = 2,
                            Price = 2.00M,
                            Size = SizeEnum.Medium
                        }
                    }
                },
                new IngredientModel
                {
                    Id = 2,
                    Name = "Mushrooms",
                    Prices = new List<IngredientPriceModel>
                    {
                        new IngredientPriceModel
                        {
                            Id = 3,
                            Price = 5.00M,
                            Size = SizeEnum.Small
                        },
                        new IngredientPriceModel
                        {
                            Id = 4,
                            Price = 10.00M,
                            Size = SizeEnum.Medium
                        }
                    }
                }
            };

            _unitOfWorkFake.Ingredients.CreateAsync(ingredients[0]);
            _unitOfWorkFake.Ingredients.CreateAsync(ingredients[1]);
        }

        [TestMethod]
        public async Task GetIngredientByCorrectId()
        {
            IServiceResult<IngredientModel> result = await _service.GetByIdAsync(Id);

            Assert.AreEqual(ResultType.Correct, result.Result);
            Assert.AreEqual(ingredients[0].Name, result.ReturnedObject.Name);
        }

        [TestMethod]
        public async Task GetIngredientByIncorrectId()
        {
            IServiceResult<IngredientModel> result = await _service.GetByIdAsync(Id-1000);

            Assert.AreEqual(ResultType.Error, result.Result);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [TestMethod]
        public async Task GetAllIngredients()
        {
            IServiceResult<List<IngredientModel>> result = await _service.GetAsync();

            Assert.AreEqual(ResultType.Correct, result.Result);
            Assert.AreEqual(ingredients.Count, result.ReturnedObject.Count);
        }

        [TestMethod]
        public async Task CreateNewIngredient()
        {
            IngredientModel newIngredient = new IngredientModel
            {
                Id = 3,
                Name = "Tomato"
            };

            IServiceResult<IngredientModel> result = await _service.CreateAsync(newIngredient);

            Assert.AreEqual(ResultType.Created, result.Result);
            Assert.AreEqual(newIngredient, result.ReturnedObject);
        }

        [TestMethod]
        public async Task CreateNewIngredientWithExistingName()
        {
            IngredientModel newIngredient = new IngredientModel
            {
                Id = 3,
                Name = "Tomato"
            };

            await _service.CreateAsync(newIngredient);
            IServiceResult<IngredientModel>  result = await _service.CreateAsync(newIngredient);

            Assert.AreEqual(ResultType.Error, result.Result);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [TestMethod]
        public async Task DeleteExistingIngredient()
        {
            IServiceResult result = await _service.DeleteAsync(ingredients[1].Id);

            Assert.AreEqual(ResultType.Deleted, result.Result);
        }

        [TestMethod]
        public async Task DeleteNotExistingIngredient()
        {
            await _service.DeleteAsync(ingredients[1].Id);
            IServiceResult result = await _service.DeleteAsync(ingredients[1].Id);

            Assert.AreEqual(ResultType.Error, result.Result);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [TestMethod]
        public async Task UpdatePrice()
        {
            IngredientPriceModel newPrice = new IngredientPriceModel
            {
                Id = 1,
                Size = 0,
                Price = 5.50M
            };

            IServiceResult<IngredientModel> result = await _service.UpdatePriceAsync(newPrice, 1);

            Assert.AreEqual(ResultType.Edited, result.Result);
            Assert.AreEqual(newPrice.Price, result.ReturnedObject.Prices.First(x => x.Size == newPrice.Size).Price);
        }

    }
}
