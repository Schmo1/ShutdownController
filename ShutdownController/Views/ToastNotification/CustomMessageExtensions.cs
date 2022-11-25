using ToastNotifications.Core;
using ToastNotifications;

namespace ShutdownController.Views.ToastNotification
{
    public static class CustomMessageExtensions
    {
        public static void ShowCustomMessage(this Notifier notifier, string title, string message, bool mirroring = false, MessageOptions messageOptions = null)
        {
            notifier.Notify(() => new CustomNotification(title, message, mirroring, messageOptions));
        }
    }
}
