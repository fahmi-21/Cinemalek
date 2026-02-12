namespace Cinemalek.ViewModels
{
    public class CategoriesVM
    {
        public IEnumerable<Category> Categories { get; set; } = Enumerable.Empty<Category>();
        public int CurrentPage { get; set; }
        public double TotalPages { get; set; }
        public string? CategoryName { get; set; }
        public bool? Status { get; set; }
        public Dictionary<int, int>? MoviesCount { get; set; }
    }

}
