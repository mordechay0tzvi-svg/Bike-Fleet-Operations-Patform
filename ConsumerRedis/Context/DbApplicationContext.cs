using Microsoft.EntityFrameworkCore;
using Models;
namespace DataBase;
public class DbAppContext : DbContext
{
    public DbAppContext(DbContextOptions<DbAppContext> options): base(options){}
    public DbSet<StationInformation> StationInformation => Set<StationInformation>();
    public DbSet<VehicleType> VehicleTypes => Set<VehicleType>();
}

