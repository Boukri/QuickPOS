using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace QuickPOS.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<QuickPosDbContext>
{
    public QuickPosDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<QuickPosDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=quickpos;Username=postgres;Password=postgres");
        return new QuickPosDbContext(optionsBuilder.Options);
    }
}
