using Microsoft.AspNetCore.Mvc;

namespace Cinemalek.Areas.Customer.Controllers
{
    [Area(SD.CUSTOMER_AREA)]
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Cart> _cartrepository;
        private readonly IRepository<Movie> _movierepository;

        public CartController(IRepository<Movie> movierepository, IRepository<Cart> cartrepository, UserManager<ApplicationUser> userManager)
        {
            _movierepository = movierepository;
            _cartrepository = cartrepository;
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(int movieid, int count)
        {
            var user = await _userManager.GetUserAsync(User);
            var movie = await _movierepository.GetOneAsync(e => e.Id == movieid);

            if (user is null || movie is null) return NotFound();

            var cartinDb = await _cartrepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.Movie.Id == movieid);
            if (cartinDb is null)
            {
                await _cartrepository.CreateAsync(new Cart
                {
                    ApplicationUserId = user.Id,
                    MovieId = movieid,
                    Count = count,
                    ListPrice = movie.Price
                });
            }
            else
                cartinDb.Count += count;

            await _cartrepository.Commitasync();
            TempData["success"] = "Movie added to cart successfully";

            return RedirectToAction("index", "Home");
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null) return NotFound();

            var userCarts = await _cartrepository
                .GetAllAsync(e => e.ApplicationUserId == user.Id, includes: [e => e.Movie]);

            return View(userCarts);
        }
        public async Task<IActionResult> Increment(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var cart = await _cartrepository.GetOneAsync(
                e => e.Id == id && e.ApplicationUserId == user.Id,
                includes: new Expression<Func<Cart, object>>[]
                {
                    e => e.Movie
                }
            );
            if (cart == null) return NotFound();
            //if (cart.Count != cart.Movie)
            //{
                cart.Count += 1;
                await _cartrepository.Commitasync();
            //}
            //else
            //{
            //    TempData["error"] = "No more stock available";
            //}


            
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Decrement(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var cart = await _cartrepository.GetOneAsync(
                e => e.Id == id && e.ApplicationUserId == user.Id,
                includes: new Expression<Func<Cart, object>>[]
                {
                    e => e.Movie
                }
            );

            if (cart == null) return NotFound();
            if (cart.Count > 1) 
            {
                cart.Count -= 1;
                await _cartrepository.Commitasync();

            }

            
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var cart = await _cartrepository.GetOneAsync(
                e => e.Id == id && e.ApplicationUserId == user.Id,
                includes: new Expression<Func<Cart, object>>[]
                {
                    e => e.Movie
                }
            );
            if (cart == null) return NotFound();

            _cartrepository.Delete(cart);
            await _cartrepository.Commitasync();
            return RedirectToAction(nameof(Index));
        }
    }
}
