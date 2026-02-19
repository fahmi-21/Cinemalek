using Cinemalek.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Cinemalek.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Cinemalek.DataAccess
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MovieSubImg> MovieSubImgs { get; set; }
        public DbSet<ActorMovie> ActorsMovies { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
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
                    .WithMany(a => a.ActorsMovies)  
                    .HasForeignKey(am => am.ActorId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
        public DbSet<Cinemalek.ViewModels.RegisterVM> RegisterVM { get; set; } = default!;
        public DbSet<Cinemalek.ViewModels.LoginVM> LoginVM { get; set; } = default!;

    }

}

