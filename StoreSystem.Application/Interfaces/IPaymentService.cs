using StoreSystem.Core.common;
using System.Threading.Tasks;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.PaymentContract.Req;
using StoreSystem.Application.Contract.PaymentContract.Res;

namespace StoreSystem.Application.Interfaces
{

    public interface IPaymentService
    {
        Task<GeneralResponse<int>> RecordPaymentAsync(PaymentReq req);
        Task<GeneralResponse<PagedResult<PaymentRes>>> GetPaymentsByCustomerAsync(int customerId, int pageNumber, int pageSize);
        Task<GeneralResponse<PagedResult<PaymentRes>>> GetPaymentsBySupplierAsync(int supplierId, int pageNumber, int pageSize);
    }
}
