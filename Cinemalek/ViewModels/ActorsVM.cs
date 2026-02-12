namespace Cinemalek.ViewModels
{
    public class ActorsVM
    {
        public IEnumerable<Actor> Actors { get; set; } = default!;
        public int CurrentPage { get; set; }
        public double TotalPages { get; set; }
    }
}
