using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Cinemalek
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>( 
                options =>
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                }
                
            );
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>( option =>
            {
                option.User.RequireUniqueEmail = true;
                option.SignIn.RequireConfirmedEmail = true;
                option.Password.RequiredLength = 8;
                option.Lockout.MaxFailedAccessAttempts = 5;
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddScoped(typeof(IRepository<Category>), typeof(Repositories<Category>));
            builder.Services.AddScoped(typeof(IRepository<Actor>), typeof(Repositories<Actor>));
            builder.Services.AddScoped(typeof(IRepository<Cinema>), typeof(Repositories<Cinema>));
            builder.Services.AddScoped(typeof(IRepository<Movie>), typeof(Repositories<Movie>));
            builder.Services.AddScoped(typeof(IMovieSubImgsREpository), typeof(MovieSubImgsRepository));
            builder.Services.AddScoped(typeof(IRepository<ApplicationUserOTP>), typeof(Repositories<ApplicationUserOTP>));
            

            builder.Services.AddTransient<IEmailSender , EmailSender>();
            builder.Services.AddScoped<IAccountServices, AccountServices>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
