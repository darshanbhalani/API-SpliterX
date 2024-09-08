namespace SpliterX_API.Models
{
    public class LoginRequest
    {
        public string PhoneNumberOrEmail { get; set; }
        public string Password { get; set; }
    }
    public class SignupRequest
    {
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string Password { get; set; }
    }

    public class UserDetails
    {
        public long Id { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
    }
    public class UserUpdateModel
    {
        public long UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
    }

}
