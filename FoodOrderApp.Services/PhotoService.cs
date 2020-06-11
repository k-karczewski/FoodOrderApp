﻿using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.Services.ServiceResults;
using FoodOrderApp.Interfaces.UnitOfWork;
using FoodOrderApp.Models.Dtos;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.PhotoModels;
using FoodOrderApp.Services.ServiceResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderApp.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Cloudinary _cloudinary;

        public PhotoService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;

            Account account = new Account(
                _configuration.GetSection("CloudinarySettings").GetSection("CloudName").Value,
                _configuration.GetSection("CloudinarySettings").GetSection("ApiKey").Value,
                _configuration.GetSection("CloudinarySettings").GetSection("ApiSecret").Value);

            _cloudinary = new Cloudinary(account);
        }

        public async Task<IServiceResult> AddPizzaPhotoAsync(PhotoToCreateDto newPhoto)
        {
            try
            {
                if (newPhoto.PhotoInBytes.Length > 0)
                {
                    PizzaModel photoRecipent = (await _unitOfWork.Pizzas.GetByExpressionAsync(x => x.Id == newPhoto.PizzaId, i => i.Include(p => p.Photo))).SingleOrDefault();

                    // replace old photo if exists
                    if(photoRecipent.Photo != null)
                    {
                        bool deletionResult = await DeletePhotoAsync(photoRecipent.Photo);

                        if(deletionResult == false)
                        {
                            throw new Exception("Error during removing of photo from repo or cloud");
                        }
                    }

                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription($"Photo of {photoRecipent.Name} pizza", new MemoryStream(newPhoto.PhotoInBytes))
                    };

                    ImageUploadResult response = await _cloudinary.UploadAsync(uploadParams);

                    if(response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        PhotoModel photo = new PhotoModel
                        {
                            PizzaId = newPhoto.PizzaId,
                            PublicId = response.PublicId,
                            Url = response.Url.ToString(),
                            Pizza = photoRecipent
                        };

                        await _unitOfWork.Photos.CreateAsync(photo);
                        await _unitOfWork.SaveChangesAsync();

                        return new ServiceResult(ResultType.Correct);
                    }

                    throw new Exception("Error during addition of pizza photo");
                }

                throw new Exception("Photo has not been received");

            }
            catch (Exception e)
            {
                return new ServiceResult(ResultType.Error, new List<string> { e.Message });
            }

        }

        public async Task<IServiceResult> DeletePizzaPhotoAsync(int pizzaId)
        {
            try
            {
                PizzaModel photoRecipent = (await _unitOfWork.Pizzas.GetByExpressionAsync(x => x.Id == pizzaId, i => i.Include(p => p.Photo))).SingleOrDefault();

                if (photoRecipent.Photo != null)
                {
                    bool deletionResult = await DeletePhotoAsync(photoRecipent.Photo);

                    if (deletionResult == false)
                    {
                        throw new Exception("Error during removing of photo from repo or cloud");
                    }

                    return new ServiceResult(ResultType.Deleted);
                }

                throw new Exception($"Pizza with id {pizzaId} does not exist or does not have any photo");
            }
            catch (Exception e)
            {
                return new ServiceResult(ResultType.Error, new List<string> { e.Message });
            }
        }


        private async Task<bool> DeletePhotoAsync(PhotoModel photo)
        {
            try
            {
                await _cloudinary.DeleteResourcesAsync(photo.PublicId);
                await _unitOfWork.Photos.DeleteAsync(photo);

                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
