namespace webbanhangmvc.Models
{
    public class DonHang
    {
        public int MaHoaDon { get; set; }
        public string? TenKh { get; set; }
        public string? TenSp { get; set; }
        public DateTime? NgayBan { get; set; }
        public string? Soluong { get; set; }

        public decimal? ThanhTien { get; set; }

        public string? DiaChi { get; set; }

        public string? Phuongthuctt { get; set; }
        public string? Xulydonhang { get; set; }

        public string? TransactionId { get; set; } 
        public string? bankSubAccId { get; set; }
        public string? UsernameKh { get; set; }
        public string? TinhTrang { get; set; }

    }
}
