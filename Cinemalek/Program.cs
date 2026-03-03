using Cinemalek.Conventions;
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
            builder.Services.AddControllersWithViews( options =>
            {
                options.Conventions.Add(new AuthorizeAreaConvention("Admin" ,
                    $"{SD.ADMIN_ROLE},{SD.SUPER_ADMIN_ROLE}"));
            });
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
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            builder.Services.AddScoped(typeof(IRepository<Category>), typeof(Repositories<Category>));
            builder.Services.AddScoped(typeof(IRepository<Actor>), typeof(Repositories<Actor>));
            builder.Services.AddScoped(typeof(IRepository<Cinema>), typeof(Repositories<Cinema>));
            builder.Services.AddScoped(typeof(IRepository<Movie>), typeof(Repositories<Movie>));
            builder.Services.AddScoped(typeof(IRepository<Cart>), typeof(Repositories<Cart>));
            builder.Services.AddScoped(typeof(IRepository<MovieSubImg>), typeof(Repositories<MovieSubImg>));
            builder.Services.AddScoped(typeof(IMovieSubImgsREpository), typeof(MovieSubImgsRepository));
            builder.Services.AddScoped(typeof(IRepository<ApplicationUserOTP>), typeof(Repositories<ApplicationUserOTP>));
            builder.Services.AddScoped(typeof(IDBInitilizer), typeof(DBInitilizer));


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

            var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IDBInitilizer>();
            service.Initialize();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
