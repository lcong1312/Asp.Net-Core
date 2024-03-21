using Microsoft.AspNetCore.Mvc;
using webbanhangmvc.Infrastructure;
using webbanhangmvc.Models;

namespace webbanhangmvc.ViewComponents
{
    public class CartWidgt : ViewComponent
    {
       
        public IViewComponentResult Invoke()
        {
            return View(HttpContext.Session.GetJson<Cart>("cart"));
        }
    }
}
