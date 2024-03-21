using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;
using webbanhangmvc.Models;
using webbanhangmvc.Models.ProductModels;
using X.PagedList;

namespace webbanhangmvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly QlbanVaLiContext _db;

        public ProductAPIController(QlbanVaLiContext db)
        {
            _db = db;
        }

        [HttpGet("{maloai}")]
        public IActionResult ProductByCategory(string maloai, int? page)
        {
            int pageSize = 8;
            int pageNumber = page ?? 1;

            var products = _db.TDanhMucSps
                               .Where(p => p.MaLoai == maloai)
                               .OrderBy(p => p.TenSp)
                               .Select(p => new Product
                               {
                                   MaSp = p.MaSp,
                                   TenSp = p.TenSp,
                                   MaLoai = p.MaLoai,
                                   AnhDaiDien = p.AnhDaiDien,
                                   GiaNhoNhat = p.GiaNhoNhat
                               })
                               .ToPagedList(pageNumber, pageSize);
            var paginationMetadata = new
            {
                products.PageNumber,
                products.PageSize,
                products.TotalItemCount,
                products.PageCount,
                products.HasNextPage,
                products.HasPreviousPage
            };

            var response = new
            {
                products = products,
                pagination = paginationMetadata
            };

            return Ok(response);
        }
    }
}
