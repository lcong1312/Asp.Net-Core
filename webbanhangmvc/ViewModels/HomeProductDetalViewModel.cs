using webbanhangmvc.Models;
namespace webbanhangmvc.ViewModels
{
    public class HomeProductDetalViewModel
    {
        public TDanhMucSp? danhMucSp { get; set; }
        public List<TAnhSp>? anhSp { get; set; }
        public List<Comment>? comments { get; set; }
        public int CommentCount { get; set; }
    }
}
