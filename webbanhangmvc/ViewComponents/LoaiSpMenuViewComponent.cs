using webbanhangmvc.Models;
using Microsoft.AspNetCore.Mvc;
using webbanhangmvc.Repository;

namespace webbanhangmvc.ViewComponents
{
    public class LoaiSpMenuViewComponent: ViewComponent
    {
        private readonly ILoaiSpRepository _LoaiSp;
        public LoaiSpMenuViewComponent(ILoaiSpRepository    LoaiSpRepository)
        {
            _LoaiSp = LoaiSpRepository;
        }
        public IViewComponentResult Invoke()
        {
            var loaisp = _LoaiSp.GetAllLoaiSp().OrderBy(x => x.Loai);
            return View(loaisp);
        }
    }
}
