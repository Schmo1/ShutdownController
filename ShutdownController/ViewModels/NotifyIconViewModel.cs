using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ShutdownController.Commands;
using ShutdownController.Core;
using ShutdownController.Utility;


namespace ShutdownController.ViewModels
{
    public class NotifyIconViewModel : ObservableObject
    {


        private string _sysTrayMenuText;
        private ImageSource _showIcon;
        private ImageSource _hideIcon;


        public NotifyIconViewModel()
        {
            SystemTrayMenuText = Assembly.GetExecutingAssembly().GetName().Name;
        }



        /// <summary>
        /// Shows a window, if none is already open.
        /// </summary>
        public ICommand ShowWindowCommand 
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        MyLogger.Instance().Info("Show Window pressed");
                        Application.Current.MainWindow = new MainWindow();
                        Application.Current.MainWindow.Show();
                    },

                    CanExecuteFunc = () =>
                    {
                        if(Application.Current.MainWindow != null)
                        {
                            if (!Application.Current.MainWindow.IsVisible)
                                return true;
                            
                            return false;
                        }
                        else
                            return false;
                        
                    }
                };
            }
        }

        /// <summary>
        /// Hides the main window. This command is only enabled if a window is open.
        /// </summary>
        public ICommand HideWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () => 
                    { 
                        MyLogger.Instance().Info("Hide Window pressed");
                        Application.Current.MainWindow.Close();
                    },
                    CanExecuteFunc = () => {
                        if (Application.Current.MainWindow != null)
                        {
                            if (Application.Current.MainWindow.IsVisible)
                                return true;
                            
                            return false;
                        }
                        else
                            return true;
                        
                    }
                };
            }
        }


        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => Application.Current.Shutdown()};
            }
        }

        public ICommand ShowWindowDoubleClickCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => true,

                    CommandAction = () =>
                    {
                        MyLogger.Instance().Info("DoubleClick on TrayIcon Pressed => Open MainWindow");
                        Application.Current.MainWindow = new MainWindow();
                        Application.Current.MainWindow.Show();
                    }
                };
            }
        }


        public string SystemTrayMenuText
        {
            get { return _sysTrayMenuText; }
            set {_sysTrayMenuText = value; OnPropertyChanged(); }
        }


        public ImageSource ShowIcon { get => _showIcon; private set { _showIcon = value; OnPropertyChanged(); } }
        public ImageSource HideIcon { get => _hideIcon; private set { _hideIcon = value; OnPropertyChanged(); } }

       
    }
}
