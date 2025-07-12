using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ShutdownController.Views
{
    /// <summary>
    /// Interaktionslogik für ToggleButton.xaml
    /// </summary>
    public partial class ToggleButton : UserControl
    {


        public static readonly DependencyProperty ToggledProperty = DependencyProperty.Register
            ("Toggled", typeof(bool), typeof(ToggleButton), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ToggleChangedCallback));




        private void ToggleChangedCallback(bool value)
        {
            //...
            
        }

        private static void ToggleChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ToggleButton)d).ToggleChangedCallback((bool)e.NewValue);
        }



        public bool Toggled
        {
            get => (bool)GetValue(ToggledProperty);
            set => SetValue(ToggledProperty, value);
        }




        public ToggleButton()
        {
            InitializeComponent();

        }


        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Toggled = !Toggled && IsEnabled;
            
        }



        
    }
}
