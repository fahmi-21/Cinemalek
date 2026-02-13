

using System.Security.Cryptography;

namespace Cinemalek.Repository
{
    public class MovieSubImgsRepository : Repositories <MovieSubImg> , IMovieSubImgsREpository
    {
        public void DeleteRange (IEnumerable<MovieSubImg> subimgsList ) 
        { 
            _context.MovieSubImgs.RemoveRange (subimgsList); 
        }
    }
}
