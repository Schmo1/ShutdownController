
using ToastNotifications.Core;

namespace ShutdownController.Views.ToastNotification
{
    /// <summary>
    /// Interaktionslogik für CustomDisplayPart.xaml
    /// </summary>
    public partial class CustomDisplayPart : NotificationDisplayPart
    {
        public CustomDisplayPart(CustomNotification customNotification)
        {
            InitializeComponent();
            Bind(customNotification);
        }
    }
}
