
using Cinemalek.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cinemalek.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class CategoryController : Controller
    {
        private IRepository<Category> repository;

        public CategoryController(IRepository<Category> repository)
        {
            this.repository = repository;
        }
        public async Task<IActionResult> Index(string? name, bool? status, int page = 1)
        {
            var categories = await repository.GetAllAsync( tracked : false);

            if (!string.IsNullOrWhiteSpace(name))
                categories = categories
                    .Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            if (status.HasValue)
                categories = categories
                    .Where(c => c.Status == status.Value)
                    .ToList();


            int pageSize = 5;
            int currentpage = page;
            double totalPages = Math.Ceiling(categories.Count / (double)pageSize);
            categories = categories.Skip((page - 1) * pageSize).Take(pageSize).ToList();


            TempData["Notification"] = "Category has been added successfuly";

            return View(new CategoriesVM
            {
                Categories = categories ,
                CurrentPage = page,
                TotalPages = totalPages,
                CategoryName = name,
                Status = status,

            });
        }


        [HttpGet]
        public IActionResult Create()
        {

            return View(new Category());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if( !ModelState.IsValid )
            {
                TempData["error-notification"] = "Invalid Data";
                return View(category);
            }

            var exists = await repository.GetOneAsync( c => c.Name == category.Name && c.Id != category.Id);


            if (exists is not null)
            {
                ModelState.AddModelError("Name", "The name already exists");
                return View(category);
            }

            Response.Cookies.Append("success-notification" , "Category has been added successfuly");

            await repository.CreateAsync(category);
            await repository.Commitasync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edite([FromRoute] int id)
        {


            var category =await repository.GetOneAsync( e => e.Id == id);
            if (category is null) return NotFound();

            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Edite(Category category)
        {
            if (!ModelState.IsValid)
            {
                TempData["error-notification"] = "Invalid Data";
                return View(category);
            }

            var exists = await repository.GetOneAsync(c => c.Name == category.Name);

            if (exists != null)
            {
                ModelState.AddModelError("Name", "The name already exists");
                return View(category);
            }

            repository.Edit(category);
            await repository.Commitasync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var category = await repository.GetOneAsync(e => e.Id == id);
            if (category is null) return NotFound();

            repository.Delete(category);
            await repository.Commitasync();
            return RedirectToAction(nameof(Index));
        }
    }
}
