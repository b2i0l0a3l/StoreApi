using AutoMapper;
using StoreSystem.Application.Contract.EmployeeContract.Req;
using StoreSystem.Application.Contract.EmployeeContract.Res;
using StoreSystem.Core.Entities;

namespace StoreSystem.Application.Contract.Common.Mappings
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<StoreEmployee, EmployeeRes>().ReverseMap();
            CreateMap<StoreEmployee, EmployeeReq>().ReverseMap();
        }
    }
}
