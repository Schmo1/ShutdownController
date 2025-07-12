using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShutdownController.Configuration;
using ShutdownController.ViewModels;
using ShutdownController.Views;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace ShutdownController;


public partial class App : Application
{
	private IHost _host;

	private void OnStartup(object sender, StartupEventArgs e)
	{
		var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

		_host = Host.CreateDefaultBuilder(e.Args)
			   .ConfigureAppConfiguration(c =>
			   {
				   c.SetBasePath(appLocation);
			   })
			   .ConfigureServices(ConfigureServices)
			   .Build();
	}

	private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
	{
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
