using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToastNotifications.Core;

namespace ShutdownController.Views.ToastNotification
{
    public enum CustomNotificationArrowPosition
    {
        Left,
        Right,
        Top,
        Bottom
    }


    public class CustomNotification : NotificationBase, INotifyPropertyChanged
    {
        private CustomDisplayPart _displayPart;

        public override NotificationDisplayPart DisplayPart => _displayPart ?? (_displayPart = new CustomDisplayPart(this));

        public CustomNotification(string title, string message, CustomNotificationArrowPosition position = CustomNotificationArrowPosition.Left, MessageOptions messageOptions = null) : base(message, messageOptions)
        {
            Title = title;
            Message = message;
            PositionArrow = position;
        }


        private CustomNotificationArrowPosition _positionArrow;
        public CustomNotificationArrowPosition PositionArrow
        {
            get
            {
                return _positionArrow;
            }
            set
            {
                _positionArrow = value;
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
