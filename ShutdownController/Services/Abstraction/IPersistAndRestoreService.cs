namespace ShutdownController.Services.Abstraction;

public interface IPersistAndRestoreService
{
    void RestoreData();

    void PersistData();
}
