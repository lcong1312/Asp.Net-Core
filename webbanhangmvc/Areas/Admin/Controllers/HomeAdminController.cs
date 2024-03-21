using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using webbanhangmvc.Models;
using webbanhangmvc.ViewModels;
using X.PagedList;
using System.IO;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Text.RegularExpressions;

namespace webbanhangmvc.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    public class HomeAdminController : Controller
    {
        private readonly IRazorViewEngine _razorViewEngine;
        QlbanVaLiContext db = new QlbanVaLiContext();
        public HomeAdminController(QlbanVaLiContext _db, IRazorViewEngine razorViewEngine)
        {
            db = _db;
            _razorViewEngine = razorViewEngine;
        }
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            var Username = HttpContext.Session.GetString("Username");
            
            if (Username != null)
            {
                ViewBag.UserName = Username;
                ViewBag.ShowLogin = false;
            }
            else
            {
                ViewBag.ShowLogin = true;
                return RedirectToAction("LoginAdmin", "HomeAdmin"); 
            }

            DateTime startdate = DateTime.Now.AddDays(-30);
            int tongdonhang= GetSoDonDatHang(startdate);
            decimal tongTienBan = GetTongTienBan(startdate);
            int LuongTruyCap = GetTruyCap(startdate);
            ViewBag.TruyCap = LuongTruyCap;
            ViewBag.TongTien = tongTienBan;
            ViewBag.TongSoDonDatHang = tongdonhang;
            return View();
        }

        //[Route("Gettotal")]
        //[HttpPost]
        //public List<object> Gettotal()
        //{
        //    List<object> data = new List<object>();
        //    DateTime startdate = DateTime.Now.AddDays(-30);
        //    List<string> labels = new List<string>();
        //    List<int> totalVisits = new List<int>();
        //    List<int> totalOrders = new List<int>();
        //    List<decimal> totalSales = new List<decimal>();

        //    int cumulativeVisits = 0;
        //    int cumulativeOrders = 0;
        //    decimal cumulativeSales = 0;

        //    for (int i = 0; i < 30; i++)
        //    {
        //        DateTime date = startdate.AddDays(i);
        //        labels.Add(date.ToString("MM/dd/yy"));
        //        int dailyVisits = GetTruyCap(date);
        //        int dailyOrders = GetSoDonDatHang(date);
        //        decimal dailySales = GetTongTienBan(date);

        //        cumulativeVisits += dailyVisits;
        //        cumulativeOrders += dailyOrders;
        //        cumulativeSales += dailySales;

        //        totalVisits.Add(cumulativeVisits);
        //        totalOrders.Add(cumulativeOrders);
        //        totalSales.Add(cumulativeSales);
        //    }

        //    data.Add(labels);
        //    data.Add(totalVisits);
        //    data.Add(totalOrders);
        //    data.Add(totalSales);
        //    return data;
        //}

        private int GetTruyCap(DateTime startDate)
        {
            int LuongTruyCap = 0;
            using (SqlConnection connection = new SqlConnection(db.Database.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("LuongTruyCap", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;

                    connection.Open();
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        LuongTruyCap = Convert.ToInt32(result);
                    }
                }
            }
            return LuongTruyCap;
        }
        private int GetSoDonDatHang(DateTime startDate)
        {
            int tongdonhang = 0;
            using (SqlConnection connection = new SqlConnection(db.Database.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("SoDonDatHang", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;

                    connection.Open();
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        tongdonhang = Convert.ToInt32(result);
                    }
                }
            }
            return tongdonhang;
        }

        private decimal GetTongTienBan(DateTime startDate)
        {
            decimal tongTienBan = 0;
            using (SqlConnection connection = new SqlConnection(db.Database.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("TongTien", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;

                    connection.Open();
                    var result = command.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        tongTienBan = Convert.ToDecimal(result);
                    }
                }
            }
            return tongTienBan;
        }


        [Route("ViewHeader")]
        [HttpGet]
        public async Task<IActionResult> ViewHeader()
        {
            var Username = HttpContext.Session.GetString("Username");

            if (Username != null)
            {
                ViewBag.UserName = Username;
                ViewBag.ShowLogin = false;
            }
            else
            {
                ViewBag.ShowLogin = true;
                return RedirectToAction("LoginAdmin", "HomeAdmin");
            }

            var layoutFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Shared", "_LayoutOgani.cshtml");

            string headerContent = "";
            string cssLinks = "";
            string jsLinks = "";

            if (System.IO.File.Exists(layoutFilePath))
            {
                // Đọc nội dung từ tệp LayoutOgani.cshtml
                string layoutContent = System.IO.File.ReadAllText(layoutFilePath);

                // Tìm vị trí bắt đầu của thẻ <header> và kết thúc của thẻ </header>
                int headerStartIndex = layoutContent.IndexOf("<header");
                int headerEndIndex = layoutContent.IndexOf("</header>", headerStartIndex);

                if (headerStartIndex != -1 && headerEndIndex != -1)
                {
                    // Trích xuất nội dung chỉ trong thẻ <header> và </header>
                    headerContent = layoutContent.Substring(headerStartIndex, headerEndIndex - headerStartIndex + 9);
                }

                // Tìm và trích xuất các thẻ <link> và <script>
                cssLinks = ExtractTags(layoutContent, "link", "rel=\"stylesheet\"");
                jsLinks = ExtractTags(layoutContent, "script", "src");
            }

            var model = new LayoutViewModel
            {
                Header = headerContent,
                CssLinks = cssLinks,
                JsLinks = jsLinks
            };
            return View("ViewHeader", model);
        }

        private string ExtractTags(string content, string tagName, string attribute)
        {
            var matches = Regex.Matches(content, $@"<{tagName}[^>]*{attribute}[^>]*>", RegexOptions.IgnoreCase);
            return string.Join("\n", matches.Cast<Match>().Select(m => m.Value));
        }


        [Route("EditLayout")]
        [HttpGet]
        public IActionResult EditLayout()
        {
            var Username = HttpContext.Session.GetString("Username");

            if (Username != null)
            {
                ViewBag.UserName = Username;
                ViewBag.ShowLogin = false;
            }
            else
            {
                ViewBag.ShowLogin = true;
                return RedirectToAction("LoginAdmin", "HomeAdmin");
            }

            var layoutFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Shared", "_LayoutOgani.cshtml");

            string headerContent = "";
            if (System.IO.File.Exists(layoutFilePath))
            {
                string layoutContent = System.IO.File.ReadAllText(layoutFilePath);

                int headerStartIndex = layoutContent.IndexOf("<header");
                int headerEndIndex = layoutContent.IndexOf("</header>", headerStartIndex) + 9;

                if (headerStartIndex != -1 && headerEndIndex != -1)
                {
                    headerContent = layoutContent.Substring(headerStartIndex, headerEndIndex - headerStartIndex);
                }
            }
            var model = new LayoutViewModel { Header = headerContent };
            return View(model);
        }

        [Route("EditLayout")]
        [HttpPost]
        public IActionResult EditLayout(LayoutViewModel model)
        {

            var layoutFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Shared", "_LayoutOgani.cshtml");

            if (System.IO.File.Exists(layoutFilePath))
            {
                string layoutContent = System.IO.File.ReadAllText(layoutFilePath);

                int headerStartIndex = layoutContent.IndexOf("<header");
                int headerEndIndex = layoutContent.IndexOf("</header>", headerStartIndex) + 9;

                if (headerStartIndex != -1 && headerEndIndex != -1)
                {
                    string newLayoutContent = layoutContent.Substring(0, headerStartIndex) +
                                              model.Header +
                                              layoutContent.Substring(headerEndIndex);

                    System.IO.File.WriteAllText(layoutFilePath, newLayoutContent);

                    //lưu vào csdl
                        var layoutRecord = db.LayoutContents.FirstOrDefault(l => l.Section == "Header");
                            layoutRecord = new LayoutContent { Section = "Header",
                                Content = model.Header,
                            UpdateTime=DateTime.Now};
                            db.LayoutContents.Add(layoutRecord);
                        db.SaveChanges();
                }
            }
            return RedirectToAction("Index", "Admin");
        }

        [Route("ViewBody")]
        [HttpGet]
        public async Task<IActionResult> ViewBody()
        {
            var Username = HttpContext.Session.GetString("Username");

            if (Username != null)
            {
                ViewBag.UserName = Username;
                ViewBag.ShowLogin = false;
            }
            else
            {
                ViewBag.ShowLogin = true;
                return RedirectToAction("LoginAdmin", "HomeAdmin");
            }

            var layoutFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Shared", "_LayoutOgani.cshtml");

            string bodyContent = "";
            string cssLinks = "";
            string jsLinks = "";

            if (System.IO.File.Exists(layoutFilePath))
            {
                // Đọc nội dung từ tệp LayoutOgani.cshtml
                string layoutContent = System.IO.File.ReadAllText(layoutFilePath);

                // Tìm vị trí bắt đầu của thẻ <body> và kết thúc của thẻ </body>
                int bodyStartIndex = layoutContent.IndexOf("<body");
                int bodyEndIndex = layoutContent.IndexOf("</body>", bodyStartIndex);

                if (bodyStartIndex != -1 && bodyEndIndex != -1)
                {
                    // Trích xuất nội dung chỉ trong thẻ <body> và </body>
                    bodyContent = layoutContent.Substring(bodyStartIndex, bodyEndIndex - bodyStartIndex + 7);
                }

                // Tìm và trích xuất các thẻ <link> và <script>
                cssLinks = ExtractTags(layoutContent, "link", "rel=\"stylesheet\"");
                jsLinks = ExtractTags(layoutContent, "script", "src");
            }

            var model = new LayoutViewModel
            {
                Body = bodyContent,
                CssLinks = cssLinks,
                JsLinks = jsLinks
            };
            return View("ViewBody", model);
        }



        [Route("EditBody")]
        [HttpGet]
        public IActionResult EditBody()
        {
            var Username = HttpContext.Session.GetString("Username");

            if (Username != null)
            {
                ViewBag.UserName = Username;
                ViewBag.ShowLogin = false;
            }
            else
            {
                ViewBag.ShowLogin = true;
                return RedirectToAction("LoginAdmin", "HomeAdmin");
            }
            var layoutFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Shared", "_LayoutOgani.cshtml");

            string bodyContent = "";
            if (System.IO.File.Exists(layoutFilePath))
            {
                string layoutContent = System.IO.File.ReadAllText(layoutFilePath);

                int bodyStartIndex = layoutContent.IndexOf("<body");
                int bodyEndIndex = layoutContent.IndexOf("</body>", bodyStartIndex) + 7;
                if (bodyStartIndex != -1 && bodyEndIndex != -1)
                {
                    bodyContent = layoutContent.Substring(bodyStartIndex, bodyEndIndex - bodyStartIndex);
                }
            }
            var model = new LayoutViewModel { Body = bodyContent };
            return View(model);
        }

        [Route("EditBody")]
        [HttpPost]
        public IActionResult EditBody(LayoutViewModel model)
        {
            
            var layoutFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Shared", "_LayoutOgani.cshtml");

            if (System.IO.File.Exists(layoutFilePath))
            {
                string layoutContent = System.IO.File.ReadAllText(layoutFilePath);

                int bodyStartIndex = layoutContent.IndexOf("<body");
                int bodyEndIndex = layoutContent.IndexOf("</body>", bodyStartIndex) + 7;
                if (bodyStartIndex != -1 && bodyEndIndex != -1)
                {
                    layoutContent = layoutContent.Substring(0, bodyStartIndex) +
                                    model.Body +
                                    layoutContent.Substring(bodyEndIndex);
                }

                System.IO.File.WriteAllText(layoutFilePath, layoutContent);

                var layoutRecord = db.LayoutContents.FirstOrDefault(l => l.Section == "Body");
                layoutRecord = new LayoutContent
                {
                    Section = "Body",
                    Content = model.Body,
                    UpdateTime = DateTime.Now
                };
                db.LayoutContents.Add(layoutRecord);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Admin");
        }

        [Route("ViewFooter")]
        [HttpGet]
        public async Task<IActionResult> ViewFooter()
        {
            var Username = HttpContext.Session.GetString("Username");

            if (Username != null)
            {
                ViewBag.UserName = Username;
                ViewBag.ShowLogin = false;
            }
            else
            {
                ViewBag.ShowLogin = true;
                return RedirectToAction("LoginAdmin", "HomeAdmin");
            }

            var layoutFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Shared", "_LayoutOgani.cshtml");

            string FooterContent = "";
            string cssLinks = "";
            string jsLinks = "";

            if (System.IO.File.Exists(layoutFilePath))
            {
                string layoutContent = System.IO.File.ReadAllText(layoutFilePath);

                int bodyStartIndex = layoutContent.IndexOf("<footer");
                int bodyEndIndex = layoutContent.IndexOf("</footer>", bodyStartIndex);

                if (bodyStartIndex != -1 && bodyEndIndex != -1)
                {
                    FooterContent = layoutContent.Substring(bodyStartIndex, bodyEndIndex - bodyStartIndex + 9);
                }

                cssLinks = ExtractTags(layoutContent, "link", "rel=\"stylesheet\"");
                jsLinks = ExtractTags(layoutContent, "script", "src");
            }

            var model = new LayoutViewModel
            {
                Footer = FooterContent,
                CssLinks = cssLinks,
                JsLinks = jsLinks
            };
            return View("ViewFooter", model);
        }

        [Route("EditFooter")]
        [HttpGet]
        public IActionResult EditFooter()
        {
            var Username = HttpContext.Session.GetString("Username");

            if (Username != null)
            {
                ViewBag.UserName = Username;
                ViewBag.ShowLogin = false;
            }
            else
            {
                ViewBag.ShowLogin = true;
                return RedirectToAction("LoginAdmin", "HomeAdmin");
            }

            var layoutFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Shared", "_LayoutOgani.cshtml");

            string bodyContent = "";
            if (System.IO.File.Exists(layoutFilePath))
            {
                string layoutContent = System.IO.File.ReadAllText(layoutFilePath);

                int bodyStartIndex = layoutContent.IndexOf("<footer");
                int bodyEndIndex = layoutContent.IndexOf("</footer>", bodyStartIndex) + 9;

                if (bodyStartIndex != -1 && bodyEndIndex != -1)
                {
                    bodyContent = layoutContent.Substring(bodyStartIndex, bodyEndIndex - bodyStartIndex);
                }
            }
            var model = new LayoutViewModel { Footer = bodyContent };
            return View(model);
        }

        [Route("EditFooter")]
        [HttpPost]
        public IActionResult EditFooter(LayoutViewModel model)
        {
            var layoutFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Shared", "_LayoutOgani.cshtml");

            if (System.IO.File.Exists(layoutFilePath))
            {
                string layoutContent = System.IO.File.ReadAllText(layoutFilePath);

                int footerStartIndex = layoutContent.IndexOf("<footer");
                int footerEndIndex = layoutContent.IndexOf("</footer>", footerStartIndex) + 9;

                if (footerStartIndex != -1 && footerEndIndex != -1)
                {
                    layoutContent = layoutContent.Substring(0, footerStartIndex) +
                                    model.Footer +
                                    layoutContent.Substring(footerEndIndex);

                    System.IO.File.WriteAllText(layoutFilePath, layoutContent);

                    var layoutRecord = db.LayoutContents.FirstOrDefault(l => l.Section == "Footer");
                    layoutRecord = new LayoutContent
                    {
                        Section = "Footer",
                        Content = model.Footer,
                        UpdateTime = DateTime.Now
                    };
                    db.LayoutContents.Add(layoutRecord);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index", "Admin");
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
                //if (sanPham.AnhDaiDien != null)
                //{
                //    var filename = Path.GetFileName(sanPham.AnhDaiDien);
                //    var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProductsImages", filename);
                //    using (var fileStream = new FileStream(filepath, FileMode.Create))
                //    {
                //        sanPham.AnhDaiDien.CopyTo(fileStream);
                //    }
                //    //sanPham.AnhDaiDien = filename;
                //}
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
        public IActionResult DonHang(int? page)
        {
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listsanpham = db.DonHangs.Where(o=>!string.IsNullOrEmpty(o.TenSp )&&!string.IsNullOrEmpty(o.TenKh)).OrderBy(o => o.NgayBan).AsNoTracking();
            PagedList<DonHang> orders = new PagedList<DonHang>(listsanpham, pageNumber, pageSize);
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("LoginAdmin", "HomeAdmin");
            }
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
