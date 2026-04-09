using dmb_backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace dmb_backend.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<NewsItem> NewsItems => Set<NewsItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<NewsItem>(e =>
        {
            e.ToTable("news_items");
            e.HasKey(x => x.Id);

            e.Property(x => x.Title)
                .HasMaxLength(140)
                .IsRequired();

            e.Property(x => x.Text)
                .HasColumnName("Content")
                .HasMaxLength(4000)
                .IsRequired();

            e.Property(x => x.CreatedAtUtc)
                .IsRequired();

            e.Property(x => x.CreatedByUserId)
                .HasMaxLength(255);
        });
    }
}