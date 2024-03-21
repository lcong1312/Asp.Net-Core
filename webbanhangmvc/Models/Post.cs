using System;
using System.ComponentModel.DataAnnotations;

namespace webbanhangmvc.Models
{
    public class Post
    {
        public int Id { get; set; }

        public string Title { get; set; }

        [MaxLength]
        public string Content { get; set; }

        [MaxLength]
        public string ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
