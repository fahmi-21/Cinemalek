namespace Cinemalek.ViewModels
{
    public class MovieFilterVM
    {
        public string? Name { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? CategoryId { get; set; }
        public bool? Status { get; set; }

        public DateTime? Date { get; set; }
        
    }
}
