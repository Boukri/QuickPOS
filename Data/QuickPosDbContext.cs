using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuickPOS.Models.Entities;

namespace QuickPOS.Data;

public class QuickPosDbContext : IdentityDbContext<AppIdentityUser, AppIdentityRole, int>
{
    private readonly AuditableInterceptor? _auditableInterceptor;

    public QuickPosDbContext(DbContextOptions<QuickPosDbContext> options,
        AuditableInterceptor? auditableInterceptor = null) : base(options)
    {
        _auditableInterceptor = auditableInterceptor;
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<CategoryModel> Categories => Set<CategoryModel>();
    public DbSet<StockBatch> StockBatches => Set<StockBatch>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<TransactionItem> TransactionItems => Set<TransactionItem>();
    public DbSet<DailyClose> DailyCloses => Set<DailyClose>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));

        if (_auditableInterceptor is not null)
            optionsBuilder.AddInterceptors(_auditableInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // apply configuation from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(QuickPosDbContext).Assembly);


        // Seed data
        var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Seed default admin user
        // Password: Admin@123
        // Pre-computed hash to avoid non-deterministic seed data
        modelBuilder.Entity<AppIdentityUser>().HasData(new AppIdentityUser
        {
            Id = 1,
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@quickpos.com",
            NormalizedEmail = "ADMIN@QUICKPOS.COM",
            EmailConfirmed = true,
            FullName = "System Administrator",
            SecurityStamp = "ADMIN-SECURITY-STAMP-2025",
            PasswordHash = "AQAAAAEAACcQAAAAECbLZ1M7aAxA94L+mfakwTQlMixLNNtmYTROOdK1mEgCO65f6U67LySuflv30rBQRQ==" // Admin@123
        });

        // Seed admin role
        modelBuilder.Entity<AppIdentityRole>().HasData(new AppIdentityRole
        {
            Id = 1,
            Name = "Admin",
            NormalizedName = "ADMIN",
            Description = "Full system access"
        });

        // Assign admin role to admin user
        modelBuilder.Entity<IdentityUserRole<int>>().HasData(new IdentityUserRole<int>
        {
            UserId = 1,
            RoleId = 1
        });

        // Seed all permissions for admin
        var adminPermissions = Permissions.AllPermissions.Select((perm, idx) => new UserPermission
        {
            Id = idx + 1,
            UserId = 1,
            PermissionKey = perm
        }).ToArray();

        modelBuilder.Entity<UserPermission>().HasData(adminPermissions);
    }
}
