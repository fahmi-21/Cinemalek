using Cinemalek.Models;
using Cinemalek.ViewModels;

namespace Cinemalek.DataAccess
{
    public class AppDbContext : DbContext
    {
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MovieSubImg> MovieSubImgs { get; set; }
        public DbSet<ActorMovie> ActorsMovies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Data Source=.\\SQLEXPRESS;Integrated Security=True;Initial Catalog=Cinemalek;Encrypt=False;Trust Server Certificate=True;"
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ActorMovie>(entity =>
            {
                entity.ToTable("ActorsMovies");

                entity.HasKey(am => new { am.MovieId, am.ActorId });

                entity.HasOne(am => am.Movie)
                    .WithMany(m => m.ActorsMovies)
                    .HasForeignKey(am => am.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(am => am.Actor)
                    .WithMany(a => a.ActorsMovies)  // ✅ جمع
                    .HasForeignKey(am => am.ActorId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

    }

}

