using System.Windows;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuickPOS.Core;
using Ardalis.Specification;
using QuickPOS.Data;
using QuickPOS.Features.Auth;
using QuickPOS.Features.DailyClose;
using QuickPOS.Features.Dashboard;
using QuickPOS.Features.Inventory;
using QuickPOS.Features.Inventory;
using QuickPOS.Features.Products;
using QuickPOS.Features.Selling;
using QuickPOS.Features.Users;
using QuickPOS.Models.Entities;

namespace QuickPOS;

public partial class App : Application
{
    private readonly ServiceProvider _serviceProvider;

    static App()
    {
        var culture = (CultureInfo)CultureInfo.GetCultureInfo("fr-DZ").Clone();
        culture.NumberFormat.CurrencySymbol = "DZD";
        culture.NumberFormat.CurrencyPositivePattern = 3; // n DZD
        culture.NumberFormat.CurrencyNegativePattern = 8; // -n DZD
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(
                System.Windows.Markup.XmlLanguage.GetLanguage(culture.IetfLanguageTag)));
    }

    public IServiceProvider Services => _serviceProvider;

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Database
        services.AddDbContextFactory<QuickPosDbContext>(options =>
            options.UseNpgsql("Host=localhost;Database=quickpos;Username=postgres;Password=postgres"));

        // Identity
        services.AddIdentityCore<AppIdentityUser>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 4;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
        .AddRoles<AppIdentityRole>()
        .AddEntityFrameworkStores<QuickPosDbContext>();

        // Repositories
        services.AddScoped(typeof(IRepositoryBase<>), typeof(EfRepository<>));

        // Stock service
        services.AddScoped<IStockService, StockService>();

        // User settings
        services.AddSingleton<UserSettingsService>();

        // Cart service (singleton for app-wide state)
        services.AddSingleton<CartService>();

        // Authentication
        services.AddSingleton<CurrentUserProvider>();
        services.AddSingleton<ICurrentUserProvider>(sp => sp.GetRequiredService<CurrentUserProvider>());
        services.AddSingleton<AuthenticationService>();

        // Auditing
        services.AddSingleton<AuditableInterceptor>();

        // Navigation
        services.AddSingleton<NavigationService>(sp => new NavigationService(sp));

        // ViewModels
        services.AddTransient<SellingViewModel>();
        services.AddTransient<ProductsViewModel>();
        services.AddTransient<InventoryBatchViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<UsersViewModel>();
        services.AddTransient<DailyCloseViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<MainViewModel>();

        // Windows
        services.AddTransient<LoginWindow>();
        services.AddTransient<MainWindow>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Ensure database is created and migrated
        using (var scope = _serviceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<QuickPosDbContext>();
            db.Database.Migrate();
        }

        var settings = _serviceProvider.GetRequiredService<UserSettingsService>();
        settings.Load();
        LocalizationService.Instance.Language = settings.Settings.Language;

        ShowLoginLoop();
    }

    /// <summary>Runs the login → main loop. Called on startup and after every logout.</summary>
    public void ShowLoginLoop()
    {
        // Close any existing main window without triggering shutdown
        var existing = Current.MainWindow;
        if (existing is MainWindow) { existing.Hide(); }

        var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
        if (loginWindow.ShowDialog() == true)
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            Current.MainWindow = mainWindow;
            mainWindow.Show();
        }
        else
        {
            Shutdown();
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        var settings = _serviceProvider.GetRequiredService<UserSettingsService>();
        settings.Settings.Language = LocalizationService.Instance.Language;
        settings.Save();
        _serviceProvider.Dispose();
        base.OnExit(e);
    }
}
