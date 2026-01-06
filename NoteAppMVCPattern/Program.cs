using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NoteApp.Application.Repo.Notebooks;
using NoteApp.Application.Repo.Notes;
using NoteApp.Application.Repo.Tags;
using NoteApp.Application.Services.Notebooks;
using NoteApp.Application.Services.Notes;
using NoteApp.Application.Services.Tags;
using NoteApp.Core.Entities;
using NoteApp.Infrastructure.Models;
using NoteApp.Infrastructure.Repo.Notebooks;
using NoteApp.Infrastructure.Repo.Tags;
using NoteApp.Infrastructure.Services.Notes;
using NoteApp.WebUI.Middleware;
using System.Text;

namespace NoteApp.WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("MYappsettings.json", optional: false, reloadOnChange: true);

            
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 3;
            })
                .AddEntityFrameworkStores<AppDBContext>()
                .AddDefaultTokenProviders();

            //builder.Services.AddDbContext<AppDBContext>(options =>
            //{
            //    var env = builder.Environment;

            //    if (env.IsDevelopment())
            //    {
            //        options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDB"));
            //    }
            //    else
            //    {
            //        options.UseNpgsql(builder.Configuration.GetConnectionString("CloudDB"));
            //    }
            //});

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });



            builder.Services.AddControllersWithViews();
            // FluentValidation kaydý
            builder.Services.AddValidatorsFromAssemblyContaining<NoteValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();

            //builder.Services.AddScoped<IValidator<AppUser>, UserValidator>();
            //builder.Services.AddScoped<IValidator<Note>, NoteValidator>();

            // Identity’ye FluentValidation adaptörü ekle
            //builder.Services.AddScoped<IUserValidator<AppUser>, FluentUserValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<FluentUserValidator>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/User/Login";
            });


            builder.Services.AddDbContext<AppDBContext>(opt =>
             opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Note

            builder.Services.AddScoped<INoteRepository, NoteRepository>(); 
            builder.Services.AddScoped<INoteService, NoteService>();
            

                                // Tag

            builder.Services.AddScoped<ITagRepository, TagRepository>();    
            builder.Services.AddScoped<ITagService, TagService>();
            
                                // Notebook

            builder.Services.AddScoped<INotebookService, NotebookService>();
            builder.Services.AddScoped<INotebookRepository, NotebookRepository>();

            builder.Services.AddAuthentication()
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
         {
             options.TokenValidationParameters = new TokenValidationParameters
             {
                 ValidateIssuer = true,
                 ValidateAudience = true,
                 ValidateLifetime = true,
                 ValidateIssuerSigningKey = true,
                 ValidIssuer = builder.Configuration["Jwt:Issuer"],
                 ValidAudience = builder.Configuration["Jwt:Audience"],
                 IssuerSigningKey = new SymmetricSecurityKey(
                 Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
             };
         });


            var app = builder.Build();

            // --- 1. ADIM: BU KODU EN TEPEYE KOYMALISIN ---
            // Bu ayar, AWS Load Balancer'dan gelen "Ben aslýnda HTTPS'im" bilgisini okur.
            // KnownNetworks ve KnownProxies listesini temizliyoruz çünkü AWS'nin IP'leri deðiþkendir.
            
            //var forwardedHeaderOptions = new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //};
            //forwardedHeaderOptions.KnownNetworks.Clear();
            //forwardedHeaderOptions.KnownProxies.Clear();

            //app.UseForwardedHeaders(forwardedHeaderOptions);
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseGlobalExceptionMiddleware();
            // Düzenle
            // app.UseForwardedHeaders();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=User}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
