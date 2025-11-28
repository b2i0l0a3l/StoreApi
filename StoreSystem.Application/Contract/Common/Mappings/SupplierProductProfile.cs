using AutoMapper;
using StoreSystem.Application.Contract.SupplierProductContract.Req;
using StoreSystem.Application.Contract.SupplierProductContract.Res;
using StoreSystem.Core.Entities;

namespace StoreSystem.Application.Contract.Common.Mappings
{
    public class SupplierProductProfile : Profile
    {
        public SupplierProductProfile()
        {
            CreateMap<SupplierProduct, SupplierProductRes>().ReverseMap();
            CreateMap<SupplierProduct, SupplierProductReq>().ReverseMap();
        }
    }
}
