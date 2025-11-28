using AutoMapper;
using StoreSystem.Core.Entities;
using StoreSystem.Application.Contract.InventoryContract.req;
using StoreSystem.Application.Contract.InventoryContract.res;

namespace StoreSystem.Application.Contract.Common.Mappings
{
    public class InventoryProfile : Profile
    {
        public InventoryProfile()
        {
            CreateMap<Inventory, InventoryRes>().ReverseMap();
            CreateMap<Inventory, InventoryReq>().ReverseMap();
        }
    }
}
