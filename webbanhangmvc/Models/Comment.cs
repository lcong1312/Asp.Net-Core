using System.ComponentModel.DataAnnotations.Schema;

namespace webbanhangmvc.Models
{
    public class Comment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? MaSp { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        
    }
}
