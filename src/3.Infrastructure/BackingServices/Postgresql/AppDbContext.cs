using Microsoft.EntityFrameworkCore;
using PocLineAPI.Domain;

namespace PocLineAPI.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Document> Documents { get; set; }

    // Add DbSet for other entities here
}

