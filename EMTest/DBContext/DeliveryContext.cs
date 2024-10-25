using EMTest.DTO;
using Microsoft.EntityFrameworkCore;

namespace EMTest.DBContext;

public class DeliveryContext: DbContext   
{
    public DbSet<DeliveryOrder> Orders { get; set; }
    public DbSet<LogEntry> Logs { get; set; }

    public DeliveryContext(DbContextOptions<DeliveryContext> options) : base(options)
    {
    }
}