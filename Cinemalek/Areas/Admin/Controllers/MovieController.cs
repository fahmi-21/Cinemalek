using Cinemalek.Models;
using Cinemalek.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cinemalek.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Cinemalek.Repository;

[Area(SD.ADMIN_AREA)]
public class MovieController : Controller
{
    private readonly Repositories<Movie> _movieRepository = new();
    private readonly Repositories<Category> _categoryRepository = new();
    private readonly Repositories<Cinema> _cinemaRepository = new();
    private readonly Repositories<Actor> _actorRepository = new();
    private readonly Repositories<MovieSubImg> _subImgRepository = new();

    // ========================== INDEX ==========================
    public async Task<IActionResult> Index(MovieFilterVM? filter, int page = 1)
    {
        var movies = await _movieRepository.GetAllAsync(
            includes: new Expression<Func<Movie, object>>[]
            {
                e => e.Category,
                e => e.Cinema
            },
            tracked: false);

        

        if (filter != null)
        {
            if (!string.IsNullOrEmpty(filter.Name))
                movies = movies.Where(m => m.Name.Contains(filter.Name)).ToList();

            if (filter.MinPrice.HasValue)
                movies = movies.Where(m => m.Price >= filter.MinPrice).ToList();

            if (filter.MaxPrice.HasValue)
                movies = movies.Where(m => m.Price <= filter.MaxPrice).ToList();

            if (filter.CategoryId.HasValue)
                movies = movies.Where(m => m.CategoryId == filter.CategoryId).ToList();

            if (filter.Date.HasValue)
                movies = movies.Where(m => m.Date.Date == filter.Date.Value.Date).ToList();

            if (filter.Status.HasValue)
                movies = movies.Where(m => m.Status == filter.Status).ToList();
        }

        int pageSize = 5;
        int totalPages = (int)Math.Ceiling(movies.Count() / (double)pageSize);
        page = Math.Max(page, 1);

        var paged = movies.Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();

        return View(new MovieVM
        {
            Movies = paged,
            Categories = await _categoryRepository.GetAllAsync(tracked: false),
            Cinemas = await _cinemaRepository.GetAllAsync(tracked: false),
            CurrentPage = page,
            TotalPages = totalPages,
            Filter = filter
        });
    }

    // ========================== CREATE GET ==========================
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View(new MovieCreateVM
        {
            Movie = new Movie(),
            Categories = await _categoryRepository.GetAllAsync(tracked: false),
            Cinemas = await _cinemaRepository.GetAllAsync(tracked: false),
            Actors = await _actorRepository.GetAllAsync(tracked: false)
        });
    }

    // ========================== CREATE POST ==========================
    [HttpPost]
    public async Task<IActionResult> Create(
        MovieCreateVM vm,
        IFormFile? MainImg,
        List<IFormFile>? SubImgs,
        List<int>? actorsid)
    {
        if (await _movieRepository.AnyAsync(m => m.Name == vm.Movie.Name))
        {
            ModelState.AddModelError("Movie.Name", "Movie name already exists");
        }

        if (!ModelState.IsValid)
        {
            vm.Categories = await _categoryRepository.GetAllAsync();
            vm.Cinemas = await _cinemaRepository.GetAllAsync();
            vm.Actors = await _actorRepository.GetAllAsync();
            return View(vm);
        }

        // Main Image
        if (MainImg != null && MainImg.Length > 0)
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot/Admin/Imgs/MoviesImg");
            Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() +
                DateTime.UtcNow.ToString("yyyyMMdd") +
                Path.GetExtension(MainImg.FileName);

            var path = Path.Combine(folder, fileName);
            using var stream = System.IO.File.Create(path);
            await MainImg.CopyToAsync(stream);

            vm.Movie.Img = fileName;
        }

        // Sub Images
        if (SubImgs != null && SubImgs.Any())
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot/Admin/Imgs/MoviesImg/SubImgs");
            Directory.CreateDirectory(folder);

            vm.Movie.SubImages = new List<MovieSubImg>();

            foreach (var img in SubImgs)
            {
                var fileName = Guid.NewGuid() +
                    DateTime.UtcNow.ToString("yyyyMMdd") +
                    Path.GetExtension(img.FileName);

                var path = Path.Combine(folder, fileName);
                using var stream = System.IO.File.Create(path);
                await img.CopyToAsync(stream);

                vm.Movie.SubImages.Add(new MovieSubImg { Img = fileName });
            }
        }

        // Actors
        if (actorsid != null && actorsid.Any())
        {
            vm.Movie.ActorsMovies = actorsid
                .Select(id => new ActorMovie { ActorId = id })
                .ToList();
        }

        await _movieRepository.CreateAsync(vm.Movie);
        await _movieRepository.Commitasync();

        return RedirectToAction(nameof(Index));
    }

    // ========================== DELETE ==========================
    public async Task<IActionResult> Delete(int id)
    {
        var movie = await _movieRepository.GetOneAsync(m => m.Id == id);
        if (movie == null) return NotFound();

        _movieRepository.Delete(movie);
        await _movieRepository.Commitasync();

        return RedirectToAction(nameof(Index));
    }

    // ========================== DETAILS ==========================
    public async Task<IActionResult> Details(int id)
    {
        var movie = (await _movieRepository.GetAllAsync(
            m => m.Id == id,
            includes: new Expression<Func<Movie, object>>[]
            {
                e => e.SubImages,
                e => e.Category,
                e => e.Cinema,
                e => e.ActorsMovies
            },
            tracked: false)).FirstOrDefault();

        if (movie == null) return NotFound();

        return View(movie);
    }
}

}
