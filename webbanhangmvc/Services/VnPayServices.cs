using System.Globalization;
using webbanhangmvc.Helpers;
using webbanhangmvc.ViewModels;

namespace webbanhangmvc.Services
{
    public class VnPayServices : IVnPayServices
    {
        private readonly IConfiguration _config;

        public VnPayServices(IConfiguration config)
        {
            _config = config;
        }

        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model)
        {
            var tick = DateTime.Now.Ticks.ToString();

            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamCurrentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            DateTime createdDateUtc = DateTime.SpecifyKind(vietnamCurrentTime, DateTimeKind.Utc);

            DateTime vietnamCreateDate = TimeZoneInfo.ConvertTimeFromUtc(createdDateUtc, vietnamTimeZone);
            DateTime vietnamExpireDate = vietnamCreateDate.AddMinutes(15);

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString()); // Số tiền thanh toán.

            // Ngày tạo giao dịch
            vnpay.AddRequestData("vnp_CreateDate", vietnamCreateDate.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture));

            // Thêm thời gian hết hạn giao dịch (ví dụ: 15 phút sau khi tạo)
            vnpay.AddRequestData("vnp_ExpireDate", vietnamExpireDate.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture));

            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", ""+model.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); // default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", tick); // Mã tham chiếu của giao dịch.

            var paymentUrl = vnpay.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);

            return paymentUrl;
        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);
            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false
                };
            }

            return new VnPaymentResponseModel
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = vnp_orderId.ToString(),
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode
            };
        }
    }
}
