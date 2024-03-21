using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using webbanhangmvc.Models;
using webbanhangmvc.ViewModels;
using X.PagedList;

namespace webbanhangmvc.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    public class HomeAdminController : Controller
    {
        QlbanVaLiContext db = new QlbanVaLiContext();
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                ViewBag.UserName = HttpContext.Session.GetString("Username");
                ViewBag.ShowLogin = false; // Ẩn nút đăng nhập
            }
            else
            {
                ViewBag.ShowLogin = true; // Hiển thị nút đăng nhập
                return RedirectToAction("LoginAdmin", "HomeAdmin"); // Chuyển hướng người dùng đến trang đăng nhập nếu không phải là admin
            }
            return View();
        }

        [Route("danhmucsanpham")]
        public IActionResult DanhMucSanPham(int? page)
        {
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listsanpham = db.TDanhMucSps.AsNoTracking().OrderBy(x => x.TenSp);
            PagedList<TDanhMucSp> lst = new PagedList<TDanhMucSp>(listsanpham, pageNumber, pageSize);
            if (HttpContext.Session.GetString("Username") == null)
            {
                // Người dùng đã đăng nhập, chuyển hướng đến trang chính
                return RedirectToAction("LoginAdmin", "HomeAdmin");
            }
            return View(lst);
        }
        [Route("ThemSanPhamMoi")]
        [HttpGet]
        public IActionResult ThemSanPhamMoi()
        {
            ViewBag.MaChatLieu = new SelectList(db.TChatLieus.ToList(),"MaChatLieu","ChatLieu");
            ViewBag.MaHangSx = new SelectList(db.THangSxes.ToList(), "MaHangSx", "HangSx");
            ViewBag.MaNuocSx = new SelectList(db.TQuocGia.ToList(), "MaNuoc", "TenNuoc");
            ViewBag.MaLoai = new SelectList(db.TLoaiSps.ToList(), "MaLoai", "Loai");
            ViewBag.MaDt = new SelectList(db.TLoaiDts.ToList(), "MaDt", "TenLoai");

            return View();
        }
        [Route("ThemSanPhamMoi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemSanPhamMoi(TDanhMucSp sanPham)
        {
            if(ModelState.IsValid)
            {
                db.TDanhMucSps.Add(sanPham);
                db.SaveChanges();
                TempData["Message"] = "Thêm Mới Thành Công";
                return RedirectToAction("DanhMucSanPham");
            }
            return View(sanPham);
        }
        [Route("SuaSanPham")]
        [HttpGet]
        public IActionResult SuaSanPham(string maSanPham)
        {
            ViewBag.MaChatLieu = new SelectList(db.TChatLieus.ToList(), "MaChatLieu", "ChatLieu");
            ViewBag.MaHangSx = new SelectList(db.THangSxes.ToList(), "MaHangSx", "HangSx");
            ViewBag.MaNuocSx = new SelectList(db.TQuocGia.ToList(), "MaNuoc", "TenNuoc");
            ViewBag.MaLoai = new SelectList(db.TLoaiSps.ToList(), "MaLoai", "Loai");
            ViewBag.MaDt = new SelectList(db.TLoaiDts.ToList(), "MaDt", "TenLoai");
            var sanPham = db.TDanhMucSps.Find(maSanPham);
            return View(sanPham);
        }
        [Route("SuaSanPham")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaSanPham(TDanhMucSp sanPham)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges(); 
                TempData["Message"] = "Sửa Thành Công";
                return RedirectToAction("DanhMucSanPham","Homeadmin");
            }
            return View(sanPham);
        }
        [Route("XoaSanPham")]
        [HttpGet]
        public IActionResult XoaSanPham(string maSanPham)
        {
            TempData["Message"] = "";
            var chitetSanPham = db.TChiTietSanPhams.Where(x => x.MaSp == maSanPham).ToList();
            if(chitetSanPham.Count()>0)
            {
                TempData["Message"] = "Không Xóa Được Sản Phẩm Này";
                return RedirectToAction("DanhMucSanPham","Homeadmin");
            }
            var anhSanPham = db.TAnhSps.Where(x => x.MaSp==maSanPham);
            if (anhSanPham.Any()) db.RemoveRange(anhSanPham);
            db.Remove(db.TDanhMucSps.Find(maSanPham));
            db.SaveChanges();
            TempData["Message"] = "Sản Phẩm Đã Được Xóa";
            return RedirectToAction("DanhMucSanPham", "Homeadmin");
        }

        [Route("DonHang")]
        public IActionResult DonHang()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                // Người dùng đã đăng nhập, chuyển hướng đến trang chính
                return RedirectToAction("LoginAdmin", "HomeAdmin");
            }
            var orders = db.DonHangs.Where(o => !string.IsNullOrEmpty(o.TenSp)).ToList();
            return View(orders);
        }

        [Route("SuaDonHang")]
        [HttpGet]
        public IActionResult SuaDonHang(int? Mahoadon)
        {
            var donHang = db.DonHangs.Find(Mahoadon);
            return View(donHang);
        }

        [Route("SuaDonHang")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaDonHang(DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(donHang).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Sửa Thành Công";
                return RedirectToAction("DonHang", "Homeadmin");
            }
            return View(donHang);
        }

        [Route("XoaDonHang")]
        [HttpGet]
        public IActionResult XoaDonHang(int Mahoadon)
        {
            TempData["Message"] = "";
            //var donhang = db.DonHangs.Where(x => x.MaHoaDon == Mahoadon).ToList();
            //if (donhang.Count() > 0)
            //{
            //    TempData["Message"] = "Không Xóa Được Sản Phẩm Này";
            //    return RedirectToAction("DonHang", "Homeadmin");
            //}
            var sanpham = db.DonHangs.Where(x => x.MaHoaDon == Mahoadon);
            if (sanpham.Any()) db.RemoveRange(sanpham);
            db.Remove(db.DonHangs.Find(Mahoadon));
            db.SaveChanges();
            TempData["Message"] = "Sản Phẩm Đã Được Xóa";
            return RedirectToAction("DonHang", "Homeadmin");
        }

        [HttpGet]
        public IActionResult LoginAdmin()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                return RedirectToAction("Index", "HomeAdmin");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LoginAdmin(TUserNv use)
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                // Người dùng đã đăng nhập, chuyển hướng đến trang chính
                return RedirectToAction("Index", "HomeAdmin");
            }

            var t = db.TUserNvs.SingleOrDefault(y=>y.Username == use.Username && y.Password==use.Password);

            if (t != null)
            {
                HttpContext.Session.SetString("Username", t.Username);
                return RedirectToAction("Index", "HomeAdmin");
            }

            // Đăng nhập không thành công, hiển thị lại trang đăng nhập
            ModelState.AddModelError(string.Empty, "Tên người dùng hoặc mật khẩu không đúng");
            return View(use);
        }
        [Route("LogoutAdmin")]
        public IActionResult LogoutAdmin()
        {
            HttpContext.Session.Clear(); // Xóa tất cả các session
            return RedirectToAction("Index", "Homeadmin"); // Chuyển hướng về trang đăng nhập
        }

        [Route("QuanLyBaiDang")]
        public IActionResult QuanLyBaiDang()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                // Người dùng đã đăng nhập, chuyển hướng đến trang chính
                return RedirectToAction("LoginAdmin", "HomeAdmin");
            }
            var posts = db.Posts.ToList();
            return View(posts);
        }

        [Route("TaoBaiDang")]
        [HttpGet]
        public IActionResult TaoBaiDang()
        {
            return View();
        }

        [Route("TaoBaiDang")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TaoBaiDang(Post post)
        {
            if (ModelState.IsValid)
            {
                post.CreatedAt = DateTime.Now; // Thêm ngày tạo bài đăng
                db.Posts.Add(post);
                db.SaveChanges();
                TempData["Message"] = "Tạo bài đăng mới thành công";
                return RedirectToAction("QuanLyBaiDang");
            }
            return View(post);
        }
        [Route("XoaBaiDang")]
        [HttpGet]
        public IActionResult XoaBaiDang(int id)
        {
            //var donhang = db.DonHangs.Where(x => x.MaHoaDon == Mahoadon).ToList();
            //if (donhang.Count() > 0)
            //{
            //    TempData["Message"] = "Không Xóa Được Sản Phẩm Này";
            //    return RedirectToAction("DonHang", "Homeadmin");
            //}
            var baidang = db.Posts.Where(x => x.Id == id);
            if (baidang.Any()) db.RemoveRange(baidang);
            db.Remove(db.Posts.Find(id));
            db.SaveChanges();
            //TempData["Message"] = "Đã Được Xóa";
            return RedirectToAction("QuanLyBaiDang", "Homeadmin");
        }
        [Route("SuaBaiDang")]
        [HttpGet]
        public IActionResult SuaBaiDang(int? Id)
        {
            var post = db.Posts.Find(Id);
            return View(post);
        }

        [Route("SuaBaiDang")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaBaiDang(Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("QuanLyBaiDang", "Homeadmin");
            }
            return View(post);
        }
        [Route("NguoiDung")]
        public IActionResult NguoiDung()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                // Người dùng đã đăng nhập, chuyển hướng đến trang chính
                return RedirectToAction("LoginAdmin", "HomeAdmin");
            }
            var users = db.TUserKhs.ToList();
            return View(users);
        }
        [Route("ThemNguoiDung")]
        [HttpGet]
        public IActionResult ThemNguoiDung()
        {
            return View();
        }
        [Route("ThemNguoiDung")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemNguoiDung(TUserKh user)
        {
            if (ModelState.IsValid)
            {
                db.TUserKhs.Add(user);
                db.SaveChanges();
                return RedirectToAction("NguoiDung");
            }
            return View(user);
        }

        [Route("XoaNguoiDung")]
        [HttpGet]
        public IActionResult XoaNguoiDung(string id)
        {
            var baidang = db.TUserKhs.Where(x => x.UsernameKh == id);
            if (baidang.Any()) db.RemoveRange(baidang);
            db.Remove(db.TUserKhs.Find(id));
            db.SaveChanges();
            //TempData["Message"] = "Đã Được Xóa";
            return RedirectToAction("NguoiDung", "Homeadmin");
        }
        [Route("SuaNguoiDung")]
        [HttpGet]
        public IActionResult SuaNguoiDung(string? Id)
        {
            var sa = db.TUserKhs.Find(Id);
            return View(sa);
        }

        [Route("SuaNguoiDung")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaNguoiDung(TUserKh post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("NguoiDung", "Homeadmin");
            }
            return View(post);
        }

        [Route("NhanVien")]
        public IActionResult NhanVien()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                // Người dùng đã đăng nhập, chuyển hướng đến trang chính
                return RedirectToAction("LoginAdmin", "HomeAdmin");
            }
            var users = db.TUserNvs.ToList();
            return View(users);
        } 
        [Route("ThemNhanVien")]
        [HttpGet]
        public IActionResult ThemNhanVien()
        {
            return View();
        }
        [Route("ThemNhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemNhanVien(TUserNv user)
        {
            if (ModelState.IsValid)
            {
                db.TUserNvs.Add(user);
                db.SaveChanges();
                return RedirectToAction("NhanVien");
            }
            return View(user);
        }

        [Route("XoaNhanVien")]
        [HttpGet]
        public IActionResult XoaNhanVien(string id)
        {
            var baidang = db.TUserNvs.Where(x => x.Username == id);
            if (baidang.Any()) db.RemoveRange(baidang);
            db.Remove(db.TUserNvs.Find(id));
            db.SaveChanges();
            //TempData["Message"] = "Đã Được Xóa";
            return RedirectToAction("NhanVien", "Homeadmin");
        }
        [Route("SuaNhanVien")]
        [HttpGet]
        public IActionResult SuaNhanVien(string? Id)
        {
            var sa = db.TUserNvs.Find(Id);
            return View(sa);
        }

        [Route("SuaNhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaNhanVien(TUserNv post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("NhanVien", "Homeadmin");
            }
            return View(post);
        }
    }
}
