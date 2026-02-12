namespace Cinemalek.ViewModels
{
    public class MovieVM
    {
        public IEnumerable<Movie> Movies { get; set; } = default!;
        public IEnumerable<Category> Categories { get; set; } = default!;
        public IEnumerable<Cinema> Cinemas { get; set; } = default!;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public MovieFilterVM? Filter { get; set; }
    }
}
