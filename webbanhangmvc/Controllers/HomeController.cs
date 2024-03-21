using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using webbanhangmvc.Models;
using webbanhangmvc.Models.Authentication;
using webbanhangmvc.ViewModels;
using X.PagedList;

namespace webbanhangmvc.Controllers
{

    public class HomeController : Controller
    {
        QlbanVaLiContext db = new QlbanVaLiContext();
        private readonly QlbanVaLiContext _context;

        public HomeController(QlbanVaLiContext context)
        {
            _context = context;
        }
        // [Authentication]
        public IActionResult Index(string maloai, int? page)
        {
            int pageSize = 8;
            int pageNumber = page ?? 1;
            IQueryable<TDanhMucSp> listsanpham;
            if (!string.IsNullOrEmpty(maloai))
            {
                listsanpham = _context.TDanhMucSps.Where(x => x.MaLoai == maloai).OrderBy(x => x.TenSp);
            }
            else
            {
                listsanpham = _context.TDanhMucSps.OrderBy(x => x.TenSp);
            }
            PagedList<TDanhMucSp> lst = new PagedList<TDanhMucSp>(listsanpham, pageNumber, pageSize);
            if (HttpContext.Session.GetString("UserName") != null)
            {
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.ShowLogin = false; // Ẩn nút đăng nhập
            }
            else
            {
                ViewBag.ShowLogin = true; // Hiển thị nút đăng nhập
            }
            ViewBag.CurrentPageNumber = pageNumber;
            ViewBag.SelectedCategory = maloai;
            return View(lst);
        }


        public IActionResult SanPhamTheoLoai(string maloai, int? page)
        {
            int pageSize = 8;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listsanpham = db.TDanhMucSps.Where(x => x.MaLoai == maloai).OrderBy(x => x.TenSp).ToList();
            PagedList<TDanhMucSp> lst = new PagedList<TDanhMucSp>(listsanpham, pageNumber, pageSize);
            ViewBag.maloai = maloai;
            return View(lst);
        }
        public IActionResult ChiTietSanPham(string maSp)
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
            var sanPham = db.TDanhMucSps.SingleOrDefault(x => x.MaSp == maSp);
            var anhSanPham = db.TAnhSps.Where(x => x.MaSp == maSp).ToList();
            ViewBag.anhSanPham = anhSanPham;
            return View(sanPham);
        }

        public IActionResult ProductDetail(string maSp, string UserName)
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
            var sanPham = db.TDanhMucSps.SingleOrDefault(x => x.MaSp == maSp);
            var anhSanPham = db.TAnhSps.Where(x => x.MaSp == maSp).ToList();
            var homeProductDetailViewModel = new HomeProductDetalViewModel { danhMucSp = sanPham, anhSp = anhSanPham };
            if (!string.IsNullOrEmpty(UserName))
            {
                ViewBag.UserName = UserName;
                ViewBag.ShowLogin = false; // Ẩn nút đăng nhập
            }
            else
            {
                ViewBag.ShowLogin = true; // Hiển thị nút đăng nhập
            }

            return View(homeProductDetailViewModel);
        }

        public IActionResult lienhe()
        {
            return View();
        }
        //public IActionResult Thongtin()
        //{
        //    // Kiểm tra xem người dùng đã đăng nhập chưa
        //    if (HttpContext.Session.GetString("UserName") != null)
        //    {
        //        // Lấy username của người dùng từ session
        //        string userName = HttpContext.Session.GetString("UserName");

        //        // Truy vấn cơ sở dữ liệu để lấy thông tin người dùng dựa trên username
        //        var user = _context.TUserKhs.FirstOrDefault(u => u.UsernameKh == userName);

        //        // Kiểm tra xem người dùng có tồn tại trong cơ sở dữ liệu hay không
        //        if (user != null)
        //        {
        //            // Chuyển dữ liệu người dùng sang view thông tin
        //            return View(user);
        //        }
        //        else
        //        {
        //            // Nếu không tìm thấy thông tin người dùng trong cơ sở dữ liệu, có thể xử lý tùy ý của bạn,
        //            // ví dụ: đăng xuất người dùng và chuyển hướng về trang chủ
        //            HttpContext.Session.Clear(); // Xóa tất cả các session
        //            return RedirectToAction("Index", "Home"); // Chuyển hướng về trang đăng nhập
        //        }
        //    }
        //    else
        //    {
        //        // Nếu người dùng chưa đăng nhập, có thể chuyển hướng hoặc xử lý khác tùy ý của bạn
        //        return RedirectToAction("Index", "Home");
        //    }
        //}


        //public IActionResult Giohang()
        //{
        //    return View();
        //}
        public async Task<ActionResult> RandomProductDetail()
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
            var randomProduct = await _context.TDanhMucSps.OrderBy(x => Guid.NewGuid()).FirstOrDefaultAsync();

            // Kiểm tra xem có sản phẩm ngẫu nhiên không
            if (randomProduct != null)
            {
                // Chuyển hướng đến trang chi tiết sản phẩm với mã sản phẩm của sản phẩm ngẫu nhiên
                return RedirectToAction("ProductDetail", "Home", new { maSp = randomProduct.MaSp });
            }
            else
            {
                // Xử lý nếu không có sản phẩm ngẫu nhiên
                return Content("Không có sản phẩm nào.");
            }
        }

        public IActionResult Tintuc(int page = 1)
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

            int maxRecentPosts = 5; // Số bài viết gần đây tối đa
            int pageSize = 1; // Số bài viết trên mỗi trang

            // Lấy 1 bài viết đầu tiên
            var singlePost = db.Posts.FirstOrDefault();

            // Lấy các bài viết gần đây
            var recentPosts = db.Posts
        .OrderByDescending(p => p.Id) // Sắp xếp từ số lớn đến nhỏ
        .Skip((page - 1) * maxRecentPosts)
        .Take(maxRecentPosts)
        .ToList();

            // Truyền dữ liệu vào view
            ViewBag.SinglePost = singlePost;
            ViewBag.RecentPosts = recentPosts;

            return View();
        }
        public IActionResult TourDuLich()
        {
            return View();
        }
        public IActionResult CamNang()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Xóa tất cả các session
            return RedirectToAction("Index", "Home"); // Chuyển hướng về trang đăng nhập
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}