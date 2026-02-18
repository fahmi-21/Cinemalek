using Microsoft.AspNetCore.Mvc;

namespace Cinemalek.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class HomeController : Controller
    {
        AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var movies = _context.Movies
                .Include(e => e.Category)
                .Include(e => e.Cinema)
                .Include(e => e.ActorsMovies)
                    .ThenInclude(am => am.Actor)
                .AsNoTracking()
                .ToList();

            var actors = _context.Actors.AsNoTracking().ToList();
            var cinemas = _context.Cinemas.AsNoTracking().ToList();
            var categories = _context.Categories.AsNoTracking().ToList();


            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var firstDayOfLastMonth = firstDayOfMonth.AddMonths(-1);
            var lastDayOfLastMonth = firstDayOfMonth.AddDays(-1);
            var nextWeek = today.AddDays(7);


            var yesterday = today.AddDays(-1);
            var activeMoviesYesterday = _context.Movies
                .AsNoTracking()
                .Count(m => m.Status && m.Date.Date == yesterday);


            var lastMonthRevenue = _context.Movies
                .AsNoTracking()
                .Where(m => m.Date >= firstDayOfLastMonth && m.Date <= lastDayOfLastMonth)
                .Sum(m => m.Price);


            var thisMonthRevenue = _context.Movies
                .AsNoTracking()
                .Where(m => m.Date >= firstDayOfMonth)
                .Sum(m => m.Price);


            var nextWeekMovies = _context.Movies
                .AsNoTracking()
                .Count(m => m.Date > today && m.Date <= nextWeek);


            ViewBag.TotalMovies = movies.Count;
            ViewBag.ActiveMovies = movies.Count(m => m.Status);
            ViewBag.InactiveMovies = movies.Count(m => !m.Status);
            ViewBag.TotalActors = actors.Count;
            ViewBag.TotalCinemas = cinemas.Count;
            ViewBag.TotalCategories = categories.Count;
            ViewBag.ActiveCategories = categories.Count(c => c.Status);
            ViewBag.TotalRevenue = movies.Sum(m => m.Price);
            ViewBag.AveragePrice = movies.Any() ? movies.Average(m => m.Price) : 0;
            ViewBag.HighestPrice = movies.Any() ? movies.Max(m => m.Price) : 0;
            ViewBag.UpcomingMovies = movies.Count(m => m.Date > DateTime.Now);


            ViewBag.AvgActorsPerMovie = movies.Any()
                ? movies.Average(m => m.ActorsMovies.Count)
                : 0;

            ViewBag.ActiveMoviesYesterday = activeMoviesYesterday;
            ViewBag.LastMonthRevenue = lastMonthRevenue;
            ViewBag.ThisMonthRevenue = thisMonthRevenue;
            ViewBag.NextWeekMovies = nextWeekMovies;

            return View(new DasbordVM
            {
                Movies = movies,
                Actors = actors,
                Cinemas = cinemas,
                Categories = categories
            });
        }
    }

}
