using System;

namespace BlogWebApi.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }

        // Navigation property
        public virtual User User { get; set; }
    }
}
