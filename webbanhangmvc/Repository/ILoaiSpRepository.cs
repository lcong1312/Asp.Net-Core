using webbanhangmvc.Models;
namespace webbanhangmvc.Repository
{
    public interface ILoaiSpRepository
    {
        TLoaiSp Add(TLoaiSp loaisp);

        TLoaiSp Update(TLoaiSp loaisp);

        TLoaiSp Delete(string maloaisp);

        TLoaiSp GetLoaisp(string maloaisp);
        IEnumerable<TLoaiSp> GetAllLoaiSp();
    }
}
