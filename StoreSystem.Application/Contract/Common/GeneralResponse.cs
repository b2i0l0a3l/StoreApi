using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSystem.Application.Contract.Common
{
    public class GeneralResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public int StatusCode { get; set; }

        public GeneralResponse(bool isSuccess, string message, T? data, int statusCode)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
            StatusCode = statusCode;
        }

        /// <summary>
        ///     this method to return success response
        /// </summary>
        /// <param name="data">"Send Data For User"</param>
        /// <param name="message">Message For User</param>
        /// <param name="statusCode">Status Codes ex: 200 , 201, 400, 500</param>
        /// <returns></returns> <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static GeneralResponse<T> Success(T data, string message = "", int statusCode = 200)
        {
            return new GeneralResponse<T>(true, message, data, statusCode);
        }
        
        /// <summary>
        ///    ///     this method to return failure response
        /// </summary>
        /// <param name="message">Message for Client</param>
        /// <param name="statusCode">Status Codes ex: 200 , 201, 400, 500<</param>
        /// <returns></returns> <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static GeneralResponse<T> Failure(string message, int statusCode = 400)
        {
            return new GeneralResponse<T>(false, message, default, statusCode);
        }
    }
}