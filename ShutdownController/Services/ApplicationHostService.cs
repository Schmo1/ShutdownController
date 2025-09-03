using Microsoft.Extensions.Hosting;

using ShutdownController.Views;
using ShutdownController.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using ShutdownController.Services.Abstraction;

namespace ShutdownController.Services;

public class ApplicationHostService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IPersistAndRestoreService _persistAndRestoreService;
    private bool _isInitialized;
    private MainWindow _mainWindow;

    public ApplicationHostService(IServiceProvider serviceProvider, IPersistAndRestoreService persistAndRestoreService)
    {
        _serviceProvider = serviceProvider;
        this._persistAndRestoreService = persistAndRestoreService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Initialize services that you need before app activation
        await InitializeAsync();

        await HandleActivationAsync();

        // Tasks after activation
        await StartupAsync();
        _isInitialized = true;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _persistAndRestoreService.PersistData();
        return Task.CompletedTask;
    }

    private Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            _persistAndRestoreService.RestoreData();
        }
        return Task.CompletedTask;
    }

    private Task StartupAsync()
    {
        if (!_isInitialized)
        {
            //_toastNotificationsService.ShowToastNotificationSample();
        }
        return Task.CompletedTask;
    }

    private Task HandleActivationAsync()
    {
        //var activationHandler = _activationHandlers.FirstOrDefault(h => h.CanHandle());

        //if (activationHandler != null)
        //{
        //    await activationHandler.HandleAsync();
        //}


        if (!System.Windows.Application.Current.Windows.OfType<MainWindow>().Any())
        {
            _mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            _mainWindow.Show();
            return Task.CompletedTask;
        }
        return Task.CompletedTask;
	}
}
