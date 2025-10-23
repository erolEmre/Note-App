using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NoteAppMVCPattern.Models;
namespace NoteAppMVCPattern.Models
{
    public class AppDBContext : IdentityDbContext<AppUser>
    {
        public DbSet<Note> Notes { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) 
        { 
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // mapping olmuyor burası olmazsa.

            modelBuilder.Entity<Note>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notes)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notebook>()
                .HasData(
                new Notebook
                {
                    Id = 1,
                    Name = "Default",
                    Description = "varsayılan notebook açıklaması",
                    Color = "bg-secondary",
                    CreatedAt = new DateTime(2025, 10, 23, 0, 0, 0, DateTimeKind.Utc)
                }

                );
           
        }
    }
}
