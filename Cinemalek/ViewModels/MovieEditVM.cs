namespace Cinemalek.ViewModels
{
    public class MovieEditVM
    {
        public Movie Movie { get; set; } = null!;

       
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Cinema> Cinemas { get; set; } = new List<Cinema>();
        public IEnumerable<Actor> Actors { get; set; } = new List<Actor>();
        public List<int> SelectedActorIds { get; set; } = new List<int>();
        public IEnumerable<MovieSubImg> SubImgs { get; set; } = new List<MovieSubImg>();
    }

}
