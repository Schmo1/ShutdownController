using ToastNotifications.Core;
using ToastNotifications;

namespace ShutdownController.Views.ToastNotification
{
    public static class CustomMessageExtensions
    {
        public static void ShowCustomMessage(this Notifier notifier, string title, string message, CustomNotificationArrowPosition arrowPosition = CustomNotificationArrowPosition.Left, MessageOptions messageOptions = null)
        {
            notifier.Notify(() => new CustomNotification(title, message, arrowPosition, messageOptions));
        }
    }
}
