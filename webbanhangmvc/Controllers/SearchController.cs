using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using webbanhangmvc.Models;
using X.PagedList;

namespace webbanhangmvc.Controllers
{
    public class SearchController : Controller
    {
        private readonly QlbanVaLiContext _context;

        public SearchController(QlbanVaLiContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchString, int? page)
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
            int pageSize = 8;
            int pageNumber = page ?? 1;

            var query = _context.TDanhMucSps.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.TenSp.Contains(searchString));
            }

            var searchResult = query.OrderBy(x => x.TenSp).ToPagedList(pageNumber, pageSize);

            ViewBag.SearchString = searchString;

            ViewBag.CurrentPageNumber = pageNumber;

            return View(searchResult);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult ProductDetail(string id)
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
            var product = _context.TDanhMucSps.FirstOrDefault(p => p.MaSp == id);
            if (product == null)
            {
                return NotFound();
            }

            // Nếu sản phẩm tồn tại, trả về view chi tiết sản phẩm
            return View(product);
        }

    }
}
