using webbanhangmvc.Models;
namespace webbanhangmvc.Repository
{
    public class ChietKhauRepository : IChietKhauRepository
    {
        private readonly QlbanVaLiContext _context;
        public ChietKhauRepository(QlbanVaLiContext context)
        {
            _context = context;
        }
        public TDanhMucSp Add(TDanhMucSp loaisp)
        {
            _context.TDanhMucSps.Add(loaisp);
            _context.SaveChanges();
            return loaisp;
        }

        public TDanhMucSp Delete(string maloaisp)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TDanhMucSp> GetAllLoaiSp()
        {
            return _context.TDanhMucSps;
        }

        public TDanhMucSp GetLoaisp(string maloaisp)
        {
            return _context.TDanhMucSps.Find(maloaisp);
        }

        public TDanhMucSp Update(TDanhMucSp loaisp)
        {
            _context.Update(loaisp);
            _context.SaveChanges();
            return loaisp;
        }
    }
}
