using System.Text.RegularExpressions;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShutdownController.Views
{
    /// <summary>
    /// Interaktionslogik für DiskView.xaml
    /// </summary>
    public partial class DiskView : UserControl
    {


        private static readonly Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text

        private static readonly Regex regexWithDigits = new Regex(@"[0 - 9] + (\.[0 - 9]+)?"); //regex that matches disallowed text

        public DiskView()
        {
            InitializeComponent();
        }

        private void PreviewTextInputOnlyNumbers(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text, false);
        }

        // Use the DataObject.Pasting Handler 
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text, false))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }


        private void PreviewTextInputOnlyNumbersAndPoint(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text, true);
        }

        // Use the DataObject.Pasting Handler 
        private void TextBoxPastingWithPoint(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text, true))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }


        private static bool IsTextAllowed(string text, bool withDecimalPoint)
        {
            if (withDecimalPoint)
                return !regexWithDigits.IsMatch(text);
            else
                return !regex.IsMatch(text);
        }
    }
}

