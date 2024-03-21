using Microsoft.EntityFrameworkCore;
using webbanhangmvc.Infrastructure;

namespace webbanhangmvc.Models
{
    public class Cart
    {

        public List<CartLine> lines { get; set; } = new List<CartLine>();
        public void AddItem(TDanhMucSp product,int quantity)
        {
            CartLine? line = lines.Where(p => p.Product.MaSp == product.MaSp).FirstOrDefault();
            if (line == null)
            {
                lines.Add(new CartLine
                {
                    Product = product,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }
        public void RemoveLine(TDanhMucSp product) => lines.RemoveAll(l => l.Product.MaSp == product.MaSp);
        public decimal ComputeToTalValue()
        {
            return (decimal)lines.Sum(e => e.Product?.GiaLonNhat * e.Quantity);
        }
        public decimal ComputeQuantityValue()
        {
            return (decimal)lines.Sum(e => e.Quantity);
        }
        public string GetSp(string Sp)
        {
            CartLine line = lines.FirstOrDefault(p => p.Product.TenSp == Sp);
            if(line !=null)
            {
                return line.Product.TenSp;
            }
            return Sp;
        }

        public string GetProductName(string name)
        {
            return name;
        }
        public string GetQuantity()
        {
            return lines.Sum(e => e.Quantity).ToString(); // Tính tổng số lượng của các sản phẩm trong giỏ hàng
        }
        public string Diachiship(string address)
        {
            return address;
        }
        public string Phuongthuctt(string paymentMethod)
        {
            return paymentMethod;
        }
        public void Clear() => lines.Clear();
    }
    public class CartLine
    {
        public int CartLineID { get; set; }
        public TDanhMucSp Product { get; set; } = new();
        public int Quantity { get; set; }

    }
}
