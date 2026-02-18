
using Cinemalek.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cinemalek.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class MovieController : Controller
    {
        private IRepository<Movie> repository;
        private IRepository<Category> categoryRepository ;
        private IRepository<Cinema> cinemaRepository ;
        private IRepository<Actor> actorRepository;
        private IMovieSubImgsREpository subImgRepository ;

        public MovieController(IRepository<Movie> repository, IRepository<Category> categoryRepository, IRepository<Cinema> cinemaRepository, IRepository<Actor> actorRepository, IMovieSubImgsREpository subImgRepository)
        {
            this.repository = repository;
            this.categoryRepository = categoryRepository;
            this.cinemaRepository = cinemaRepository;
            this.actorRepository = actorRepository;
            this.subImgRepository = subImgRepository;
        }


        public async Task<IActionResult> Index(MovieFilterVM filter, int page = 1)
        {
            var movies = await repository.GetAllAsync(includes: [e => e.Category, e => e.Cinema, e => e.ActorsMovies], tracked: false);






            if (!string.IsNullOrEmpty(filter.Name))
            {
                movies = movies.Where(m => m.Name.ToLower().Contains(filter.Name.ToLower())).ToList();
            }
            if (filter.MinPrice.HasValue)
            {
                movies = movies.Where(m => m.Price >= filter.MinPrice.Value).ToList();
            }
            if (filter.MaxPrice.HasValue)
            {
                movies = movies.Where(m => m.Price <= filter.MaxPrice.Value).ToList();
            }
            if (filter.CategoryId.HasValue)
            {
                movies = movies.Where(m => m.CategoryId == filter.CategoryId.Value).ToList();
            }
            if (filter.Status.HasValue)
            {
                movies = movies.Where(m => m.Status == filter.Status.Value).ToList();
            }
            if (filter.Date.HasValue)
            {
                movies = movies.Where(m => m.Date == filter.Date.Value).ToList();
            }


            //pagination 
            if (page < 1)
                page = 1;

            int pagesize = 5;
            var currentPage = page;
            double totalPages = Math.Ceiling((double)movies.Count / pagesize);
            movies = movies.Skip((currentPage - 1) * pagesize).Take(pagesize).ToList();

            return View(new MovieVM
            {
                Movies = movies,
                Categories = await categoryRepository.GetAllAsync(),
                Cinemas = await cinemaRepository.GetAllAsync(),
                CurrentPage = currentPage,
                TotalPages = (int)totalPages,
                Filter = filter
            });

        }

        [HttpGet]
        public IActionResult Create()
        {
            var movieVM = new MovieCreateVM
            {
                Movie = new Movie(),
                Categories = categoryRepository.GetAllAsync(tracked: false).Result,
                Cinemas = cinemaRepository.GetAllAsync(tracked: false).Result,
                Actors = actorRepository.GetAllAsync(tracked: false).Result
            };
            return View(movieVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(MovieCreateVM movieVM, IFormFile MainImg, List<IFormFile> SubImgs, List<int> actorsid)
        {


            if (MainImg is not null && MainImg.Length > 0)
            {
                var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("dd-MM-yyyy") + Path.GetExtension(MainImg.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Admin\\Imgs\\MoviesImg", newFileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    MainImg.CopyTo(stream);
                }
                movieVM.Movie.Img = newFileName;
            }
            if (SubImgs is not null && SubImgs.Any())
            {
                movieVM.Movie.SubImages = new List<MovieSubImg>();
                foreach (var Img in SubImgs)
                {
                    if (Img is not null && Img.Length > 0)
                    {
                        var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("dd-MM-yyyy") + Path.GetExtension(Img.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Admin\\Imgs\\MoviesImg\\SubImgs", newFileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            Img.CopyTo(stream);
                        }
                        movieVM.Movie.SubImages.Add(new MovieSubImg { Img = newFileName });
                    }
                }
            }
            if (actorsid is not null && actorsid.Any())
            {
                movieVM.Movie.ActorsMovies = new List<ActorMovie>();

                foreach (var actorId in actorsid)
                {
                    movieVM.Movie.ActorsMovies.Add(new ActorMovie
                    {
                        ActorId = actorId
                    });
                }
            }

            await repository.CreateAsync(movieVM.Movie);
            await repository.Commitasync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var movie = await repository.GetOneAsync(e => e.Id == id);

            if (movie is null)
                return NotFound();

            var movieSubImgs = await subImgRepository.GetAllAsync(e => e.Movie.Id == id);
            var cinemas = await cinemaRepository.GetAllAsync(tracked: false);
            var categories = await categoryRepository.GetAllAsync(tracked: false);
            var actors = await actorRepository.GetAllAsync(tracked: false);

            var movieVM = new MovieEditVM
            {
                Movie = movie,
                Categories = categories.AsEnumerable(),
                Cinemas = cinemas.AsEnumerable(),
                Actors = actors.AsEnumerable(),
                SubImgs = movieSubImgs.AsEnumerable()
            };
            return View(movieVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(MovieEditVM movieVM, IFormFile MainImg, List<IFormFile> SubImgs, List<int> actorsid)
        {
            
            var movieInDb = await repository.GetOneAsync(e => e.Id == movieVM.Movie.Id, tracked: false);

            
            if (MainImg is not null && MainImg.Length > 0)
            {
                
                var newfilename = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("ddMMyyyy") + Path.GetExtension(MainImg.FileName);
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Admin\\Imgs\\MoviesImg", newfilename);

                using (var stream = System.IO.File.Create(filepath))
                {
                    await MainImg.CopyToAsync(stream);
                }

                
                if (!string.IsNullOrEmpty(movieInDb.Img))
                {
                    var oldfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Admin\\Imgs\\MoviesImg", movieInDb.Img);
                    if (System.IO.File.Exists(oldfilepath))
                    {
                        System.IO.File.Delete(oldfilepath);
                    }
                }

                movieVM.Movie.Img = newfilename;
            }
            
            repository.Edit(movieVM.Movie);

            // تحديث الممثلين
            if (actorsid is not null)
            {
                // جلب الفيلم مع الممثلين
                var movieWithActors = await repository.GetAllAsync(
                    expression: m => m.Id == movieVM.Movie.Id,
                    includes: new Expression<Func<Movie, object>>[] { m => m.ActorsMovies },
                    tracked: true
                );

                var movie = movieWithActors?.FirstOrDefault();
                if (movie != null)
                {
                    // مسح الممثلين القدامى
                    if (movie.ActorsMovies != null)
                    {
                        movie.ActorsMovies.Clear();
                    }
                    else
                    {
                        movie.ActorsMovies = new List<ActorMovie>();
                    }

                    // إضافة الممثلين الجدد
                    foreach (var actorId in actorsid)
                    {
                        movie.ActorsMovies.Add(new ActorMovie
                        {
                            ActorId = actorId,
                            MovieId = movie.Id
                        });
                    }

                    await repository.Commitasync();
                }
            }

            // معالجة الصور الفرعية
            if (SubImgs != null && SubImgs.Any(s => s != null && s.Length > 0))
            {
                // جلب الصور الفرعية القديمة
                var movieOldsubimgs = await subImgRepository.GetAllAsync(e => e.MovieId == movieVM.Movie.Id);

                // إضافة الصور الجديدة
                foreach (var Img in SubImgs.Where(s => s != null && s.Length > 0))
                {
                    var newfilename = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("ddMMyyyy") + Path.GetExtension(Img.FileName);
                    var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Admin\\Imgs\\MoviesImg\\SubImgs", newfilename);

                    using (var stream = System.IO.File.Create(filepath))
                    {
                        await Img.CopyToAsync(stream);
                    }

                    await subImgRepository.CreateAsync(new MovieSubImg
                    {
                        Img = newfilename,
                        MovieId = movieVM.Movie.Id
                    });
                }

                // حذف الصور القديمة من الملفات
                foreach (var item in movieOldsubimgs)
                {
                    if (!string.IsNullOrEmpty(item?.Img))
                    {
                        var oldfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Admin\\Imgs\\MoviesImg\\SubImgs", item.Img);
                        if (System.IO.File.Exists(oldfilepath))
                        {
                            System.IO.File.Delete(oldfilepath);
                        }
                    }
                }

                // حذف الصور القديمة من قاعدة البيانات
                if (movieOldsubimgs != null && movieOldsubimgs.Any())
                {
                    subImgRepository.DeleteRange(movieOldsubimgs);
                    await subImgRepository.Commitasync(); // استخدم Commitasync تبع الـ subImgRepository
                }
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete([FromRoute] int Id)
        {
            var movie = await repository.GetOneAsync(m => m.Id == Id);
            if (movie is null) return NotFound();

            var oldfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Admin\\Imgs\\MoviesImg", movie.Img);
            if (System.IO.File.Exists(oldfilepath))
            {
                System.IO.File.Delete(oldfilepath);
            }

            var movieSubImgs = await subImgRepository.GetAllAsync(e => e.MovieId == Id);
            foreach (var item in movieSubImgs)
            {
                if (!string.IsNullOrEmpty(item?.Img))
                {
                    var subimgfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Admin\\Imgs\\MoviesImg\\SubImgs", item.Img);
                    if (System.IO.File.Exists(subimgfilepath))
                    {
                        System.IO.File.Delete(subimgfilepath);
                    }
                }
            }
            subImgRepository.DeleteRange(movieSubImgs);
            repository.Delete(movie);
            await repository.Commitasync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Details(int Id)
        {
            var movies = await repository.GetAllAsync(
                expression: m => m.Id == Id,
                includes: new Expression<Func<Movie, object>>[]
                {
                    m => m.Category,
                    m => m.Cinema,
                    m => m.ActorsMovies
                },
                tracked: false
            );

            var movie = movies.FirstOrDefault();
            if (movie == null) return NotFound();

            
            var actors = new List<Actor>();
            if (movie.ActorsMovies != null && movie.ActorsMovies.Any())
            {
                var actorIds = movie.ActorsMovies.Select(am => am.ActorId).ToList();
                actors = (await actorRepository.GetAllAsync(a => actorIds.Contains(a.Id), tracked: false)).ToList();
            }

            
            var subImgs = movie.SubImages?.ToList() ?? new List<MovieSubImg>();

            var movieVM = new MovieDetailsVM
            {
                Movie = movie,
                Category = movie.Category,
                Cinema = movie.Cinema,
                Actors = actors,
                SubImgs = subImgs
            };

            return View(movieVM);
        }





    }
}
