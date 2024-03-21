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
            if (HttpContext.Session.GetString("UserName") != null)
            {
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.ShowLogin = false; // Ẩn nút đăng nhập
            }
            else
            {
                ViewBag.ShowLogin = true; // Hiển thị nút đăng nhập
            }
            LogAcces();
            int pageSize = 8;
            int maxPagesToShow = 5; // Số trang tối đa để hiển thị

            int pageNumber = page ?? 1;
            IQueryable<TDanhMucSp> listsanpham;
            IQueryable<TDanhMucSp> listsanphamck;
            listsanphamck = _context.TDanhMucSps.Where(x => x.ChietKhau >= 20).OrderBy(x => x.TenSp);
            listsanpham = _context.TDanhMucSps.Where(x => x.ChietKhau < 20||x.ChietKhau==null).OrderBy(x => x.TenSp);
            if (!string.IsNullOrEmpty(maloai))
            {
                listsanphamck = listsanphamck.Where(x => x.MaLoai == maloai);
                listsanpham = listsanpham.Where(x => x.MaLoai == maloai);  
                //listsanpham = _context.TDanhMucSps.Where(x => x.MaLoai == maloai).OrderBy(x => x.TenSp);
            }
            //else
            //{
            //    listsanphamck = _context.TDanhMucSps.Where(x => x.ChietKhau >= 20).OrderBy(x => x.TenSp);
            //    listsanpham = _context.TDanhMucSps.Where(x => x.ChietKhau < 20 || x.ChietKhau == null).OrderBy(x => x.TenSp);
            //    //listsanpham = _context.TDanhMucSps.OrderBy(x => x.TenSp);
            //}
            PagedList<TDanhMucSp> lst = new PagedList<TDanhMucSp>(listsanpham, pageNumber, pageSize);
            PagedList<TDanhMucSp> lstck = new PagedList<TDanhMucSp>(listsanphamck, pageNumber, pageSize);
            // Tính số lượng trang hiển thị tối đa
            int totalPages = lst.PageCount;
            int startPage = Math.Max(1, pageNumber - (maxPagesToShow / 2));
            int endPage = Math.Min(totalPages, startPage + maxPagesToShow - 1);

            ViewBag.CurrentPageNumber = pageNumber;
            ViewBag.SelectedCategory = maloai;
            ViewBag.StartPage = startPage;
            ViewBag.EndPage = endPage;
            ViewBag.TotalPages = totalPages;

            return View(lst);
        }


        private void LogAcces()
        {
             var IpAdress = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(IpAdress))
            {
                IpAdress = "Unknow";
            }
            var acces = new TruyCap
            {
                IpAdress = IpAdress,
                ThoiGian = DateTime.Now
            };

            db.TruyCaps.Add(acces);
            db.SaveChanges();
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
            var comment = _context.Comments.Where(x => x.MaSp == maSp).ToList();
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
            var comment = db.Comments.Where(x => x.MaSp == maSp).ToList();
            var homeProductDetailViewModel = new HomeProductDetalViewModel { danhMucSp = sanPham, anhSp = anhSanPham ,comments=comment,CommentCount=comment.Count};

            return View(homeProductDetailViewModel);
        }

        public IActionResult lienhe()
        {
            return View();
        }
        
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

        public IActionResult Tintuc(int id)
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

            // Lấy bài viết đầu tiên dựa trên id được chọn
            var singlePost = db.Posts.FirstOrDefault(p => p.Id == id);

            if (singlePost == null)
            {
                // k có bài viết, hiển thị bài đầu tiên
                singlePost = db.Posts.FirstOrDefault();
            }

            // Lấy các bài viết gần đây
            var recentPosts = db.Posts
                .Where(p => p.Id != id) // Loại bỏ bài viết đầu tiên
                .OrderByDescending(p => p.Id) // Sắp xếp từ số lớn đến nhỏ
                .Take(5) // Lấy 5 bài viết gần đây
                .ToList();

            // Truyền dữ liệu vào view
            ViewBag.SinglePost = singlePost;
            ViewBag.RecentPosts = recentPosts;

            return View();
        }

        //[HttpPost]
        public IActionResult AddComment(string masp, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return BadRequest("Thiếu thông tin bình luận .");
            }
            Random rd = new Random();
            var newComment = new Comment
            {
                Id = rd.Next(1000, 10001),
                Content = content,
                CreatedAt = DateTime.Now,
                MaSp = masp
            };
                _context.Comments.Add(newComment);
                _context.SaveChanges();
                return RedirectToAction("ProductDetail","Home",new {masp=masp});
        }

        public IActionResult sanphamchietkhau(string maloai, int? page)
        {
            int pageSize = 8;
            int pageNumber = page ?? 1;
            IQueryable<TDanhMucSp> listsanphamck;
            listsanphamck = _context.TDanhMucSps.Where(x => x.ChietKhau >= 20).OrderBy(x => x.TenSp);

            if (!string.IsNullOrEmpty(maloai))
            {
                listsanphamck = listsanphamck.Where(x => x.MaLoai == maloai);
            }

            PagedList<TDanhMucSp> lstck = new PagedList<TDanhMucSp>(listsanphamck, pageNumber, pageSize);

            return View("sanphamchietkhau", lstck);
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