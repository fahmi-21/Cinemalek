using Cinemalek.Areas.Admin.Controllers;
using Cinemalek.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;


namespace Cinemalek.Areas.Identity.Controllers
{
    [Area(SD.IDENTITY_AREA)]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly IEmailSender _emailSender;
        private readonly IAccountServices _accountservices;
        private readonly IRepository<ApplicationUserOTP> _applicationUserOTPRepo;

        public AccountController(UserManager<ApplicationUser> userManager , SignInManager<ApplicationUser> signinmangar , IEmailSender emailSender
            ,IAccountServices accountServices , IRepository<ApplicationUserOTP> applicationUserOTPRepo )
        {
            _userManager = userManager;
            _signinManager = signinmangar;
            _emailSender = emailSender;
            _accountservices = accountServices;
            _applicationUserOTPRepo = applicationUserOTPRepo;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
                return View(registerVM);

            ApplicationUser applicationUser = new ApplicationUser()
            {
                FName = registerVM.FName,
                LName = registerVM.LName,
                UserName = registerVM.UserName,
                Email = registerVM.Email
            };
            //var applicationuser = registerVM.Adapt<ApplicationUser>();
            //applicationuser.Id = Guid.NewGuid().ToString();

            var result = await _userManager.CreateAsync(applicationUser, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                    ModelState.AddModelError("", item.Description);

                return View(registerVM);
            }


            var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

            var confirmationLink = Url
                .Action("ConfirmEmail",
                "Account",
                new { area = "Identity", token, Id = applicationUser.Id },
                Request.Scheme);

            await _accountservices.SendEmailAsync( EmailType.Confirmation, $"<h1>Click <a href='{confirmationLink}'>here</a> to cofirm youyr account</h1>", applicationUser);

            TempData["success-notification"] = "Account has been added successfuly";

            return Redirect(nameof(Login));
        }

        public async Task<IActionResult> ConfirmEmail( string Id, string token)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user is null) return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                    ModelState.AddModelError(String.Empty , item.Description );

                TempData["error-notification"] = $"invalid confirmation link please tryv again";

                
                //return View( "Index", "Home" , new {area = "Customer"});
            }
            else
                TempData["success-notification"] = $"account has been confirmed successfuly";

            return RedirectToAction(nameof(Login));
        }

        [HttpGet] 
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login( LoginVM loginvm)
        {
            if (!ModelState.IsValid)
                return View(loginvm);

            var user =  await _userManager.FindByEmailAsync(loginvm.EmailOrUserName)??
                     await _userManager.FindByNameAsync(loginvm.EmailOrUserName);

            //bool isvalid = true;

            if (user == null)
            {
                ModelState.AddModelError("EmailOrUserName", "Invalid Email or UserName");
                ModelState.AddModelError("Password", "Invalid Password");
                return View(loginvm);
            }

            //isvalid = await _userManager.CheckPasswordAsync(user , loginvm.Password);
            var result = await _signinManager.PasswordSignInAsync(user, loginvm.Password ,  loginvm.RememberMe  ,true);

            if (result.Succeeded)
            {
                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("EmailOrUserName", "Confirm Your Mail First");
                    return View(loginvm);
                }
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(String.Empty, "Too Many Atemps , please try again later");
                    return View(loginvm);
                }

                ModelState.AddModelError("EmailOrUserName", "Invalid Email or UserName");
                ModelState.AddModelError("Password", "Invalid Password");
                return View(loginvm);
            }
            TempData["success-notification"] = $"Welcome Back {user.UserName}";

            return RedirectToAction( nameof (Register));
            //return View( "Index", "Home" , new {area = "Customer"});
        }
        [HttpGet]
        public async Task<IActionResult> ForgetPassword( )
        {
            var user = new ForgetPasswordVM();
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword( ForgetPasswordVM forgetPasswordVM)
        {
            if (!ModelState.IsValid)
                return View(forgetPasswordVM);

            var user = await _userManager.FindByEmailAsync(forgetPasswordVM.EmailOrUserName) ??
                     await _userManager.FindByNameAsync(forgetPasswordVM.EmailOrUserName);

            var usertobscount =(await _applicationUserOTPRepo.GetAllAsync( e =>(DateTime.UtcNow 
            - e.CreateAt).TotalHours > 24)).Count();

            if (user is not null && usertobscount <= 3)
            {
                string otp = Random.Shared.Next(100000, 999999).ToString();
                string msg = $"<h1> Your OTB Is:  {otp}  Dont Share It.</h1>";
                await _accountservices.SendEmailAsync(EmailType.CorgetPassword,msg, user);

                var otpuser = new ApplicationUserOTP
                {
                    ApplicationUserId = user.Id,
                    OTP = otp
                };
          

                await _applicationUserOTPRepo.CreateAsync( otpuser );
                await _applicationUserOTPRepo.Commitasync();
                TempData["success-notification"] = "OTB Has Been sended Successfuly , If Account Is Exist ";
            }
            else if (usertobscount > 3)
            {
                TempData["error-notification"] = "You Have Too Many Attempts , Please Try Again Later after 24 hour";
                return RedirectToAction("LogIn", "Account", new { area = "Identity" });
            }

            

            return RedirectToAction("LogIn", "Account", new { area = "Identity" });
        }

        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {
            if (!ModelState.IsValid)
                return View(resendEmailConfirmationVM);

            var user = await _userManager.FindByEmailAsync(resendEmailConfirmationVM.EmailOrUserName) ??
                     await _userManager.FindByNameAsync(resendEmailConfirmationVM.EmailOrUserName);

            if (user is not null && !user.EmailConfirmed)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmationLink = Url.Action("ConfirmEmail","Account",
                    new { area = "Identity", token, Id = user.Id },
                    Request.Scheme);

                await _accountservices.SendEmailAsync(EmailType.Redsendconfirmation, $"<h1>Click <a href='{confirmationLink}'>here</a> to cofirm youyr account</h1>", user);
            }

            TempData["success-notification"] = "Email Has Been Resended Successfuly , If Account Is Exist and Not Confirmed";

            return RedirectToAction("LogIn", "Account", new { area = "Identity" });
        }
    }
}
