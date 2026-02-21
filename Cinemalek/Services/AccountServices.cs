using Azure.Core;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Cinemalek.Services
{
    public enum EmailType
    {
        Confirmation,
        CorgetPassword,
        Redsendconfirmation
    }
    public class AccountServices : IAccountServices
    {
        private readonly IEmailSender _emailsender;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountServices( IEmailSender emailSender , UserManager<ApplicationUser> userManager )  
        {
            _emailsender = emailSender;
            _userManager = userManager;
        }
        public async Task SendEmailAsync(EmailType emailType, string msg, ApplicationUser applicationUser)
        {

            if (emailType == EmailType.Confirmation)
            {
                await _emailsender.SendEmailAsync(applicationUser.Email!, "Welcome To Cinemalek , please Confirm Your Account!", msg);
            }
            if (emailType == EmailType.Redsendconfirmation)
            {
                await _emailsender.SendEmailAsync(applicationUser.Email!, "Welcome To Cinemalek , please Confirm Your Account!", msg);
            }
            if (emailType == EmailType.CorgetPassword)
            {
                await _emailsender.SendEmailAsync(applicationUser.Email!, "Welcome To Cinemalek , please Confirm Your Account!", msg);
            }
        }
    }
}
