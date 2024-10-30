using Microsoft.EntityFrameworkCore;
using OrderFiltering.Models;

namespace OrderFiltering.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    {
    }

    public DbSet<OrderModel> Orders { get; set; }
    public DbSet<DistrictModel> Districts { get; set; }
    public DbSet<FilteredOrderModel> FilteredOrders { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}