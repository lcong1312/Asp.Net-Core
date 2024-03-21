using System.ComponentModel.DataAnnotations;

namespace webbanhangmvc.Models
{
    public class GiftCode
    {
        public  int ID { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public decimal GiaTri { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public bool TrangThai { get; set; }
    }
}
