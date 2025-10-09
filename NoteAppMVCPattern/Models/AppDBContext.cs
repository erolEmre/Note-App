using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NoteAppMVCPattern.Models
{
    public class AppDBContext : IdentityDbContext<AppUser>
    {
        public DbSet<Note> Notes { get; set; }
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
           
        }
    }
}
