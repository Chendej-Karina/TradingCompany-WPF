using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TradingCompany.BLL.Security;
using TradingCompany.BLL.Services;
using TradingCompany.BLL.Validation;
using TradingCompany.DAL;
using TradingCompany.DAL.Data;
using TradingCompany.DAL.Interfaces;
using TradingCompany.ViewModels;
using TradingCompany.ViewModels.Basic;
using TradingCompany.WPF.Infrastructure;

namespace TradingCompany.WPF;

public partial class App : Application
{
    private ServiceProvider? _provider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // дати й числа відображаються за регіональними налаштуваннями користувача
        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var services = new ServiceCollection();
        ConfigureServices(services, configuration.GetConnectionString("TradingCompany")!);
        _provider = services.BuildServiceProvider();

        var window = _provider.GetRequiredService<MainWindow>();
        _provider.GetRequiredService<ShellViewModel>().Start();
        window.Show();
    }

    /// <summary>Реєстрація залежностей (Dependency Injection).</summary>
    private static void ConfigureServices(IServiceCollection services, string connectionString)
    {
        // Singleton: спільний стан застосунку
        services.AddSingleton<IUserSession, UserSession>();
        services.AddSingleton<IPermissionService, PermissionService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IAuctionValidator, AuctionValidator>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ShellViewModel>();
        services.AddSingleton<MainWindow>();

        // Scoped: для кожної сторінки створюється власний scope (і власний DbContext)
        services.AddDbContext<TradingCompanyContext>(
            options => options.UseSqlServer(connectionString), ServiceLifetime.Scoped);

        services.AddScoped<IUserDal, UserDal>();
        services.AddScoped<IItemDal, ItemDal>();
        services.AddScoped<IAuctionDal, AuctionDal>();
        services.AddScoped<ICurrencyDal, CurrencyDal>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<IAuctionService, AuctionService>();

        services.AddScoped<LoginViewModel>();
        services.AddScoped<HomeViewModel>();
        services.AddScoped<NoAccessViewModel>();
        services.AddScoped<ItemListViewModel>();
        services.AddScoped<AuctionListViewModel>();
        services.AddScoped<AuctionEditViewModel>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _provider?.Dispose();
        base.OnExit(e);
    }
}
