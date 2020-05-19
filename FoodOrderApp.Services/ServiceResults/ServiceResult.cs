using FoodOrderApp.Interfaces.Services.ServiceResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodOrderApp.Services.ServiceResults
{
    public class ServiceResult : IServiceResult
    {
        public virtual ResultType Result { get; set; }
        public virtual ICollection<string> Errors { get; set; } = null;

        public ServiceResult(ResultType result)
        {
            Result = result;
        }

        public ServiceResult(ResultType result, ICollection<string> errors)
        {
            Result = result;
            Errors = errors;
        }
    }

    public class ServiceResult<TReturn> : ServiceResult, IServiceResult<TReturn>
    {
        public TReturn ReturnedObject { get; set; }

        public ServiceResult(ResultType result, TReturn returnedObject) : base(result)
        {
            ReturnedObject = returnedObject;
        }

        public ServiceResult(ResultType result, ICollection<string> errors) : base(result, errors){ }
    }
}
