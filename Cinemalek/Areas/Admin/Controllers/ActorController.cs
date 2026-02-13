using Cinemalek.Models;
using Cinemalek.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cinemalek.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class ActorController : Controller
    {
        
        private Repositories<Actor> repository = new();
        public async Task<IActionResult> Index(string actorname, int page = 1)
        {
            var actors = await repository.GetAllAsync(tracked: false);

            if (!string.IsNullOrWhiteSpace(actorname))
                actors = actors.Where(c => c.Name.ToLower().Contains(actorname.ToLower())).ToList();

            // Pagination---------------------------------------------
            if (page < 1)
                page = 1;

            int currentpage = page;
            int pagesize = 5;
            double totalpages = (int)Math.Ceiling(actors.Count() / (double)pagesize);
            actors = actors.Skip((page - 1) * pagesize).Take(pagesize).ToList();


            return View(new ActorsVM
            {
                Actors = actors.AsEnumerable(),
                CurrentPage = currentpage,
                TotalPages = (int)totalpages
            });
        }

        public IActionResult Create()
        {
            return View(new Actor());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Actor actor, IFormFile Img)
        {
            if (Img is null || Img.Length == 0)
                ModelState.AddModelError("Img", "Profile picture is required");
            if ( await repository.AnyAsync(a => a.Name.ToLower() == actor.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "This actor name already exists!");
                return View(actor);
            }
            if (!ModelState.IsValid)
                return View(actor);

            var newfilename = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("dd-MM-yyyy") + Path.GetExtension(Img.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Admin\\Imgs\\ActorsImg", newfilename);
            using (var stream = System.IO.File.Create(filePath))
            {
                Img.CopyTo(stream);
            }


            actor.Img = newfilename;


            await repository.CreateAsync (actor);
            await repository.Commitasync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var actor = await repository.GetOneAsync(a => a.Id == id);

            if (actor == null)
                return NotFound();

            return View(actor);
        }

        [HttpGet]
        public async Task <IActionResult> Edit([FromRoute]int id)
        {
            var actor = await repository.GetOneAsync(a => a.Id == id);

            if (actor is null)
                return NotFound();

            return View(actor);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Actor actor, IFormFile? file)
        {
           
            if ( file is not null && file.Length > 0 )
            {
                var newfilename = Guid.NewGuid() + DateTime.UtcNow.ToString("yyyy-MM-dd") + Path.GetExtension(file.FileName);

                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Admin\\Imgs\\ActorsImg", newfilename);
                using (var stream = System.IO.File.Create(filepath))
                {
                    file.CopyTo(stream);
                }

                actor.Img = newfilename;
            }
           
            repository.Edit(actor);
            await repository.Commitasync();
            return RedirectToAction(nameof(Index));
        }

        public async Task <IActionResult> Delete([FromRoute]int id)
        {
            var actor =await repository.GetOneAsync(a => a.Id == id);
            if (actor is null)
                return NotFound();

            repository.Delete(actor);
            await repository.Commitasync();
            return RedirectToAction(nameof(Index));
        }
    }
}
