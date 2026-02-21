using Microsoft.AspNetCore.Identity;


namespace Cinemalek.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;   
        public string? Address { get; set; }
        public ICollection<ApplicationUserOTP> OTPs { get; set; } = new List<ApplicationUserOTP>();
    }
}
