using AutoMapper;
using StoreSystem.Core.Entities;
using StoreSystem.Application.Contract.StockMovementContract.req;
using StoreSystem.Application.Contract.StockMovementContract.res;

namespace StoreSystem.Application.Contract.Common.Mappings
{
    public class StockMovementProfile : Profile
    {
        public StockMovementProfile()
        {
            CreateMap<StockMovement, StockMovementRes>().ReverseMap();
            CreateMap<StockMovement, StockMovementReq>().ReverseMap();
        }
    }
}
