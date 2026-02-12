namespace Cinemalek.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Cinema
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public string Location { get; set; } = string.Empty;
        public string Img { get; set; } = string.Empty;
        public List<Movie>? Movies { get; set; }
    }

}
