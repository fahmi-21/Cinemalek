using Microsoft.EntityFrameworkCore;

namespace Cinemalek.Repository
{
    public interface IMovieSubImgsREpository : IRepository<MovieSubImg>
    {
        void DeleteRange(IEnumerable<MovieSubImg> subimgsList);
    }
}
