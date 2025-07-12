using System.Text.RegularExpressions;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using ShutdownController.Utility;
using System.Globalization;

namespace ShutdownController.Views
{
    /// <summary>
    /// Interaktionslogik für DiskView.xaml
    /// </summary>
    public partial class DiskView : UserControl
    {


        public DiskView()
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

