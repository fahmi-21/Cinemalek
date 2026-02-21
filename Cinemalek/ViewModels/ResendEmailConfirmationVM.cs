namespace Cinemalek.ViewModels
{
    public class ResendEmailConfirmationVM
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Email Or UserName")]
        public string EmailOrUserName { get; set; } = string.Empty;
    }
}
