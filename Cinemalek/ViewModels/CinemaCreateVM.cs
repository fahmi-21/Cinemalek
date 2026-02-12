namespace Cinemalek.ViewModels
{
    public class CinemaCreateVM
    {
       
        public Cinema Cinema {  get; set; }

        [Required(ErrorMessage = "Image is required")]
        public IFormFile Img { get; set; } = null!;
    }

}
