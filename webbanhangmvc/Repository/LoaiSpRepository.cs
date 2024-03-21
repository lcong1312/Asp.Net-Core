using webbanhangmvc.Models;
namespace webbanhangmvc.Repository
{
    public class LoaiSpRepository : ILoaiSpRepository
    {
        private readonly QlbanVaLiContext _context;
        public LoaiSpRepository(QlbanVaLiContext context)
        {
            _context = context;
        }
        public TLoaiSp Add(TLoaiSp loaisp)
        {
            _context.TLoaiSps.Add(loaisp);
            _context.SaveChanges();
            return loaisp;
        }

        public TLoaiSp Delete(string maloaisp)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TLoaiSp> GetAllLoaiSp()
        {
            return _context.TLoaiSps;
        }

        public TLoaiSp GetLoaisp(string maloaisp)
        {
            return _context.TLoaiSps.Find(maloaisp);
        }

        public TLoaiSp Update(TLoaiSp loaisp)
        {
            _context.Update(loaisp);
            _context.SaveChanges();
            return loaisp;
        }
    }
}
