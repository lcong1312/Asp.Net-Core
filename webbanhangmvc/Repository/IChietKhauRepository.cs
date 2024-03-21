using webbanhangmvc.Models;
namespace webbanhangmvc.Repository
{
    public interface IChietKhauRepository
    {
        TDanhMucSp Add(TDanhMucSp masp);

        TDanhMucSp Update(TDanhMucSp tensp);

        TDanhMucSp Delete(string masp);

        TDanhMucSp GetLoaisp(string masp);
        IEnumerable<TDanhMucSp> GetAllLoaiSp();
    }
}
