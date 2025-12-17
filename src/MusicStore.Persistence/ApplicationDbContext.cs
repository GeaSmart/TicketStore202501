using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicStore.Entities.Info;

namespace MusicStore.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<MusicStoreUserIdentity>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);    
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            modelBuilder.Ignore<ConcertInfo>();

            modelBuilder.Entity<MusicStoreUserIdentity>(x => x.ToTable("User"));
            modelBuilder.Entity<IdentityRole>(x => x.ToTable("Role"));
            modelBuilder.Entity<IdentityUserRole<string>>(x => x.ToTable("UserRole"));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseLazyLoadingProxies(); // Habilita proxies para lazy loading
            }
        }
    }
}