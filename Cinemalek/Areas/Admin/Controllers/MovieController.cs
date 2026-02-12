using Cinemalek.Models;
using Cinemalek.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Cinemalek.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class MovieController : Controller
    {
        AppDbContext _context = new AppDbContext();
        private Repositories

        public IActionResult Index(MovieFilterVM? movieFilterVM, int page = 1)
        {
            var movies = _context.Movies
                .AsNoTracking()
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .AsQueryable();

            if (movieFilterVM != null)
            {
                if (!string.IsNullOrEmpty(movieFilterVM.Name))
                    movies = movies.Where(m => m.Name.Contains(movieFilterVM.Name));

                if (movieFilterVM.MinPrice.HasValue)
                    movies = movies.Where(m => m.Price >= movieFilterVM.MinPrice.Value);

                if (movieFilterVM.MaxPrice.HasValue)
                    movies = movies.Where(m => m.Price <= movieFilterVM.MaxPrice.Value);

                if (movieFilterVM.CategoryId.HasValue)
                    movies = movies.Where(m => m.CategoryId == movieFilterVM.CategoryId);

                if (movieFilterVM.Date.HasValue)
                    movies = movies.Where(m => m.Date.Date == movieFilterVM.Date.Value.Date);

                if (movieFilterVM.Status.HasValue)
                    movies = movies.Where(m => m.Status == movieFilterVM.Status.Value);
            }

            int pageSize = 5;
            int totalPages = (int)Math.Ceiling(movies.Count() / (double)pageSize);
            page = Math.Max(page, 1);

            var pagedMovies = movies.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return View(new MovieVM
            {
                Movies = pagedMovies,
                CurrentPage = page,
                TotalPages = totalPages,
                Categories = _context.Categories.AsNoTracking().ToList(),
                Cinemas = _context.Cinemas.AsNoTracking().ToList(),
                Filter = movieFilterVM
            });
        }

        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.Categories.AsNoTracking().AsQueryable();
            var cinemas = _context.Cinemas.AsNoTracking().AsQueryable();
            var actors = _context.Actors.AsNoTracking().AsQueryable();
            return View(new MovieCreateVM
            {
                Movie = new Movie(),
                Categories = categories.AsEnumerable(),
                Cinemas = cinemas.AsEnumerable(),
                Actors = actors.AsEnumerable()
            });
        }

        [HttpPost]
        public IActionResult Create( MovieCreateVM movieCreateVM, IFormFile MainImg, List<IFormFile> SubImgs, List<int> actorsid)
        {
            
            
            if (!string.IsNullOrWhiteSpace(movieCreateVM.Movie.Name))
            {
                bool exists = _context.Movies.Any(m => m.Name == movieCreateVM.Movie.Name);
                if (exists)
                {
                    ModelState.AddModelError("Movie.Name", "Movie name already exists");
                    if (!ModelState.IsValid)
                    {
                        movieCreateVM.Categories = _context.Categories.AsNoTracking().ToList();
                        movieCreateVM.Cinemas = _context.Cinemas.AsNoTracking().ToList();
                        movieCreateVM.Actors = _context.Actors.AsNoTracking().ToList();

                        return View(movieCreateVM);
                    }
                }



            }

            // ================== Main Image ==================
            if (MainImg is not null && MainImg.Length > 0)
            {
                var newfilename = Guid.NewGuid() + DateTime.UtcNow.ToString("dd-MM-yyyy")
                                  + Path.GetExtension(MainImg.FileName);

                var filepath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/Admin/Imgs/MoviesImg",
                    newfilename);

                using var stream = System.IO.File.Create(filepath);
                MainImg.CopyTo(stream);

                movieCreateVM.Movie.Img = newfilename;
            }

            // ================== Sub Images ==================
            if (SubImgs is not null && SubImgs.Any())
            {
                movieCreateVM.Movie.SubImages = new List<MovieSubImg>();

                foreach (var img in SubImgs)
                {
                    if (img.Length > 0)
                    {
                        var newImgname = Guid.NewGuid() + Path.GetExtension(img.FileName);
                        var filePath = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot/Admin/Imgs/MoviesImg/SubImgs",
                            newImgname);

                        using var stream = System.IO.File.Create(filePath);
                        img.CopyTo(stream);

                        movieCreateVM.Movie.SubImages.Add(new MovieSubImg { Img = newImgname });
                    }
                }
            }

            // ================== Actors ==================
            if (actorsid is not null && actorsid.Any())
            {
                movieCreateVM.Movie.ActorsMovies = new List<ActorMovie>();

                foreach (var actorId in actorsid)
                {
                    movieCreateVM.Movie.ActorsMovies.Add(new ActorMovie
                    {
                        ActorId = actorId
                    });
                }
            }

            // ================== Save ==================
            _context.Movies.Add(movieCreateVM.Movie);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete([FromRoute]int id)
        {
            
            var movie = _context.Movies.Find(id);
            

            _context.Movies.Remove(movie);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit([FromRoute] int id)
        {
            var movie = _context.Movies
                .Include(e => e.ActorsMovies) 
                    .ThenInclude(am => am.Actor)
                .Include(e => e.Category)
                .Include(e => e.Cinema)
                .Include(e => e.SubImages)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null) return NotFound();

            var vm = new MovieEditVM
            {
                Movie = movie,
                SubImgs = movie.SubImages,
                Categories = _context.Categories.ToList(),
                Cinemas = _context.Cinemas.ToList(),
                Actors = _context.Actors.ToList(),
                SelectedActorIds = movie.ActorsMovies.Select(am => am.ActorId).ToList()  
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(Movie? movie, IFormFile? mainimg, List<IFormFile>? subimgs, List<int>? actorsid)
        {
            if (movie == null) return NotFound();

            var movieDb = _context.Movies
                .Include(m => m.ActorsMovies)  
                .Include(m => m.SubImages)
                .FirstOrDefault(m => m.Id == movie.Id);

            if (movieDb == null) return NotFound();

            if (_context.Movies.Any(m => m.Name.ToLower() == movie.Name.ToLower() && m.Id != movie.Id))
                ModelState.AddModelError("Name", "Movie name already exists");

            if (!ModelState.IsValid)
                return View(movieDb);

            movieDb.Name = movie.Name;
            movieDb.Description = movie.Description;
            movieDb.Price = movie.Price;
            movieDb.Date = movie.Date;
            movieDb.Status = movie.Status;
            movieDb.CategoryId = movie.CategoryId;
            movieDb.CinemaId = movie.CinemaId;

            if (mainimg != null && mainimg.Length > 0)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Imgs", "MoviesImg");
                Directory.CreateDirectory(folder);

                var oldPath = Path.Combine(folder, movieDb.Img);
                if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);

                var newName = Guid.NewGuid() + DateTime.UtcNow.ToString("yyyyMMdd") + Path.GetExtension(mainimg.FileName);
                var path = Path.Combine(folder, newName);
                using (var stream = System.IO.File.Create(path)) mainimg.CopyTo(stream);

                movieDb.Img = newName;
            }

            var subFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Imgs", "MoviesImg", "SubImgs");
            Directory.CreateDirectory(subFolder);

            if (subimgs != null && subimgs.Any())
            {
                foreach (var oldImg in movieDb.SubImages.ToList())
                {
                    var oldFile = Path.Combine(subFolder, oldImg.Img);
                    if (System.IO.File.Exists(oldFile)) System.IO.File.Delete(oldFile);
                }
                _context.MovieSubImgs.RemoveRange(movieDb.SubImages);
                movieDb.SubImages.Clear();

                foreach (var img in subimgs)
                {
                    var fileName = Guid.NewGuid() + DateTime.UtcNow.ToString("yyyyMMdd") + Path.GetExtension(img.FileName);
                    var filePath = Path.Combine(subFolder, fileName);
                    using (var stream = System.IO.File.Create(filePath)) img.CopyTo(stream);

                    movieDb.SubImages.Add(new MovieSubImg { Img = fileName });
                }
            }

           
            movieDb.ActorsMovies.Clear();
            if (actorsid != null && actorsid.Any())
            {
                foreach (var actorId in actorsid)
                {
                    movieDb.ActorsMovies.Add(new ActorMovie { ActorId = actorId });
                }
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details([FromRoute] int id)
        {
            var movie = _context.Movies
                .AsNoTracking()
                .Include(m => m.SubImages)
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.ActorsMovies)
                    .ThenInclude(am => am.Actor)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null)
                return NotFound();

            return View(movie);
        }
    }
}
