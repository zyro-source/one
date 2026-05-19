using buildwave.Entities;
using Microsoft.EntityFrameworkCore;

namespace buildwave.Data;

public class ApplicationDbContext : DbContext
{
      public ApplicationDbContext(
          DbContextOptions<ApplicationDbContext> options
      ) : base(options)
      {
      }

      public DbSet<User> Users => Set<User>();

      public DbSet<UserSession> UserSessions
          => Set<UserSession>();

      public DbSet<Role> Roles
          => Set<Role>();

      public DbSet<UserRole> UserRoles
          => Set<UserRole>();
      public DbSet<Permission> Permissions => Set<Permission>();
      public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
            base.OnModelCreating(modelBuilder);

            // USERS

            modelBuilder.Entity<User>(entity =>
            {
                  entity.ToTable("users");

                  entity.HasKey(x => x.Id);

                  entity.HasIndex(x => x.Email)
                    .IsUnique();

                  entity.Property(x => x.FullName)
                    .HasMaxLength(150);

                  entity.Property(x => x.Email)
                    .HasMaxLength(150);

                  entity.Property(x => x.PasswordHash)
                    .HasColumnType("text");
            });

            // USER SESSIONS

            modelBuilder.Entity<UserSession>(entity =>
            {
                  entity.ToTable("user_sessions");

                  entity.HasKey(x => x.Id);

                  entity.HasIndex(x => x.SessionToken)
                    .IsUnique();

                  entity.Property(x => x.SessionToken)
                    .HasMaxLength(200);

                  entity.Property(x => x.IpAddress)
                    .HasMaxLength(100);

                  entity.Property(x => x.UserAgent)
                    .HasColumnType("text");

                  entity.HasOne(x => x.User)
                    .WithMany(x => x.Sessions)
                    .HasForeignKey(x => x.UserId);
            });

            // ROLES

            modelBuilder.Entity<Role>(entity =>
            {
                  entity.ToTable("roles");

                  entity.HasKey(x => x.Id);

                  entity.HasIndex(x => x.Name)
                    .IsUnique();

                  entity.Property(x => x.Name)
                    .HasMaxLength(100);
            });

            // USER ROLES

            modelBuilder.Entity<UserRole>(entity =>
            {
                  entity.ToTable("user_roles");

                  entity.HasKey(x => new
                  {
                        x.UserId,
                        x.RoleId
                  });

                  entity.HasOne(x => x.User)
                    .WithMany(x => x.UserRoles)
                    .HasForeignKey(x => x.UserId);

                  entity.HasOne(x => x.Role)
                    .WithMany(x => x.UserRoles)
                    .HasForeignKey(x => x.RoleId);
            });

            modelBuilder.Entity<RolePermission>()
    .HasKey(x => new { x.RoleId, x.PermissionId });

            modelBuilder.Entity<RolePermission>()
                .HasOne(x => x.Role)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(x => x.Permission)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.PermissionId);
      }
}