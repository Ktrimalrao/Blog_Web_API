using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlogWebApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public virtual ICollection<Blog> Blogs { get; set; }
    }
}