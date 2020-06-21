using FoodOrderApp.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Interfaces.Services.ServiceResults
{
    public interface IServiceResult
    {
        ResultType Result { get; set; }
        ICollection<string> Errors { get; set; }
    }

    public interface IServiceResult<TReturn>
    {
        ResultType Result { get; set; }
        TReturn ReturnedObject { get; set; }
        ICollection<string> Errors { get; set; }
    }
}
