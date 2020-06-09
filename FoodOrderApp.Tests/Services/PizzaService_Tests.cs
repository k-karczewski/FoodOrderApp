using FoodOrderApp.Interfaces.Repositories;
using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Interfaces.UnitOfWork;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.PriceModels;
using FoodOrderApp.Services;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FoodOrderApp.Tests.Services
{
    [TestClass]
    public class PizzaService_Tests
    {
        private Mock<IUnitOfWork> uowMock;
        private Mock<IFoodOrderRepository<PizzaModel>> pizzaRepoMock;
        private IEnumerable<PizzaModel> expectedPizzas;
        private IList<StarterModel> expectedStarters;
        private IList<IngredientModel> expectedIngredients;

        [TestInitialize]
        public void Initialize()
        {
            pizzaRepoMock = new Mock<IFoodOrderRepository<PizzaModel>>();
            uowMock = new Mock<IUnitOfWork>();

            CreateTestPizzas();
        }

        [TestMethod]
        public async Task GetPizzaByName()
        {
            // initialize expected values
            string expectedPizzaName = "TestPizza";
            PizzaModel expectedPizza = (await GetPizzaByName(expectedPizzaName)).SingleOrDefault();

            // mock used methods/properties
            pizzaRepoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<PizzaModel, bool>>>(),
                                                       It.IsAny<Func<IQueryable<PizzaModel>, IIncludableQueryable<PizzaModel, object>>>())).Returns(GetPizzaByName(expectedPizzaName));

            uowMock.Setup(x => x.Pizzas).Returns(pizzaRepoMock.Object);

            // create instance of service
            IPizzaService service = new PizzaService(uowMock.Object);  
            
            // call function to test
            IServiceResult<PizzaToReturnDto> result = await service.GetByNameAsync(expectedPizzaName);

            // asserts
            Assert.AreEqual(ResultType.Correct, result.Result);
            Assert.IsNotNull(result.ReturnedObject);
            Assert.AreEqual(expectedPizza.Id, result.ReturnedObject.Id);
            Assert.AreEqual(expectedPizza.Name.ToLower(), result.ReturnedObject.Name.ToLower());
            Assert.AreEqual(expectedPizza.PizzaIngredients.Count, result.ReturnedObject.Ingredients.Count);
            Assert.AreEqual(expectedPizza.TotalPrices.Count, result.ReturnedObject.TotalPrices.Count);

            // check of total price makes sure that also ingredients have correct prices
            for(int i = 0; i < expectedPizza.TotalPrices.Count; i++)
            {
                Assert.AreEqual(expectedPizza.TotalPrices.ElementAt(i).Price, result.ReturnedObject.TotalPrices.ElementAt(i).Price);
            }         
        }

        [TestMethod]
        public async Task GetPizzaByIncorrectName()
        {
            // initialize expected values
            string expectedPizzaName = "TestPizza123";
            PizzaModel expectedPizza = (await GetPizzaByName(expectedPizzaName)).SingleOrDefault();

            // mock used methods/properties
            pizzaRepoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<PizzaModel, bool>>>(),
                                                       It.IsAny<Func<IQueryable<PizzaModel>, IIncludableQueryable<PizzaModel, object>>>())).Returns(GetPizzaByName(expectedPizzaName));

            uowMock.Setup(x => x.Pizzas).Returns(pizzaRepoMock.Object);

            // create instance of service
            IPizzaService service = new PizzaService(uowMock.Object);

            // call function to test
            IServiceResult<PizzaToReturnDto> result = await service.GetByNameAsync(expectedPizzaName);

            Assert.AreEqual(ResultType.Error, result.Result);
            Assert.IsNull(result.ReturnedObject);
            Assert.IsNotNull(result.Errors);
            Assert.AreEqual($"Pizzas with name: {expectedPizzaName} were not found", result.Errors.First());
        }

        [TestMethod]
        public async Task GetPizzaById()
        {
            int expectedId = 1;
            PizzaModel expectedPizza = (await GetPizzaById(expectedId)).Single();

            pizzaRepoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<PizzaModel, bool>>>(),
                                                       It.IsAny<Func<IQueryable<PizzaModel>, IIncludableQueryable<PizzaModel, object>>>())).Returns(GetPizzaById(expectedId));

            uowMock.Setup(x => x.Pizzas).Returns(pizzaRepoMock.Object);

            // create instance of service
            IPizzaService service = new PizzaService(uowMock.Object);

            // call function to test
            IServiceResult<PizzaModel> result = await service.GetByIdAsync(expectedId);

            Assert.AreEqual(ResultType.Correct, result.Result);
            Assert.IsNotNull(result.ReturnedObject);
            Assert.AreEqual(expectedPizza.Id, result.ReturnedObject.Id);
            Assert.AreEqual(expectedPizza.Name.ToLower(), result.ReturnedObject.Name.ToLower());
            Assert.AreEqual(expectedPizza.PizzaIngredients.Count, result.ReturnedObject.PizzaIngredients.Count);
            Assert.AreEqual(expectedPizza.TotalPrices.Count, result.ReturnedObject.TotalPrices.Count);

            // check of total price makes sure that also ingredients have correct prices
            for (int i = 0; i < expectedPizza.TotalPrices.Count; i++)
            {
                Assert.AreEqual(expectedPizza.TotalPrices.ElementAt(i).Price, result.ReturnedObject.TotalPrices.ElementAt(i).Price);
            }
        }

        [TestMethod]
        public async Task GetPizzaByIncorrectId()
        {
            int expectedId = -1000;
            PizzaModel expectedPizza = (await GetPizzaById(expectedId)).Single();

            pizzaRepoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<PizzaModel, bool>>>(),
                                                       It.IsAny<Func<IQueryable<PizzaModel>, IIncludableQueryable<PizzaModel, object>>>())).Returns(GetPizzaById(expectedId));

            uowMock.Setup(x => x.Pizzas).Returns(pizzaRepoMock.Object);

            // create instance of service
            IPizzaService service = new PizzaService(uowMock.Object);

            // call function to test
            IServiceResult<PizzaModel> result = await service.GetByIdAsync(expectedId);

            Assert.AreEqual(ResultType.Error, result.Result);
            Assert.IsNull(result.ReturnedObject);
            Assert.IsNotNull(result.Errors);
            Assert.AreEqual("Cannot load pizza from database", result.Errors.First());
        }

        [TestMethod]
        public async Task CreateNewPizzaCorrectly()
        {
            PizzaToCreateDto expectedPizzaToCreate = new PizzaToCreateDto
            {
                Name = "NewPizza",
                IngredientIds = new List<int>
                {
                    1
                }
            };
            // create mock of starter repository
            Mock<IFoodOrderRepository<StarterModel>> starterRepoMock = new Mock<IFoodOrderRepository<StarterModel>>();
            starterRepoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<StarterModel, bool>>>(),
                                                       It.IsAny<Func<IQueryable<StarterModel>, IIncludableQueryable<StarterModel, object>>>())).Returns(GetExpectedStarters());


            Mock<IFoodOrderRepository<IngredientModel>> ingredientRepoMock = new Mock<IFoodOrderRepository<IngredientModel>>();
            ingredientRepoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<IngredientModel, bool>>>(),
                                                       It.IsAny<Func<IQueryable<IngredientModel>, IIncludableQueryable<IngredientModel, object>>>())).
                                                       Returns(GetExpectedIngredientById(expectedPizzaToCreate.IngredientIds[0]));

            pizzaRepoMock.Setup(x => x.CreateAsync(It.IsAny<PizzaModel>())).Returns(Task.FromResult(true));

            uowMock.Setup(x => x.Pizzas).Returns(pizzaRepoMock.Object);
            uowMock.Setup(x => x.Starters).Returns(starterRepoMock.Object);
            uowMock.Setup(x => x.Ingredients).Returns(ingredientRepoMock.Object);

            IPizzaService service = new PizzaService(uowMock.Object);

            IServiceResult<PizzaToReturnDto> creationResult = await service.CreateAsync(expectedPizzaToCreate);

            Assert.AreEqual(ResultType.Created, creationResult.Result);
            Assert.IsNotNull(creationResult.ReturnedObject);
            Assert.AreEqual(expectedPizzaToCreate.Name, creationResult.ReturnedObject.Name);
            Assert.AreEqual("Mushrooms", creationResult.ReturnedObject.Ingredients.First().Name);
            Assert.IsNotNull(creationResult.ReturnedObject.TotalPrices);
            Assert.AreEqual(4, creationResult.ReturnedObject.TotalPrices.Count);
        }

        [TestMethod]
        public async Task CreateNewPizzaWithTakenName()
        {
            PizzaToCreateDto expectedPizzaToCreate = new PizzaToCreateDto
            {
                Name = "TestPizza",
                IngredientIds = new List<int>
                {
                    1
                }
            };

            pizzaRepoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<PizzaModel, bool>>>(),
                                                       It.IsAny<Func<IQueryable<PizzaModel>, IIncludableQueryable<PizzaModel, object>>>())).Returns(GetPizzaByName(expectedPizzaToCreate.Name));

            uowMock.Setup(x => x.Pizzas).Returns(pizzaRepoMock.Object);

            IPizzaService service = new PizzaService(uowMock.Object);

            IServiceResult<PizzaToReturnDto> creationResult = await service.CreateAsync(expectedPizzaToCreate);

            Assert.AreEqual(ResultType.Error, creationResult.Result);
            Assert.IsNull(creationResult.ReturnedObject);
            Assert.IsNotNull(creationResult.Errors);
            Assert.AreEqual(1, creationResult.Errors.Count);
            Assert.AreEqual($"Pizza name: {expectedPizzaToCreate.Name} is already taken", creationResult.Errors.First());
        }

        [TestMethod]
        public async Task DeletePizzaCorrectly()
        {
            int expectedPizzaId = 1;

            pizzaRepoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<PizzaModel, bool>>>(),
                                           It.IsAny<Func<IQueryable<PizzaModel>, IIncludableQueryable<PizzaModel, object>>>())).Returns(GetPizzaById(expectedPizzaId));

            pizzaRepoMock.Setup(x => x.DeleteAsync(It.IsAny<PizzaModel>())).Returns(Task.FromResult(true));

            uowMock.Setup(x => x.Pizzas).Returns(pizzaRepoMock.Object);
            uowMock.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(true));

            IPizzaService service = new PizzaService(uowMock.Object);

            IServiceResult deletionResult = await service.DeleteAsync(expectedPizzaId);

            Assert.AreEqual(ResultType.Deleted, deletionResult.Result);
            Assert.IsNull(deletionResult.Errors);
        }

        [TestMethod]
        public async Task DeleteNotExistingPizza()
        {
            int expectedPizzaId = 2;

            pizzaRepoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<PizzaModel, bool>>>(),
                                           It.IsAny<Func<IQueryable<PizzaModel>, IIncludableQueryable<PizzaModel, object>>>())).Returns(GetPizzaById(expectedPizzaId));

            uowMock.Setup(x => x.Pizzas).Returns(pizzaRepoMock.Object);

            IPizzaService service = new PizzaService(uowMock.Object);

            IServiceResult deletionResult = await service.DeleteAsync(expectedPizzaId);

            Assert.AreEqual(ResultType.Error, deletionResult.Result);
            Assert.IsNotNull(deletionResult.Errors);
            Assert.AreEqual($"Cannot delete pizza with id {expectedPizzaId}", deletionResult.Errors.First());
        }

        [TestMethod]
        public async Task CountPriceOfPizza()
        {
            int expectedPizzaId = 1;

            expectedPizzas.First().TotalPrices = new List<PizzaPriceModel>();
            
            pizzaRepoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<PizzaModel, bool>>>(),
                               It.IsAny<Func<IQueryable<PizzaModel>, IIncludableQueryable<PizzaModel, object>>>())).Returns(GetPizzaById(expectedPizzaId));

            uowMock.Setup(x => x.Pizzas).Returns(pizzaRepoMock.Object);

            IPizzaService service = new PizzaService(uowMock.Object);
            IServiceResult<PizzaModel> priceUpdateResult = await service.UpdatePriceAsync(expectedPizzaId);

            Assert.AreEqual(ResultType.Correct, priceUpdateResult.Result);
            Assert.IsNotNull(priceUpdateResult.ReturnedObject);
            Assert.AreEqual(4, priceUpdateResult.ReturnedObject.TotalPrices.Count);

            for(int i = 0; i < priceUpdateResult.ReturnedObject.TotalPrices.Count; i++)
            {
                Assert.AreNotEqual(0, priceUpdateResult.ReturnedObject.TotalPrices.ElementAt(i).Price);
            }
        }

        [TestMethod]
        public async Task CountPriceOfNotExistingPizza()
        {
            int expectedPizzaId = 2;

            pizzaRepoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<PizzaModel, bool>>>(),
                               It.IsAny<Func<IQueryable<PizzaModel>, IIncludableQueryable<PizzaModel, object>>>())).Returns(GetPizzaById(expectedPizzaId));

            uowMock.Setup(x => x.Pizzas).Returns(pizzaRepoMock.Object);

            IPizzaService service = new PizzaService(uowMock.Object);
            IServiceResult<PizzaModel> priceUpdateResult = await service.UpdatePriceAsync(expectedPizzaId);

            Assert.AreEqual(ResultType.Error, priceUpdateResult.Result);
            Assert.IsNull(priceUpdateResult.ReturnedObject);
            Assert.IsNotNull(priceUpdateResult.Errors);
            Assert.AreEqual($"Pizza with id {expectedPizzaId} was not found", priceUpdateResult.Errors.First());
        }

        [TestMethod]
        public async Task AddIngredientToPizzaCorrectly()
        {
            string expectedPizzaName = "TestPizza";
            int expectedIdOfIngredient = 3;

            pizzaRepoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<PizzaModel, bool>>>(),
                   It.IsAny<Func<IQueryable<PizzaModel>, IIncludableQueryable<PizzaModel, object>>>())).Returns(GetPizzaByName(expectedPizzaName));

            pizzaRepoMock.Setup(x => x.UpdateAsync(It.IsAny<PizzaModel>())).Returns(Task.FromResult(true));

            Mock<IFoodOrderRepository<IngredientModel>> ingredientRepoMock = new Mock<IFoodOrderRepository<IngredientModel>>();
            ingredientRepoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<IngredientModel, bool>>>(),
                   It.IsAny<Func<IQueryable<IngredientModel>, IIncludableQueryable<IngredientModel, object>>>())).Returns(GetExpectedIngredientById(expectedIdOfIngredient));

            uowMock.Setup(x => x.Pizzas).Returns(pizzaRepoMock.Object);
            uowMock.Setup(x => x.Ingredients).Returns(ingredientRepoMock.Object);
            uowMock.Setup(x => x.SaveChangesAsync());

            IPizzaService service = new PizzaService(uowMock.Object);

            IServiceResult<PizzaToReturnDto> result = await service.AddIngredientAsync(expectedPizzaName, expectedIdOfIngredient);

            Assert.AreEqual(ResultType.Edited, result.Result);
            Assert.IsNull(result.Errors);
            Assert.AreEqual(3, result.ReturnedObject.Ingredients.Count);

            for(int i = 0; i < result.ReturnedObject.TotalPrices.Count; i++)
            {
                Assert.AreEqual(expectedPizzas.First().TotalPrices.ElementAt(i).Price, result.ReturnedObject.TotalPrices.ElementAt(i).Price);
            }
        }

        [TestMethod]
        public async Task AddIngredientToPizzaThatIsAlreadyIncluded()
        {
            string expectedPizzaName = "TestPizza";
            int expectedIdOfIngredient = 2;

            pizzaRepoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<PizzaModel, bool>>>(),
                   It.IsAny<Func<IQueryable<PizzaModel>, IIncludableQueryable<PizzaModel, object>>>())).Returns(GetPizzaByName(expectedPizzaName));

            uowMock.Setup(x => x.Pizzas).Returns(pizzaRepoMock.Object);

            IPizzaService service = new PizzaService(uowMock.Object);

            IServiceResult<PizzaToReturnDto> result = await service.AddIngredientAsync(expectedPizzaName, expectedIdOfIngredient);

            Assert.AreEqual(ResultType.Error, result.Result);
            Assert.IsNull(result.ReturnedObject);
            Assert.IsNotNull(result.Errors);
            Assert.AreEqual($"Ingredient with id {expectedIdOfIngredient} is already included in pizza with name {expectedPizzaName}", result.Errors.First());
        }



        [TestMethod]
        public async Task DeleteIngredientFromPizzaCorrectly()
        {
            string expectedPizzaName = "TestPizza";
            int expectedIdOfIngredient = 2;

            pizzaRepoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<PizzaModel, bool>>>(),
                   It.IsAny<Func<IQueryable<PizzaModel>, IIncludableQueryable<PizzaModel, object>>>())).Returns(GetPizzaByName(expectedPizzaName));

            pizzaRepoMock.Setup(x => x.UpdateAsync(It.IsAny<PizzaModel>())).Returns(Task.FromResult(true));

            Mock<IFoodOrderRepository<IngredientModel>> ingredientRepoMock = new Mock<IFoodOrderRepository<IngredientModel>>();
            ingredientRepoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<IngredientModel, bool>>>(),
                   It.IsAny<Func<IQueryable<IngredientModel>, IIncludableQueryable<IngredientModel, object>>>())).Returns(GetExpectedIngredientById(expectedIdOfIngredient));

            uowMock.Setup(x => x.Pizzas).Returns(pizzaRepoMock.Object);
            uowMock.Setup(x => x.Ingredients).Returns(ingredientRepoMock.Object);
            uowMock.Setup(x => x.SaveChangesAsync());

            IPizzaService service = new PizzaService(uowMock.Object);

            IServiceResult<PizzaToReturnDto> result = await service.DeleteIngredientAsync(expectedPizzaName, expectedIdOfIngredient);

            Assert.AreEqual(ResultType.Edited, result.Result);
            Assert.IsNull(result.Errors);
            Assert.AreEqual(1, result.ReturnedObject.Ingredients.Count);

            for (int i = 0; i < result.ReturnedObject.TotalPrices.Count; i++)
            {
                Assert.AreEqual(expectedPizzas.First().TotalPrices.ElementAt(i).Price, result.ReturnedObject.TotalPrices.ElementAt(i).Price);
            }
        }

        [TestMethod]
        public async Task DeleteIngredientFromPizzaThatIsNotIncluded()
        {
            string expectedPizzaName = "TestPizza";
            int expectedIdOfIngredient = 3;

            pizzaRepoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<PizzaModel, bool>>>(),
                   It.IsAny<Func<IQueryable<PizzaModel>, IIncludableQueryable<PizzaModel, object>>>())).Returns(GetPizzaByName(expectedPizzaName));

            uowMock.Setup(x => x.Pizzas).Returns(pizzaRepoMock.Object);

            IPizzaService service = new PizzaService(uowMock.Object);

            IServiceResult<PizzaToReturnDto> result = await service.DeleteIngredientAsync(expectedPizzaName, expectedIdOfIngredient);

            Assert.AreEqual(ResultType.Error, result.Result);
            Assert.IsNull(result.ReturnedObject);
            Assert.IsNotNull(result.Errors);
            Assert.AreEqual($"Cannot delete ingredient with id {expectedIdOfIngredient} from pizza {expectedPizzaName} because it is not included in pizza {expectedPizzaName}", result.Errors.First());
        }

        private void CreateTestPizzas()
        {
            expectedIngredients = new List<IngredientModel>
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
                new IngredientModel
                {
                    Id = 3,
                    Name = "Ham",
                    Prices = new List<IngredientPriceModel>
                    {
                        new IngredientPriceModel
                        {
                            Id = 9,
                            Price = 3.00M,
                            Size = SizeEnum.Small
                        },
                        new IngredientPriceModel
                        {
                            Id = 10,
                            Price = 6.00M,
                            Size = SizeEnum.Medium
                        },
                        new IngredientPriceModel
                        {
                            Id = 11,
                            Price = 7.50M,
                            Size = SizeEnum.Big
                        },
                        new IngredientPriceModel
                        {
                            Id = 12,
                            Price = 9.00M,
                            Size = SizeEnum.Large
                        }
                    }
                },
            };

            expectedStarters = new List<StarterModel>
            {
                new StarterModel
                {
                    Id = 1,
                    Size = SizeEnum.Small,
                    Name = "Name1",
                    Price = 10.00M
                },
                new StarterModel
                {
                    Id = 2,
                    Size = SizeEnum.Medium,
                    Name = "Name2",
                    Price = 14.00M
                },
                new StarterModel
                {
                    Id = 3,
                    Size = SizeEnum.Big,
                    Name = "Name3",
                    Price = 18.00M
                },
                new StarterModel
                {
                    Id = 4,
                    Size = SizeEnum.Large,
                    Name = "Name4",
                    Price = 22.00M
                }
            };

            expectedPizzas = new List<PizzaModel>();

            PizzaModel pizza = new PizzaModel
            {
                Id = 1,
                Name = "TestPizza",
                PizzaIngredients = new List<PizzaIngredientsModel>
                {
                    new PizzaIngredientsModel
                    {
                        IngredientId = 1,
                        PizzaId = 1,
                        Ingredient = expectedIngredients[0]
                    },
                    new PizzaIngredientsModel
                    {
                        IngredientId = 2,
                        PizzaId = 1,
                        Ingredient = expectedIngredients[1]
                    },
                },
                PizzaStarters = new List<PizzaStarterModel>
                {
                    new PizzaStarterModel
                    {
                        PizzaId = 1,
                        StarterId = 1,
                        Starter = expectedStarters[0]
                    },
                    new PizzaStarterModel
                    {
                        PizzaId = 1,
                        StarterId = 2,
                        Starter = expectedStarters[1]
                    },
                    new PizzaStarterModel
                    {
                        PizzaId = 1,
                        StarterId = 3,
                        Starter = expectedStarters[2]
                    },
                    new PizzaStarterModel
                    {
                        PizzaId = 1,
                        StarterId = 4,
                        Starter = expectedStarters[3]
                    },

                },
                TotalPrices = new List<PizzaPriceModel>
                {
                    new PizzaPriceModel
                    {
                        Id = 1,
                        PizzaId = 1,
                        Price = 12.00M,
                        Size = SizeEnum.Small
                    },
                    new PizzaPriceModel
                    {
                        Id = 2,
                        PizzaId = 1,
                        Price = 18.00M,
                        Size = SizeEnum.Medium
                    },
                    new PizzaPriceModel
                    {
                        Id = 3,
                        PizzaId = 1,
                        Price = 24.00M,
                        Size = SizeEnum.Big
                    },
                    new PizzaPriceModel
                    {
                        Id = 4,
                        PizzaId = 1,
                        Price = 30.00M,
                        Size = SizeEnum.Large
                    },
                }
            };

            expectedPizzas = new List<PizzaModel>
            {
                pizza
            };
        }

        private async Task<IEnumerable<PizzaModel>> GetPizzaByName(string name)
        {
            return expectedPizzas.Where(x => x.Name.ToLower() == name.ToLower()).DefaultIfEmpty(null);
        }

        private async Task<IEnumerable<PizzaModel>> GetAllExpectedPizzas()
        {
            return expectedPizzas;
        }

        private async Task<IEnumerable<PizzaModel>> GetPizzaById(int id)
        {
            return expectedPizzas.Where(x => x.Id == id).DefaultIfEmpty(null);
        }

        private async Task<IEnumerable<StarterModel>> GetExpectedStarters() 
        {
            return expectedStarters;
        }

        private async Task<IEnumerable<IngredientModel>> GetExpectedIngredientById(int id)
        {
            return expectedIngredients.Where(x => x.Id == id);
        }
    }
}
