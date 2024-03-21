using System;
using System.Collections.Generic;

namespace webbanhangmvc.Models;

public partial class THoaDonBan
{
    public int? MaHoaDon { get; set; } 

    public DateTime? NgayHoaDon { get; set; }

    public string? MaKhachHang { get; set; }

    public string? MaNhanVien { get; set; }

    public decimal TongTienHd { get; set; }

    public double? GiamGiaHd { get; set; }

    public int PhuongThucThanhToan { get; set; }

    public string? DiaChi { get; set; }

    public string? ThongTinThue { get; set; }

    public int? TinhTrang { get; set; }

    public virtual TKhachHang? MaKhachHangNavigation { get; set; }

    public virtual TNhanVien? MaNhanVienNavigation { get; set; }

    public virtual ICollection<TChiTietHdb> TChiTietHdbs { get; } = new List<TChiTietHdb>();
}
