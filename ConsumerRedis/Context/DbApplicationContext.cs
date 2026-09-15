using Microsoft.EntityFrameworkCore;
using Models;
namespace DataBase;
public class DbAppContext : DbContext
{
    public DbAppContext(DbContextOptions<DbAppContext> options): base(options){}
    public DbSet<StationInformation> StationInformation => Set<StationInformation>();
    public DbSet<StationStatus> StationStatus => Set<StationStatus>();
    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     modelBuilder.Entity<StationInformation>()
    //         .HasOne(s => s.Status)
    //         .WithOne(s => s.Station)
    //         .HasForeignKey<StationStatus>(s => s.StationId)
    //         .OnDelete(DeleteBehavior.Cascade);
    // }
}

