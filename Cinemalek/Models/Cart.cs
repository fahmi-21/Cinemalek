namespace Cinemalek.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
         public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int Count { get; set; }
        public decimal ListPrice { get; set; }
    }
}
