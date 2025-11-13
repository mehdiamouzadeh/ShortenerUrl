using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using ShortenerUrl.Models;

namespace ShortenerUrl.Infrastructure;

public class ShortenUrlDbContext : DbContext
{ 
    public ShortenUrlDbContext(DbContextOptions<ShortenUrlDbContext> options) : base(options)
    {
        
    }

    public DbSet<UrlTag> UrlTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UrlTag>().ToCollection("UrlTags");
    }

}

