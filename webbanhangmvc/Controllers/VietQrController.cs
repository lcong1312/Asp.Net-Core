using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using webbanhangmvc.Infrastructure;
using webbanhangmvc.Models;

namespace webbanhangmvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VietQrController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly VietQRSettings _vietQRSettings;

        public VietQrController(IHttpClientFactory clientFactory, IOptions<VietQRSettings> vietQRSettings)
        {
            _clientFactory = clientFactory;
            _vietQRSettings = vietQRSettings.Value;
        }
        [HttpPost("TaoQr")]
        public async Task<IActionResult> GenerateVietQR()
        {
            // Thông tin tài khoản ngân hàng
            var payload = new
            {
                accountNo = "3600687160", // Số tài khoản ngân hàng
                accountName = "LE VIET CONG", // Tên tài khoản ngân hàng
                acqId = 970418, // Mã định danh ngân hàng (BIN code của BIDV)
                amount = 10000, // Số tiền chuyển
                addInfo = "Thanh toan don hang", // Nội dung chuyển tiền
                format = "text", // Định dạng trả về
                template = "compact" // Mẫu QR trả về
            };

            // Tạo HttpClient để gửi yêu cầu
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("x-client-id", _vietQRSettings.ClientId);
            client.DefaultRequestHeaders.Add("x-api-key", _vietQRSettings.ApiKey);

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api.vietqr.io/v2/generate", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonDocument.Parse(responseBody).RootElement;

                // Lấy mã QR dạng data URL từ response
                string qrDataURL = jsonResponse.GetProperty("data").GetProperty("qrDataURL").GetString();

                // Trả về mã QR code dưới dạng hình ảnh
                return Ok(new { qrDataURL });
            }
            else
            {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
        }
    }
}
