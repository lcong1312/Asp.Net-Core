using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using webbanhangmvc.Models;

namespace webbanhangmvc.ViewComponents
{
    public class LocSpViewComponent : ViewComponent
    {
        private readonly QlbanVaLiContext _context;

        public LocSpViewComponent(QlbanVaLiContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(int? page)
        {

            var loaiSp = _context.TLoaiSps.ToList(); 
            return View("Default", loaiSp);
        }
    }
}
