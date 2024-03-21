using Microsoft.AspNetCore.Mvc;
using webbanhangmvc.Models;
using Microsoft.AspNetCore.Http;

namespace webbanhangmvc.Controllers
{
    public class AccesController : Controller
    {
        private readonly QlbanVaLiContext db;

        public AccesController(QlbanVaLiContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(TUserKh user)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                // Người dùng đã đăng nhập, chuyển hướng đến trang chính
                return RedirectToAction("Index", "Home");
            }

            var u = db.TUserKhs.SingleOrDefault(x => x.UsernameKh == user.UsernameKh && x.Password == user.Password);

            if (u != null)
            {
                HttpContext.Session.SetString("UserName", u.UsernameKh);
                return RedirectToAction("Index", "Home");
            }

            // Đăng nhập không thành công, hiển thị lại trang đăng nhập
            return View();
        }

        //public IActionResult Logout()
        //{
        //    HttpContext.Session.Clear();
        //    return RedirectToAction("Login", "Acces");
        //}

        [HttpGet] // Đảm bảo đây là yêu cầu GET
        public IActionResult Register()
        {
            // Nếu đã đăng nhập, chuyển hướng đến trang chính
            if (HttpContext.Session.GetString("UserName") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(TUserKh user)
        {
            // Kiểm tra xem đã đăng nhập hay chưa
            if (HttpContext.Session.GetString("UserName") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra xem tên người dùng đã tồn tại chưa
                if (db.TUserKhs.Any(u => u.UsernameKh == user.UsernameKh))
                {
                    ModelState.AddModelError("Username", "Tên người dùng đã tồn tại.");
                    return View(user);
                }

                db.TUserKhs.Add(user);
                db.SaveChanges();
                return RedirectToAction("Login", "Acces");
            }

            // Đăng ký không thành công, hiển thị lại trang đăng ký
            return View(user);
        }
    }
}