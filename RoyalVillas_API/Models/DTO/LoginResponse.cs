namespace RoyalVillas_API.Models.DTO
{
    public class LoginResponse
    {
        public string? Token { get; set; }
        public UserDTO? UserDTO { get; set; }
    }
}
