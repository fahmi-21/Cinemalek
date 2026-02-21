namespace Cinemalek.Services.IServices
{
    public interface IAccountServices
    {
        Task SendEmailAsync ( EmailType emailType, string msg ,  ApplicationUser applicationUser);
    }
}
