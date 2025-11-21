using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NoteAppMVCPattern.Models;
using NoteAppMVCPattern.Repo.Notebooks;
using NoteAppMVCPattern.Repo.Notes;
using NoteAppMVCPattern.Repo.Tags;
using NoteAppMVCPattern.Services.Notebooks;
using NoteAppMVCPattern.Services.Notes;
using NoteAppMVCPattern.Services.Tags;
using System;
using System.Text;

namespace NoteAppMVCPattern
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

            builder.Services.AddDbContext<AppDBContext>(options =>
            {
                #if DEBUG
                options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDB"));
                #else
                options.UseNpgsql(builder.Configuration.GetConnectionString("CloudDB"));
                #endif
            });
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            


            // FluentValidation kaydý
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddScoped<IValidator<AppUser>, UserValidator>();
            builder.Services.AddScoped<IValidator<Note>, NoteValidator>();

            // Identity’ye FluentValidation adaptörü ekle
            builder.Services.AddScoped<IUserValidator<AppUser>, FluentUserValidator>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/User/Login";
            });

            //var env = builder.Environment.EnvironmentName;
            //if (env == "Development")
            //{
            //    builder.Services.AddDbContext<AppDBContext>(opt =>
            //    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            //}

            //builder.Services.AddDbContext<AppDBContext>(opt =>
            // opt.UseNpgsql(builder.Configuration.GetConnectionString("CloudDB")));

                                // Note

            builder.Services.AddScoped<INoteRepository, NoteRepository>(); 
            builder.Services.AddScoped<INoteService, NoteService>();
            

                                // Tag

            builder.Services.AddScoped<ITagRepository, TagRepository>();    
            builder.Services.AddScoped<ITagService, TagService>();
            
                                // Notebook

            builder.Services.AddScoped<INotebookService, NotebookService>();
            builder.Services.AddScoped<INotebookRepository, NotebookRepository>();

            builder.Services.AddControllersWithViews().
            AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());

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
