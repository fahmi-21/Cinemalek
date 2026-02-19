using Cinemalek.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Cinemalek.ViewModels;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace Cinemalek.Areas.Identity.Controllers
{
    [Area(SD.IDENTITY_AREA)]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly IEmailSender _emailSender;
        public AccountController(UserManager<ApplicationUser> userManager , SignInManager<ApplicationUser> signinmangar , IEmailSender emailSender )
        {
            _userManager = userManager;
            _signinManager = signinmangar;
            _emailSender = emailSender;
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
            var confirmationLink = Url.Action("ConfirmEmail", "Account", new { area = "Identity"  , token , applicationUser.Id} , Request.Scheme);
            await _emailSender.SendEmailAsync(applicationUser.Email, "Welcome To Cinemalek , please Confirm Your Account!" , $"<h1>Click <a href='{ConfirmEmail}'>here</a> to cofirm youyr account</h1>");

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
    }


}
