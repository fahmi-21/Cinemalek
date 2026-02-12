namespace Cinemalek.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [Length ( 5 , 250 )]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required(ErrorMessage = "Price is required")]
        [Range(1, 10000, ErrorMessage = "Price must be between 1 and 10000")]
        public decimal Price { get; set; }
        public string Img { get; set; } = string.Empty;
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public bool Status { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        [Required]
        public int CinemaId { get; set; }
        public Cinema? Cinema { get; set; } 
        public List<MovieSubImg> SubImages { get; set; } = new();
        public ICollection<ActorMovie> ActorsMovies { get; set; } = new List<ActorMovie>();
    }
}
