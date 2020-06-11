using FoodOrderApp.Interfaces.Repositories;
using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Interfaces.UnitOfWork;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.PhotoModels;
using FoodOrderApp.Services;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Tests.Services
{
    [TestClass]
    public class PhotoService_Tests
    {
        private Mock<IUnitOfWork> _uowMock;
        private Mock<IFoodOrderRepository<PizzaModel>> _pizzaRepoMock;
        private Mock<IFoodOrderRepository<PhotoModel>> _photoRepoMock;

        [TestInitialize]
        public void Initialize()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _pizzaRepoMock = new Mock<IFoodOrderRepository<PizzaModel>>();
            _photoRepoMock = new Mock<IFoodOrderRepository<PhotoModel>>();
        }



        [TestMethod]
        public async Task AddPhoto()
        {
            int expectedPizzaId = 1;
            byte[] testImage = File.ReadAllBytes(@"./../../../Services/img/test-img.jpg");

            PhotoToCreateDto newPhoto = new PhotoToCreateDto
            {
                PizzaId = expectedPizzaId,
                PhotoInBytes = testImage
            };

            _pizzaRepoMock.Setup(x => x.GetByExpressionAsync(It.IsAny<Expression<Func<PizzaModel, bool>>>(),
                                                       It.IsAny<Func<IQueryable<PizzaModel>, IIncludableQueryable<PizzaModel, object>>>())).Returns(GetPizzaExample(expectedPizzaId));

            _photoRepoMock.Setup(x => x.CreateAsync(It.IsAny<PhotoModel>())).Returns(Task.FromResult(true));

            _uowMock.Setup(x => x.Pizzas).Returns(_pizzaRepoMock.Object);
            _uowMock.Setup(x => x.Photos).Returns(_photoRepoMock.Object);
            _uowMock.Setup(x => x.SaveChangesAsync());

            var configuration = new ConfigurationBuilder().SetBasePath(Path.GetFullPath("./../../../../FoodOrderAppAPI")).AddJsonFile("appsettings.json").Build();

            IPhotoService photoService = new PhotoService(configuration, _uowMock.Object);
            IServiceResult result = await photoService.AddPizzaPhotoAsync(newPhoto);

            Assert.AreEqual(ResultType.Correct, result.Result);
        }

        private async Task<IEnumerable<PizzaModel>> GetPizzaExample(int id)
        {
            List<PizzaModel> pizzas = new List<PizzaModel>
            {
                new PizzaModel
                {
                    Id = 1,
                    Name = "test1",
                    Photo = null
                },
                new PizzaModel
                {
                    Id = 2,
                    Name = "test2",
                    Photo = new PhotoModel {}
                }
            };

            return pizzas.Where(x => x.Id == id);
        }
    }
}
