using Microsoft.Extensions.DependencyInjection;
using ShutdownController.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShutdownController.Views;


public partial class ClockView : UserControl
{


	[GeneratedRegex("[^0-9]+")]
	private static partial Regex NumberRegex();

	public ClockView()
	{
		InitializeComponent();

		DataContext = App.Services.GetRequiredService<ClockViewModel>();
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
			string text = (string)e.DataObject.GetData(typeof(string));
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
		return !NumberRegex().IsMatch(text);
	}
}
