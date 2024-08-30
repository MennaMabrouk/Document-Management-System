
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Data
{
    public class DataContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Users");

            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");

            modelBuilder.Entity<User>()
                .HasOne(u => u.Workspace)
                .WithOne(w => w.User)
                .HasForeignKey<Workspace>(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Workspace>()
              .HasMany(w => w.Folders)
              .WithOne(d => d.Workspace)
              .HasForeignKey(d => d.WorkspaceId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Folder>()
             .HasMany(di => di.Documents)
             .WithOne(dd => dd.Folder)
             .HasForeignKey(dd => dd.FolderId)
             .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<User>()
            .HasIndex(u => u.Nid)
             .IsUnique();

            modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

            modelBuilder.Entity<Workspace>()
            .HasIndex(w => w.Name)
            .IsUnique();


        }

    }
}
