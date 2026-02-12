namespace Cinemalek.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Actor
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength ( 100 , MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string Img { get; set; } = string.Empty;
        public ICollection<ActorMovie> ActorsMovies { get; set; } = new List<ActorMovie>();
    }
}
