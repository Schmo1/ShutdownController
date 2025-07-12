using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using ShutdownController.Utility;

namespace ShutdownController.Views
{
    /// <summary>
    /// Interaktionslogik für UpDownloadView.xaml
    /// </summary>
    public partial class DownUploadView : UserControl
    {

    
        public DownUploadView()
        {
            InitializeComponent();
        }

        private void PreviewTextInputOnlyNumbers(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ParseInputBox.IsOnlyNumber(e.Text);

        }
        private void PreviewTextInputOnlyNumbersAndPoint(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ParseInputBox.IsOnlyNumberOrPoint(e.Text);
        }



        // Use the DataObject.Pasting Handler 
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!int.TryParse(text, out _))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }



        // Use the DataObject.Pasting Handler 
        private void TextBoxPastingDouble(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!double.TryParse(text, out _))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
