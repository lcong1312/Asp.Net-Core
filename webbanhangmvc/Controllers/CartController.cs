using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using webbanhangmvc.Infrastructure;
using webbanhangmvc.Models;
using webbanhangmvc.Casso;
using webbanhangmvc.Services;
using webbanhangmvc.ViewModels;
using static webbanhangmvc.Models.Cart;
using Microsoft.EntityFrameworkCore;
using PagedList;

namespace webbanhangmvc.Controllers
{
    public class CartController : Controller
    {
        private readonly QlbanVaLiContext _context;
        private readonly IVnPayServices _vnPayServices;
        private readonly IHttpClientFactory _clientFactory;
        private readonly VietQRSettings _vietQRSettings;
        private readonly ICassoService _cassoService;

        public CartController(QlbanVaLiContext context, IVnPayServices vnPayServices, IHttpClientFactory clientFactory, IOptions<VietQRSettings> vietQRSettings, ICassoService cassoService)
        {
            _context = context;
            _vnPayServices = vnPayServices;
            _clientFactory = clientFactory;
            _vietQRSettings = vietQRSettings.Value;
            _cassoService = cassoService;
        }

        public IActionResult AddToCart(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return RedirectToAction("Cart");
            }
            TDanhMucSp product = _context.TDanhMucSps.FirstOrDefault(p => p.MaSp == productId);
            if (product != null)
            {
                Cart cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
                cart.AddItem(product, 1);
                HttpContext.Session.SetJson("cart", cart);
            }
            return RedirectToAction("cart");
        }

        public IActionResult UpDateCart(string productId)
        {
            TDanhMucSp product = _context.TDanhMucSps.FirstOrDefault(p => p.MaSp == productId);
            if (product != null)
            {
                Cart cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
                cart.AddItem(product, -1);
                HttpContext.Session.SetJson("cart", cart);
            }
            return RedirectToAction("Cart");
        }

        public IActionResult RemoveFromCart(string productId)
        {
            TDanhMucSp product = _context.TDanhMucSps.FirstOrDefault(p => p.MaSp == productId);
            if (product != null)
            {
                Cart cart = HttpContext.Session.GetJson<Cart>("cart");
                if (cart != null)
                {
                    cart.RemoveLine(product);
                    HttpContext.Session.SetJson("cart", cart);
                }
            }
            return RedirectToAction("Cart");
        }

        public IActionResult Cart()
        {
            Cart cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            string UserName = HttpContext.Session.GetString("UserName");

            bool isLoggedIn = !string.IsNullOrEmpty(UserName);

            if (isLoggedIn)
            {
                ViewBag.UserName = UserName; // Lưu tên người dùng vào ViewBag
                ViewBag.ShowLogin = false; // Không hiển thị nút đăng nhập
            }
            else
            {
                ViewBag.ShowLogin = true; // Hiển thị nút đăng nhập
            }

            return View(cart);
        }

        public async Task<IActionResult> Checkout(string name, string sp, string address, string phone, string paymentMethod,string tinhtrang)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.ShowLogin = false; // Ẩn nút đăng nhập
            }
            else
            {
                ViewBag.ShowLogin = true; // Hiển thị nút đăng nhập
            }
            Cart cart = HttpContext.Session.GetJson<Cart>("cart");

            if (cart == null || cart.lines.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Acces");
            }

            string TenSp = string.Join(",", cart.Getsp());
            string TK = HttpContext.Session.GetString("UserName");

            DonHang newOrder = new DonHang
            {
                TenKh = cart.GetProductName(name),
                TenSp = TenSp,
                NgayBan = DateTime.Now, // Ngày đặt hàng
                DiaChi = cart.Diachiship(address), // Địa chỉ giao hàng
                Soluong = cart.GetQuantity(),
                Phuongthuctt = cart.Phuongthuctt(paymentMethod), // Phương thức thanh toán
                Xulydonhang = "Đang xử lý",
                ThanhTien = cart.ComputeToTalValue(),// Tổng tiền đơn hàng
                UsernameKh = TK,
                TinhTrang=tinhtrang
                //TransactionId = Guid.NewGuid().ToString(),
                //bankSubAccId = "0334333196",
            };
            _context.DonHangs.Add(newOrder);
            _context.SaveChanges();


            //string transactionId = newOrder.TransactionId;

            // Tạo mã QR
            var payload = new
            {
                accountNo = "0334333196", // Số tài khoản ngân hàng
                accountName = "LE VIET CONG", // Tên tài khoản ngân hàng
                acqId = 970422, // Mã định danh ngân hàng
                amount = newOrder.ThanhTien, // Số tiền chuyển
                addInfo = "Thanh toan don hang " + newOrder.MaHoaDon, // Nội dung chuyển tiền
                format = "text", // Định dạng trả về
                template = "compact2", // Mẫu QR trả về
                //transactionId= transactionId
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
                ViewBag.QrDataURL = qrDataURL;
            }
            else
            {
                ViewBag.QrDataURL = null;
                ViewBag.Error = "Lỗi khi tạo mã QR";
            }

            // Qua VNPAY
            if (paymentMethod == "Qua Ví VnPay")
            {
                var vnPaymodel = new VnPaymentRequestModel
                {
                    Amount = (double)newOrder.ThanhTien,
                    CreatedDate = DateTime.Now,
                    Description = $"{newOrder.TenKh}{phone}",
                    FullName = newOrder.TenKh,
                    OrderId = newOrder.MaHoaDon
                };
                return Redirect(_vnPayServices.CreatePaymentUrl(HttpContext, vnPaymodel));
            }

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> CreateQrCode(string name, string address, string phone, string paymentMethod)
        {
            Cart cart = HttpContext.Session.GetJson<Cart>("cart");

            if (cart == null || cart.lines.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string TenSp = string.Join(",", cart.Getsp());
            string TK = HttpContext.Session.GetString("UserName");


            DonHang newOrder = new DonHang
            {
                TenKh = cart.GetProductName(name),
                TenSp = TenSp,
                NgayBan = DateTime.Now, // Ngày đặt hàng
                DiaChi = cart.Diachiship(address), // Địa chỉ giao hàng
                Soluong = cart.GetQuantity(),
                Phuongthuctt = cart.Phuongthuctt(paymentMethod), // Phương thức thanh toán
                Xulydonhang = "Đang xử lý",
                ThanhTien = cart.ComputeToTalValue(), // Tổng tiền đơn hàng
                TransactionId = GenerateTransactionId(9),
                bankSubAccId = "0334333196",
                UsernameKh=TK,
                TinhTrang="Chưa Thanh Toán"
            };
            _context.DonHangs.Add(newOrder);
            _context.SaveChanges();

            string transactionId = newOrder.TransactionId;
            decimal amount = (decimal)newOrder.ThanhTien;
            HttpContext.Session.SetString("TransactionId", newOrder.TransactionId);
            // Tạo mã QR
            var payload = new
            {
                accountNo = "0334333196", // Số tài khoản ngân hàng
                accountName = "LE VIET CONG", // Tên tài khoản ngân hàng
                acqId = 970422, // Mã định danh ngân hàng
                amount = amount, // Số tiền chuyển
                addInfo = transactionId, // Nội dung chuyển tiền
                format = "text", // Định dạng trả về
                template = "compact2", // Mẫu QR trả về
                transactionId = transactionId
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
                ViewBag.QrDataURL = qrDataURL;
                ViewBag.Amount = amount;
                ViewBag.AddInfo = transactionId;
                return View("qrDataURL"); // Chuyển đến view để hiển thị mã QR
            }
            else
            {
                ViewBag.QrDataURL = null;
                ViewBag.Error = "Lỗi khi tạo mã QR";
                return View("qrDataURL"); // Chuyển đến view để hiển thị lỗi
            }
        }

        private static string GenerateTransactionId(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                                        .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public IActionResult RemoveCartSession()
        {
            HttpContext.Session.Remove("cart");
            return Ok();
        }

        public IActionResult PaymentSuccess()
        {
            HttpContext.Session.Remove("cart");
            return View();
        }

        public IActionResult PaymentFail()
        {
            return View();
        }

        public IActionResult PaymentCallBack()
        {
            var response = _vnPayServices.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response?.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }

            TempData["Message"] = "Thanh toán VNPay thành công";

            string vnp_OrderInfo = Request.Query["vnp_OrderInfo"].ToString();
            if (int.TryParse(vnp_OrderInfo, out int mahoadon))
            {
                var order = _context.DonHangs.FirstOrDefault(o => o.MaHoaDon == mahoadon);
                if (order != null)
                {
                    order.TinhTrang = "Đã Thanh Toán";
                    _context.SaveChanges();
                }
            }
            HttpContext.Session.Remove("cart");
            return RedirectToAction("PaymentSuccess");
        }

        public IActionResult qrDataURL()
        {
            return View();
        }
        public IActionResult Success()
        {
            string transactionId = HttpContext.Session.GetString("TransactionId");

            if (!string.IsNullOrEmpty(transactionId))
            {
                // Tìm đơn hàng theo TransactionId
                var order = _context.DonHangs.FirstOrDefault(o => o.TransactionId == transactionId);
                if (order != null)
                {
                    // Cập nhật tình trạng đơn hàng thành "Đã Thanh Toán"
                    order.Xulydonhang = "Đang xử lý";
                    order.TinhTrang = "Đã Thanh Toán";
                    _context.SaveChanges();
                }
            }
            HttpContext.Session.Remove("cart");
            HttpContext.Session.Remove("TransactionId");
            return View();
        }
        public IActionResult HuyDon(int mahoadon)
        {
            var order = _context.DonHangs.FirstOrDefault(o => o.MaHoaDon == mahoadon);
            if (order == null)
            {
                return NotFound();
            }
            order.Xulydonhang = "Hủy Đơn";
            _context.SaveChanges();
            return RedirectToAction("KiemTraDon");
        }
        public IActionResult KiemTraDon(int? page)
        {
            string use = HttpContext.Session.GetString("UserName");
            if (use == null)
            {
                return RedirectToAction("Login", "Acces");
            }
            var orders = _context.DonHangs
                .Where(o => o.TenKh != null && o.TenSp != null && o.UsernameKh == use && o.TinhTrang != null)
                .Select(o => new OrderViewModel
                {
                    MaHoaDon = o.MaHoaDon,
                    TenKh = o.TenKh,
                    TenSp = o.TenSp,
                    NgayBan = (DateTime)o.NgayBan,
                    DiaChi = o.DiaChi,
                    Soluong = o.Soluong,
                    Phuongthuctt = o.Phuongthuctt,
                    Xylydonhang=o.Xulydonhang,
                    ThanhTien = (decimal)o.ThanhTien,
                    UsernameKh = o.UsernameKh,
                    TinhTrang = o.TinhTrang
                })
                .ToList();

            return View(orders); 
        }
    }
}
