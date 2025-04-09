namespace BlogWebApi.DTOs
{
    public class AuthDTOs
    {
        public class RegisterDTO
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class LoginDTO
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class UserDTO
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
        }
    }
}
