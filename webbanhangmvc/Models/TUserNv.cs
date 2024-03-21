using System;
using System.Collections.Generic;

namespace webbanhangmvc.Models;

public partial class TUserNv
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? TenNhanVien { get; set; }

    public DateTime? NgaySinh { get; set; }

    public string? SoDienThoai1 { get; set; }

    public string? DiaChi { get; set; }

    public string? ChucVu { get; set; }

    public virtual ICollection<TNhanVien> TNhanViens { get; } = new List<TNhanVien>();
}
