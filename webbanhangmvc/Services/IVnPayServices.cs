using webbanhangmvc.ViewModels;

namespace webbanhangmvc.Services
{
    public interface IVnPayServices
    {
       
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collection);
    }
}
