using System.Diagnostics;
using Cinemalek.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cinemalek.Areas.Customer.Controllers
{
    [Area(SD.CUSTOMER_AREA)]
    public class HomeController : Controller
    {
        private IRepository<Movie> _movierepository;
        private IRepository<Actor> _actorrepository;
        private IRepository<Category> _categoryrepository;
        private IRepository<Cinema> _cinemarepository;
        private IRepository<MovieSubImg> _subImageRepository;
        

        public HomeController(IRepository<Movie> movierepository, IRepository<Actor> actorrepository,
            IRepository<Category> categoryrepository, IRepository<Cinema> cinemarepository , IRepository<MovieSubImg> subimgRepository)
        {
            _movierepository = movierepository;
            _actorrepository = actorrepository;
            _categoryrepository = categoryrepository;
            _cinemarepository = cinemarepository;
            _subImageRepository = subimgRepository;
        }
        public IActionResult Index()
        {
                var movies = _movierepository.GetAllAsync(tracked: false).Result
                    .Where(m => m.Status)
                    .ToList();
    
                var actors = _actorrepository.GetAllAsync(tracked: false).Result.ToList();
                var cinemas = _cinemarepository.GetAllAsync(tracked: false).Result.ToList();
                var categories = _categoryrepository.GetAllAsync(tracked: false).Result.ToList();
    
                return View(new HomeVM
                {
                    Movies = movies,
                    Actors = actors,
                    Cinemas = cinemas,
                    Categories = categories
                });
            return View();
        }
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
                return NotFound();

            // جلب الفيلم فقط
            var movie = await _movierepository.GetOneAsync(m => m.Id == id, tracked: false);

            if (movie == null)
                return NotFound();

            // جلب الفئة والسينما
            var category = await _categoryrepository.GetOneAsync(c => c.Id == movie.CategoryId, tracked: false);
            var cinema = await _cinemarepository.GetOneAsync(c => c.Id == movie.CinemaId, tracked: false);

            if (category == null || cinema == null)
                return NotFound();

            // جلب قائمة الممثلين من ActorRepository بناءً على علاقة ActorsMovies
            var actors = new List<Actor>();
            if (movie.ActorsMovies != null && movie.ActorsMovies.Any())
            {
                var actorIds = movie.ActorsMovies.Select(am => am.ActorId).ToList();
                actors = (await _actorrepository.GetAllAsync(a => actorIds.Contains(a.Id), tracked: false)).ToList();
            }

            // جلب الصور الفرعية مباشرة
            var subImgs = (await _subImageRepository
                .GetAllAsync(s => s.MovieId == movie.Id, tracked: false))
                .ToList();

            return View(new MovieDetailsVM
            {
                Movie = movie,
                Category = category,
                Cinema = cinema,
                Actors = actors,
                SubImgs = subImgs
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }


    }
}
