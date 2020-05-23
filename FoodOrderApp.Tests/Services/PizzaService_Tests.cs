using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Interfaces.UnitOfWork;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.PriceModels;
//using FoodOrderApp.Services;
//using FoodOrderApp.Tests._Fakes.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Tests.Services
{
    [TestClass]
    public class PizzaService_Tests
    {
        private readonly IUnitOfWork _unitOfWorkFake;
        private readonly IPizzaService _service;

        private PizzaToCreateDto pizzaToCreate;

        public PizzaService_Tests()
        {
  //          _unitOfWorkFake = new UnitOfWork_Fake();
  //          _service = new PizzaService(_unitOfWorkFake);
        }

        [TestInitialize]
        public void Initialize()
        {
            List<IngredientModel> ingredients = new List<IngredientModel>
            {
                new IngredientModel
                {
                    Id = 1,
                    Name = "Mushrooms",
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
                        },
                        new IngredientPriceModel
                        {
                            Id = 3,
                            Price = 3.00M,
                            Size = SizeEnum.Big
                        },
                        new IngredientPriceModel
                        {
                            Id = 4,
                            Price = 4.00M,
                            Size = SizeEnum.Large
                        }
                    }
                },
                new IngredientModel
                {
                    Id = 2,
                    Name = "Cheese",
                    Prices = new List<IngredientPriceModel>
                    {
                        new IngredientPriceModel
                        {
                            Id = 5,
                            Price = 1.00M,
                            Size = SizeEnum.Small
                        },
                        new IngredientPriceModel
                        {
                            Id = 6,
                            Price = 2.00M,
                            Size = SizeEnum.Medium
                        },
                        new IngredientPriceModel
                        {
                            Id = 7,
                            Price = 3.00M,
                            Size = SizeEnum.Big
                        },
                        new IngredientPriceModel
                        {
                            Id = 8,
                            Price = 4.00M,
                            Size = SizeEnum.Large
                        }
                    }
                },
            };

            pizzaToCreate = new PizzaToCreateDto
            {
                Name = "TestPizza",
                IngredientIds = new List<int> { 1, 2 },
            };
        }

        [TestMethod]
        public async Task CreatePizzaCorrectly()
        {
            IServiceResult<PizzaToReturnDto> result = await _service.CreateAsync(pizzaToCreate);

            Assert.AreEqual(ResultType.Created, result.Result);
        }

        [TestMethod]
        public async Task CreatePizzaWithExistingName()
        {
            await _service.CreateAsync(pizzaToCreate);

            IServiceResult<PizzaToReturnDto> result = await _service.CreateAsync(pizzaToCreate);

            Assert.AreEqual(ResultType.Error, result.Result);
            Assert.AreEqual(1, result.Errors.Count);
        }

        //[TestMethod]
        //public async Task GetAllPizzas()
        //{

        //}


    }
}
