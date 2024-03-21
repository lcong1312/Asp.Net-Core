using System;
using System.Collections.Generic;

namespace webbanhangmvc.Models;

public partial class TChiTietHdb
{
    public int? MaHoaDon { get; set; }

    public string TenSp { get; set; } = null!;

    public DateTime? Ngayban { get; set; }

    public int? SoLuongBan { get; set; }

    public decimal? DonGiaBan { get; set; }

    public decimal? ThanhTien { get; set; }

    public string? DiaChi { get; set; }

    public int? Phuongthuctt { get; set; }

}
