

using System.Security.Cryptography;

namespace Cinemalek.Repository
{
    public class MovieSubImgsRepository : Repositories <MovieSubImg> , IMovieSubImgsREpository
    {
        private readonly AppDbContext _context;
        public MovieSubImgsRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public void DeleteRange (IEnumerable<MovieSubImg> subimgsList ) 
        { 
            _context.MovieSubImgs.RemoveRange (subimgsList); 
        }
    }
}
