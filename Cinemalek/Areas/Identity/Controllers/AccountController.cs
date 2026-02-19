using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cinemalek.Areas.Identity.Controllers
{
    [Area(SD.IDENTITY_AREA)]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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
            TempData["success-notification"] = "Account has been added successfuly";

            return Redirect(nameof(Login));
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        //[HttpPost]
        //public async Task<IActionResult> Login ()
        //{

        //  return View();
        //}
    }


}
