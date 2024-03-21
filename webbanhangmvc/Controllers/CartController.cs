using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using webbanhangmvc.Infrastructure;
using webbanhangmvc.Models;

namespace webbanhangmvc.Controllers
{
    public class CartController : Controller
    {
        private readonly QlbanVaLiContext _context;

        public CartController(QlbanVaLiContext context)
        {
            _context = context;
        }

        public IActionResult AddToCart(string productId)
        {
            TDanhMucSp product = _context.TDanhMucSps.FirstOrDefault(p => p.MaSp == productId);
            if (product != null)
            {
                Cart cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
                cart.AddItem(product, 1);
                HttpContext.Session.SetJson("cart", cart);
            }
            return RedirectToAction("Cart");
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

        public IActionResult Checkout(string name,string sp,string address, string paymentMethod)
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

            //Random random = new Random();
            //int randomNumber = random.Next();
            //Tạo đối tượng DonHang từ thông tin nhận được từ form
            DonHang newOrder = new DonHang

           {
               TenKh = cart.GetProductName(name),
                TenSp = cart.GetSp(sp),
                NgayBan = DateTime.Now, // Ngày đặt hàng
                DiaChi = cart.Diachiship(address), // Địa chỉ giao hàng
                Soluong = cart.GetQuantity(),
               Phuongthuctt = cart.Phuongthuctt(paymentMethod), // Phương thức thanh toán
                ThanhTien = cart.ComputeToTalValue() // Tổng tiền đơn hàng
            };

            //Thêm đối tượng mới vào DbContext và lưu vào CSDL
            _context.DonHangs.Add(newOrder);
            _context.SaveChanges();

            // Xóa giỏ hàng khỏi Session
            //HttpContext.Session.Remove("cart");

            // Chuyển hướng đến trang thông báo đặt hàng thành công (nếu có)
            return View(cart);
        }


        public IActionResult RemoveCartSession()
        {
            HttpContext.Session.Remove("cart");
            return Ok();
        }
    }
}
