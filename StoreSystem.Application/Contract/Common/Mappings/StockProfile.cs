using AutoMapper;
using StoreSystem.Core.Entities;
using StoreSystem.Application.Contract.StockContract.req;
using StoreSystem.Application.Contract.StockContract.res;

namespace StoreSystem.Application.Contract.Common.Mappings
{
    public class StockProfile : Profile
    {
        public StockProfile()
        {
            CreateMap<Stock, StockRes>().ReverseMap();
            CreateMap<Stock, StockReq>().ReverseMap();
        }
    }
}
