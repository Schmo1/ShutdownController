using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace ShutdownController.Views
{
    /// <summary>
    /// Interaktionslogik für TimerView.xaml
    /// </summary>
    public partial class TimerView : UserControl
    {


        private static readonly Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text


        public TimerView()
        {
            InitializeComponent();
        }



        private void PreviewTextInputOnlyNumbers(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        // Use the DataObject.Pasting Handler 
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private static bool IsTextAllowed(string text)
        {
            return !regex.IsMatch(text);
        }
    }
}
