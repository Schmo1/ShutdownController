using Microsoft.Extensions.Hosting;

using ShutdownController.Views;
using ShutdownController.ViewModels;

namespace ShutdownController.Services;

public class ApplicationHostService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private bool _isInitialized;

    public ApplicationHostService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
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
        //_persistAndRestoreService.PersistData();
        return Task.CompletedTask;
    }

    private Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            //_persistAndRestoreService.RestoreData();
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


        //if (App.Current.Windows.OfType<IShellWindow>().Count() == 0)
        //{
        //    // Default activation that navigates to the apps default page
        //    //_shellWindow = _serviceProvider.GetService(typeof(IShellWindow)) as IShellWindow;
        //    //_navigationService.Initialize(_shellWindow.GetNavigationFrame());
        //    //_shellWindow.ShowWindow();
        //    //_navigationService.NavigateTo(typeof(MainViewModel).FullName);
        //    //await Task.CompletedTask;
        //}
        return Task.CompletedTask;
	}
}
