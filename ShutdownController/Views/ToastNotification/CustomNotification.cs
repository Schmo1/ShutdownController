using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToastNotifications.Core;

namespace ShutdownController.Views.ToastNotification
{
    public class CustomNotification : NotificationBase, INotifyPropertyChanged
    {
        private CustomDisplayPart _displayPart;

        public override NotificationDisplayPart DisplayPart => _displayPart ?? (_displayPart = new CustomDisplayPart(this));

        public CustomNotification(string title, string message, bool mirroring = false, MessageOptions messageOptions = null) : base(message, messageOptions)
        {
            Title = title;
            Message = message;
            Mirroring = mirroring;
        }


        private bool _mirroring;
        public bool Mirroring
        {
            get
            {
                return _mirroring;
            }
            set
            {
                _mirroring = value;
                OnPropertyChanged();
            }
        }


        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
