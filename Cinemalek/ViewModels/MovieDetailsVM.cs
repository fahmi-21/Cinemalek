namespace Cinemalek.ViewModels
{
    public class MovieDetailsVM
    {
        public Movie Movie { get; set; } = default!;
        public Category Category { get; set; } = default!; 
        public Cinema Cinema { get; set; } = default!;     
        public List<Actor> Actors { get; set; } = new List<Actor>();
        public List<MovieSubImg> SubImgs { get; set; } = new List<MovieSubImg>();

    }
}
