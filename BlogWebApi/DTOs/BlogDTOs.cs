using System;

namespace BlogWebApi.DTOs
{
    public class BlogDTOs
    {
        public class BlogCreateDTO
        {
            public string Title { get; set; }
            public string Content { get; set; }
        }

        public class BlogUpdateDTO
        {
            public string Title { get; set; }
            public string Content { get; set; }
        }

        public class BlogResponseDTO
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
            public DateTime CreatedDate { get; set; }
            public string AuthorName { get; set; }
            public int UserId { get; set; }
        }
    }
}
