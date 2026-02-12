namespace Cinemalek.ViewModels
{
    public class DasbordVM
    {
        public IEnumerable<Movie> Movies { get; set; } = null!;
        public IEnumerable <Actor> Actors { get; set; } = null!;
        public IEnumerable <Cinema> Cinemas { get; set; } = null!;
        public IEnumerable <Category> Categories { get; set; } = null!;

    }
}
