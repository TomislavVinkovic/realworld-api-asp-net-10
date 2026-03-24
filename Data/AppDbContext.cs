using RealWorld.Models;
using Microsoft.EntityFrameworkCore;

namespace RealWorld.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Tag> Tags { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = DateTime.UtcNow;

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Article>()
            .HasIndex(a => a.Slug)
            .IsUnique();

        modelBuilder.Entity<Article>()
            .HasOne(a => a.Author)
            .WithMany(u => u.WrittenArticles)
            .HasForeignKey(a => a.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Article>()
            .HasMany(a => a.FavoritedBy)
            .WithMany(u => u.FavoritedArticles)
            .UsingEntity(j => j.ToTable("UserFavorites"));

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Article)
            .WithMany(a => a.Comments)
            .HasForeignKey(c => c.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Following)
            .WithMany(u => u.Followers)
            .UsingEntity<Dictionary<string, object>>(
                "UserFollows",
                j => j.HasOne<User>().WithMany().HasForeignKey("FollowingId"),
                j => j.HasOne<User>().WithMany().HasForeignKey("FollowerId")
            );
    }
}
