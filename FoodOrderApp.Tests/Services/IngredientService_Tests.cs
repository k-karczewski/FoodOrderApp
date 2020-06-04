using FoodOrderApp.Interfaces.Repositories;
using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Interfaces.UnitOfWork;
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
    public class IngredientService_Tests
    {
        List<IngredientModel> expectedIngredients;
        Mock<IUnitOfWork> uowMock;
        Mock<IFoodOrderRepository<IngredientModel>> repoMock;

        [TestInitialize]
        public void Initialize()
        {
            this.uowMock = new Mock<IUnitOfWork>();
            this.repoMock = new Mock<IFoodOrderRepository<IngredientModel>>();

            this.expectedIngredients = new List<IngredientModel>
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
        }

        [TestMethod]
        public async Task GetIngredientByCorrectId()
        {
            int idOfIngredient = 1;

            // mock getByExpression method from repository            
            repoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<IngredientModel, bool>>>(), 
                                                       It.IsAny<Func<IQueryable<IngredientModel>, IIncludableQueryable<IngredientModel, object>>>())).
                                                       Returns(GetFakeIngredientById(idOfIngredient));

            // mock Ingredients property from uow
            uowMock.Setup(x => x.Ingredients).Returns(repoMock.Object);

            // create new instance of IngredientService using mocked UnitOfWork
            IIngredientService service = new IngredientService(uowMock.Object);

            IServiceResult<IngredientModel> result = await service.GetByIdAsync(idOfIngredient);

            Assert.AreEqual(ResultType.Correct, result.Result);
            Assert.AreEqual(expectedIngredients[0].Id, result.ReturnedObject.Id);
            Assert.AreEqual(expectedIngredients[0].Name, result.ReturnedObject.Name); 
        }

        [TestMethod]
        public async Task GetIngredientByIncorrectId()
        {
            // incorrect id
            int idOfIngredient = -1000;

            // mock getByExpression method from repository            
            repoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<IngredientModel, bool>>>(),
                                                       It.IsAny<Func<IQueryable<IngredientModel>, IIncludableQueryable<IngredientModel, object>>>())).
                                                       Returns(GetFakeIngredientById(idOfIngredient));

            // mock Ingredients property from uow
            uowMock.Setup(x => x.Ingredients).Returns(repoMock.Object);

            // create new instance of IngredientService using mocked UnitOfWork
            IIngredientService service = new IngredientService(uowMock.Object);

            IServiceResult<IngredientModel> result = await service.GetByIdAsync(idOfIngredient);

            Assert.AreEqual(ResultType.Error, result.Result);
            Assert.IsNull(result.ReturnedObject);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual($"Object with id: {idOfIngredient} has not been found", result.Errors.First());
        }

        [TestMethod]
        public async Task GetAllIngredients()
        {
            repoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<IngredientModel, bool>>>(),
                                                        It.IsAny<Func<IQueryable<IngredientModel>, IIncludableQueryable<IngredientModel, object>>>())).
                                                        Returns(GetFakeIngredients());

            uowMock.Setup(x => x.Ingredients).Returns(repoMock.Object);

            IIngredientService ingredientService = new IngredientService(uowMock.Object);

            IServiceResult<List<IngredientModel>> getAllResult = await ingredientService.GetAsync();

            Assert.AreEqual(ResultType.Correct, getAllResult.Result);
            Assert.IsNotNull(getAllResult.ReturnedObject);
            Assert.AreEqual(expectedIngredients.Count, getAllResult.ReturnedObject.Count);
        }

        [TestMethod]
        public async Task CreateNewIngredient()
        {
            IngredientModel newIngredient = new IngredientModel
            {
                Id = 3,
                Name = "Tomato"
            };

            repoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<IngredientModel, bool>>>(), null)).Returns(GetFakeIngredientByName(newIngredient.Name));
            repoMock.Setup(x => x.CreateAsync(newIngredient)).Returns(Task.FromResult(true));

            uowMock.Setup(x => x.Ingredients).Returns(repoMock.Object);

            IIngredientService ingredientService = new IngredientService(uowMock.Object);

            IServiceResult<IngredientModel> result = await ingredientService.CreateAsync(newIngredient);
            
            Assert.AreEqual(ResultType.Created, result.Result);
            Assert.IsNotNull(result.ReturnedObject);
            Assert.AreEqual(newIngredient.Id, result.ReturnedObject.Id);
            Assert.AreEqual(newIngredient.Name, result.ReturnedObject.Name);
        }

        [TestMethod]
        public async Task CreateNewIngredientWithExistingName()
        {
            IngredientModel newIngredient = new IngredientModel
            {
                Id = 3,
                Name = "Mushrooms"
            };

            repoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<IngredientModel, bool>>>(), null)).Returns(GetFakeIngredientByName(newIngredient.Name));
            repoMock.Setup(x => x.CreateAsync(newIngredient)).Returns(Task.FromResult(true));

            uowMock.Setup(x => x.Ingredients).Returns(repoMock.Object);

            IIngredientService ingredientService = new IngredientService(uowMock.Object);

            await ingredientService.CreateAsync(newIngredient);
            IServiceResult<IngredientModel> result = await ingredientService.CreateAsync(newIngredient);

            Assert.AreEqual(ResultType.Error, result.Result);
            Assert.IsNull(result.ReturnedObject);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual($"Ingredient with name {newIngredient.Name} already exists in database", result.Errors.First());
        }

        [TestMethod]
        public async Task DeleteExistingIngredient()
        {
            repoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<IngredientModel, bool>>>(), null)).Returns(GetFakeIngredientByName(expectedIngredients[0].Name));
            repoMock.Setup(x => x.DeleteAsync(expectedIngredients[0])).Returns(Task.FromResult(true));

            uowMock.Setup(x => x.Ingredients).Returns(repoMock.Object);

            IIngredientService ingredientService = new IngredientService(uowMock.Object);

            IServiceResult deleteResult = await ingredientService.DeleteAsync(expectedIngredients[0].Id);

            Assert.AreEqual(ResultType.Deleted, deleteResult.Result);
            Assert.IsNull(deleteResult.Errors);
        }

        [TestMethod]
        public async Task DeleteNotExistingIngredient()
        {
            IngredientModel newIngredient = new IngredientModel
            {
                Id = 3,
                Name = "Tomato"
            };

            repoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<IngredientModel, bool>>>(), null)).Returns(GetFakeIngredientByName(newIngredient.Name));

            uowMock.Setup(x => x.Ingredients).Returns(repoMock.Object);

            IIngredientService ingredientService = new IngredientService(uowMock.Object);

            IServiceResult deleteResult = await ingredientService.DeleteAsync(newIngredient.Id);

            Assert.AreEqual(ResultType.Error, deleteResult.Result);
            Assert.IsNotNull(deleteResult.Errors);
            Assert.AreEqual($"Ingredient with id {newIngredient.Id} was not found in database", deleteResult.Errors.First());
        }

        [TestMethod]
        public async Task UpdatePrice()
        {
            int idOfIngredient = 1;
            PizzaModel pizza = (await GetFakePizzas()).First();
            IngredientPriceModel newPrice = new IngredientPriceModel
            {
                Id = 1,
                Size = SizeEnum.Small,
                Price = 5.50M
            };

            Mock<IFoodOrderRepository<PizzaModel>> pizzaRepoMock = new Mock<IFoodOrderRepository<PizzaModel>>();

            repoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<IngredientModel, bool>>>(),
                                           It.IsAny<Func<IQueryable<IngredientModel>, IIncludableQueryable<IngredientModel, object>>>())).
                                           Returns(GetFakeIngredientById(idOfIngredient));

            repoMock.Setup(x => x.UpdateAsync(expectedIngredients[idOfIngredient])).Returns(Task.FromResult(true));
  
            pizzaRepoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<PizzaModel, bool>>>(),
                                           It.IsAny<Func<IQueryable<PizzaModel>, IIncludableQueryable<PizzaModel, object>>>())).
                                           Returns(GetFakePizzas());
          
            pizzaRepoMock.Setup(x => x.UpdateAsync(pizza)).Returns(Task.FromResult(true));


            uowMock.Setup(x => x.Ingredients).Returns(repoMock.Object);
            uowMock.Setup(x => x.SaveChangesAsync());
            uowMock.Setup(x => x.Pizzas).Returns(pizzaRepoMock.Object);

            IIngredientService ingredientService = new IngredientService(uowMock.Object);

            IServiceResult<IngredientModel> result = await ingredientService.UpdatePriceAsync(newPrice, idOfIngredient);
            
            uowMock.Verify(x => x.SaveChangesAsync(), Times.Exactly(2));

            Assert.AreEqual(ResultType.Edited, result.Result);
            Assert.IsNotNull(result.ReturnedObject);
            Assert.AreEqual(newPrice.Price, result.ReturnedObject.Prices.Where(x => x.Size == SizeEnum.Small).SingleOrDefault().Price);
        }


        private async Task<IEnumerable<IngredientModel>> GetFakeIngredientById(int id)
        {
            return expectedIngredients.Where(x => x.Id == id);
        }

        private async Task<IEnumerable<IngredientModel>> GetFakeIngredientByName(string name)
        {
            return expectedIngredients.Where(x => x.Name == name);
        }

        private async Task<IEnumerable<IngredientModel>> GetFakeIngredients()
        {
            return expectedIngredients;
        }

        private async Task<IEnumerable<PizzaModel>> GetFakePizzas()
        {
            PizzaModel pizza = new PizzaModel
            {
                Id = 1,
                Name = "TestPizza",
                PizzaIngredients = new List<PizzaIngredientsModel>
                {
                    new PizzaIngredientsModel
                    {
                        PizzaId = 1,
                        IngredientId = 1,
                        Ingredient = new IngredientModel
                        {
                            Id = 1,
                            Name = "ham",
                            Prices = new List<IngredientPriceModel>
                            {
                                new IngredientPriceModel
                                {
                                    Id = 1,
                                    IngredientId = 1,
                                    Size = SizeEnum.Small,
                                    Price = 2.00M
                                },
                                new IngredientPriceModel
                                {
                                    Id = 2,
                                    IngredientId = 1,
                                    Size = SizeEnum.Big,
                                    Price = 5.00M
                                },
                            }
                        }
                    },
                    new PizzaIngredientsModel
                    {
                        PizzaId = 1,
                        IngredientId = 2,
                        Ingredient = new IngredientModel
                        {
                            Id = 2,
                            Name = "mushrooms",
                            Prices = new List<IngredientPriceModel>
                            {
                                new IngredientPriceModel
                                {
                                    Id = 3,
                                    IngredientId = 2,
                                    Size = SizeEnum.Small,
                                    Price = 3.00M
                                },
                                new IngredientPriceModel
                                {
                                    Id = 4,
                                    IngredientId = 2,
                                    Size = SizeEnum.Big,
                                    Price = 5.50M
                                },
                            }
                        }
                    }
                },
                PizzaStarters = new List<PizzaStarterModel>
                {
                    new PizzaStarterModel
                    {
                        PizzaId = 1,
                        StarterId = 1,
                        Starter = new StarterModel
                        {
                            Id = 1,
                            Name = "Starter1",
                            Size = SizeEnum.Small,
                            Price = 15.00M
                        }
                    },
                    new PizzaStarterModel
                    {
                        PizzaId = 1,
                        StarterId = 2,
                        Starter = new StarterModel
                        {
                            Id = 2,
                            Name = "Starter2",
                            Size = SizeEnum.Big,
                            Price = 25.00M
                        }
                    }
                }
            };
            
            return new List<PizzaModel> { pizza };
        }
    }
}
