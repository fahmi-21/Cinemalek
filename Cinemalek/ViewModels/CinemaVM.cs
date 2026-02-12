namespace Cinemalek.ViewModels
{
    public class CinemaVM
    {
        public IEnumerable<Cinema> Cinemas { get; set; } = default!;
        public IFormFile? file { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
