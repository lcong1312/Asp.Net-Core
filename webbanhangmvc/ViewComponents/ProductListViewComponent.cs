using Microsoft.AspNetCore.Mvc;
using System.Linq;
using webbanhangmvc.Models;
using webbanhangmvc.Repository;

namespace webbanhangmvc.ViewComponents
{
    [ViewComponent(Name = "ProductListView")]
    public class ProductListViewComponent : ViewComponent
    {
        private readonly IChietKhauRepository _ChietKhau;

        public ProductListViewComponent(IChietKhauRepository ChietKhauRepository)
        {
            _ChietKhau = ChietKhauRepository;
        }

        public IViewComponentResult Invoke()
        {
            var productsWithDiscount = _ChietKhau.GetAllLoaiSp().Where(x => x.ChietKhau >= 20).OrderBy(x => x.TenSp);
            return View(productsWithDiscount);
        }
    }
}
