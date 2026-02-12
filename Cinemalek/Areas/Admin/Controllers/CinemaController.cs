using Cinemalek.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cinemalek.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class CinemaController : Controller
    {
       
        private Repositories<Cinema> repository = new();

        public async Task<IActionResult> Index ( string? name , int page = 1 )
        {
            var cinemas= await repository.GetAllAsync( tracked: false);

            if ( name is not null )
            {
                cinemas= cinemas.Where( e => e.Name == name ).ToList();
            }

            //pagination 
            if ( page < 1 )
                page = 1;
            int currentpage = page;
            int pagesize = 5;
            int totalpages = (int)Math.Ceiling(cinemas.Count() / (double)pagesize);
            cinemas= cinemas.Skip((page - 1) * pagesize).Take(pagesize).ToList(); ;



            return View( new CinemaVM
            {
                Cinemas = cinemas,
                CurrentPage = currentpage,
                TotalPages = totalpages
            });
        }

        [HttpGet]
        public IActionResult Create ()
        {
            return View( new Cinema());
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(Cinema cinema, IFormFile Img)
        {

            if (await repository.AnyAsync(c => c.Name.ToLower() == cinema.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "The cinema name already exists");
            }

            if (Img is null || Img.Length == 0)
            {
                ModelState.AddModelError("Img", "Image is required");
            }


            if (!ModelState.IsValid)
                return View(cinema);

         
            var newfilename = Guid.NewGuid() + DateTime.UtcNow.ToString("-dd-MM-yyyy") + Path.GetExtension(Img.FileName);
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Admin\\Imgs\\CinemasImg");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var filepath = Path.Combine(folder, newfilename);
            using (var stream = System.IO.File.Create(filepath))
            {
                Img.CopyTo(stream);
            }

            cinema.Img = newfilename;

      
            await repository.CreateAsync(cinema);
            await repository.Commitasync();

            return RedirectToAction(nameof(Index));
        }

        public async Task <IActionResult> Delete ([FromRoute] int Id)
        {
            var cinema = await repository.GetOneAsync( e => e.Id == Id);

            if (cinema is null)
                return NotFound();

            repository.Delete(cinema);
            await repository.Commitasync();
            return RedirectToAction(nameof (Index));

        }
        public async Task<IActionResult> Details ( [FromRoute] int Id ) 
        {
            var cinema = await repository.GetOneAsync(e => e.Id == Id);

            if (cinema is null)
                return NotFound();

            return View(cinema); 
        }

        [HttpGet]
        public async Task<IActionResult> Edit ([FromRoute] int Id)
        {
            var cinema = await repository.GetOneAsync(e => e.Id == Id);

            if (cinema is null)
                return NotFound();


            return View(cinema);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Cinema cinema, IFormFile? Img)
        {
            
            var cinemaDb = await repository.GetOneAsync(e => e.Id == cinema.Id);
            if (cinemaDb == null)
                return NotFound();

            
            if ( await repository.AnyAsync(c => c.Name.ToLower() == cinema.Name.ToLower() && c.Id != cinema.Id))
            {
                ModelState.AddModelError("Name", "The cinema name already exists");
            }

            if (!ModelState.IsValid)
                return View(cinemaDb); 

            
            if (Img is not null && Img.Length > 0)
            {
               
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "Imgs", "CinemasImg");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

               
                var newfilename = Guid.NewGuid() + DateTime.UtcNow.ToString("-dd-MM-yyyy") + Path.GetExtension(Img.FileName);
                var filepath = Path.Combine(folder, newfilename);

                using (var stream = System.IO.File.Create(filepath))
                {
                    Img.CopyTo(stream);
                }

               
                if (!string.IsNullOrEmpty(cinemaDb.Img))
                {
                    var oldFile = Path.Combine(folder, cinemaDb.Img);
                    if (System.IO.File.Exists(oldFile))
                        System.IO.File.Delete(oldFile);
                }

                cinemaDb.Img = newfilename;
            }

 
            cinemaDb.Name = cinema.Name;
            cinemaDb.Description = cinema.Description;
            cinemaDb.Location = cinema.Location;

            await repository.Commitasync();

            return RedirectToAction(nameof(Index));
        }



    }

}
