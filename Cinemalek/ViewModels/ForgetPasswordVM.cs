namespace Cinemalek.ViewModels
{
    public class ForgetPasswordVM
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Email Or UserName")]
        public string EmailOrUserName { get; set; } = string.Empty;
    }
}
