using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShutdownController.Configuration;
using ShutdownController.ViewModels;
using ShutdownController.Views;
using System.IO;
using Serilog;
using System.Windows;
using System.Windows.Threading;
using ShutdownController.Services;

namespace ShutdownController;


public partial class App : Application
{
	private IHost _host;

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

		await _host.StartAsync();
	}

	private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
	{
		// Configure Serilog
		Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(context.Configuration)
			.CreateLogger();

		services.AddSingleton<ILogger>(Log.Logger);

		services.AddHostedService<ApplicationHostService>();
		services.AddSingleton<MainWindow>();

		services.AddSingleton<MainViewModel>();

		services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
	}

	private async void OnExit(object sender, ExitEventArgs e)
	{
		await _host.StopAsync();
		_host.Dispose();
		_host = null;
	}

	private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
	{
		// TODO: Please log and handle the exception as appropriate to your scenario
		// For more info see https://docs.microsoft.com/dotnet/api/system.windows.application.dispatcherunhandledexception?view=netcore-3.0
	}

}
