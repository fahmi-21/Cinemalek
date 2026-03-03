namespace Cinemalek.Models
{
    public class HomeVM
    {
        public IEnumerable<Movie> Movies { get; set; } = new List<Movie>();
        public IEnumerable<Actor> Actors { get; set; } = new List<Actor>();
        public IEnumerable<Cinema> Cinemas { get; set; } = new List<Cinema>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    }
}
