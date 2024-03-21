using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace webbanhangmvc.Models;

public partial class TUserKh
{
    public string UsernameKh { get; set; } 

    public string Password { get; set; } 

    public string? TenKhachHang { get; set; }

    public DateTime? NgaySinh { get; set; }

    public string? SoDienThoai1 { get; set; }

    public string? DiaChi { get; set; }

    public virtual ICollection<TKhachHang> TKhachHangs { get; } = new List<TKhachHang>();

}
