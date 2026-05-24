using Microsoft.EntityFrameworkCore;
using FindJobAPI.Models;

namespace FindJobAPI.Data;

public class FindJobContext : DbContext
{
    public FindJobContext(DbContextOptions<FindJobContext> options) : base(options) { }

    public DbSet<JobPost> JobPosts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Subcategory> Subcategories { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<JobType> JobTypes { get; set; }
    public DbSet<BadReport> BadReports { get; set; }
    public DbSet<Administrator> Administrators { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobPost>().ToTable("FJ_JobsPosts");
        modelBuilder.Entity<Category>().ToTable("FJ_Categorys_Of_Works");
        modelBuilder.Entity<Subcategory>().ToTable("FJ_SubCategory_Work");
        modelBuilder.Entity<Region>().ToTable("FJ_Region");
        modelBuilder.Entity<City>().ToTable("FJ_Citys");
        modelBuilder.Entity<JobType>().ToTable("FJ_JobType");
        modelBuilder.Entity<BadReport>().ToTable("FJ_BadReports");
        modelBuilder.Entity<Administrator>().ToTable("FJ_Administrators");

        modelBuilder.Entity<Subcategory>()
            .HasOne(s => s.Category)
            .WithMany(c => c.Subcategories)
            .HasForeignKey(s => s.CategoryId);

        modelBuilder.Entity<City>()
            .HasOne(c => c.Region)
            .WithMany(r => r.Cities)
            .HasForeignKey(c => c.RegionId);

        modelBuilder.Entity<BadReport>()
            .HasOne(b => b.Post)
            .WithMany()
            .HasForeignKey(b => b.PostId);
    }
}
