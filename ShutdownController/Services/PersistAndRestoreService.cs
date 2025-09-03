using System.Collections;
using System.IO;
using ShutdownController.Services.Abstraction;
using Microsoft.Extensions.Options;
using ShutdownController.Configuration;


namespace ShutdownController.Services;

public class PersistAndRestoreService : IPersistAndRestoreService
{
    private readonly IFileService _fileService;
    private readonly AppConfig _appConfig;
    private readonly string _localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    public PersistAndRestoreService(IFileService fileService, IOptions<AppConfig> appConfig)
    {
        _fileService = fileService;
        _appConfig = appConfig.Value;
    }

    public void PersistData()
    {
        if (App.Current.Properties is not null)
        {
            string folderPath = Path.Combine(_localAppData, _appConfig.ConfigurationsFolder);
            _fileService.Save(folderPath, _appConfig.AppPropertiesFileName, App.Current.Properties);
        }
    }

    public void RestoreData()
    {
        string folderPath = Path.Combine(_localAppData, _appConfig.ConfigurationsFolder);
        IDictionary? properties = _fileService.Read<IDictionary>(folderPath, _appConfig.AppPropertiesFileName);

        if (properties is not null)
        {
            foreach (DictionaryEntry property in properties)
            {
                App.Current.Properties.Add(property.Key, property.Value);
            }
        }
    }
}
