using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using ShutdownController.Configuration;
using ShutdownController.Services;
using ShutdownController.Services.Abstraction;
using ShutdownController.Services.Extensions;
using ShutdownController.ViewModels;
using ShutdownController.Views;
using ShutdownController.Views.MessageBox;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace ShutdownController;


public partial class App : Application
{
	private IHost _host;

	public static IServiceProvider Services { get; private set; }

	private async void OnStartup(object sender, StartupEventArgs e)
	{
		_host = Host.CreateDefaultBuilder(e.Args)
	   .ConfigureAppConfiguration(c =>
	   {
		   c.SetBasePath(Directory.GetCurrentDirectory())
		   .AddJsonFile("appsettings.json", false, false)
		   .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DEBUG") ?? "Production"}.json", true);
	   })
	   .ConfigureServices(ConfigureServices)
	   .Build();
		
		Services = _host.Services;

		await _host.StartAsync();
	}

	private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
	{
            Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(context.Configuration)
			.CreateLogger();
		services.AddServices();
		services.AddHostedService<ApplicationHostService>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IPersistAndRestoreService, PersistAndRestoreService>();
        services.AddSingleton(Log.Logger);
		services.AddSingleton<MainWindow>();
		services.AddSingleton<ShutdownOptionsView>();
		services.AddSingleton<SettingsView>();
		services.AddSingleton<DownUploadView>();
		services.AddSingleton<DiskView>();
		services.AddSingleton<TimerView>();
		services.AddSingleton<ClockView>();

		// The message box window is closed after every trigger, so a fresh
		// instance (with a fresh countdown) is needed for each activation.
		services.AddTransient<CustomMessageBoxView>();
		services.AddTransient<CustomMessageBoxViewModel>();

		services.AddSingleton<MainWindowViewModel>();
		services.AddSingleton<ShutdownOptionsViewModel>();
		services.AddSingleton<DiskViewModel>();
		services.AddSingleton<DownUploadViewModel>();
		services.AddSingleton<SettingsViewModel>();
		services.AddSingleton<TimerViewModel>();
		services.AddSingleton<ClockViewModel>();



		services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
		
	}
	


	private async void OnExit(object sender, ExitEventArgs e)
	{
		await _host.StopAsync();
		_host.Dispose();
		_host = null!;
	}


	private async void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
	{
		try
		{
			Log.Logger.Error(e.Exception, "Dispatcher unhandled exception occured");
		}
		catch (Exception)
		{ 
			//do nothing
		}

		await _host.StopAsync();
		_host.Dispose();
		_host = null; 
	}
}
