namespace Cinemalek.ViewModels
{
    public class MovieCreateVM
    {
        public Movie Movie { get; set; } = new();
        public IEnumerable<Category> Categories { get; set; } = default!;
        public IEnumerable<Cinema> Cinemas { get; set; } = default!;
        public IEnumerable<Actor> Actors { get; set; } = default!;
        
    }
}
