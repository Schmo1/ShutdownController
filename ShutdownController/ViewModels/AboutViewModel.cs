using System;
using System.Reflection;
using System.Windows;
using ShutdownController.Core;

namespace ShutdownController.ViewModels
{
    public class AboutViewModel
    {
        private readonly Window _window;


        public string AssemblyName { get => Assembly.GetEntryAssembly().GetName().Name; }
        public string VersionNumber { get => Assembly.GetEntryAssembly().GetName().Version.ToString(); }
        public string CopyRight
        {
            get
            {
                Assembly currentAssem = typeof(AboutViewModel).Assembly;
                object[] attribs = currentAssem.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
                string copyright = string.Empty;

                if (attribs.Length > 0)
                    copyright = ((AssemblyCopyrightAttribute)attribs[0]).Copyright;
                

                if (!string.IsNullOrEmpty(copyright))
                    return copyright;
                else
                    return string.Empty;
            }
        }

        public string Company 
        { 
            get
            {
                Assembly currentAssem = typeof(AboutViewModel).Assembly;
                object[] attribs = currentAssem.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
                string company = string.Empty;

                if (attribs.Length > 0)
                    company = ((AssemblyCompanyAttribute)attribs[0]).Company;
                

                if (!string.IsNullOrEmpty(company))
                    return company;
                else
                    return string.Empty;
            } 
        }

        public CommandHandler CloseAction { get; set; }

        public AboutViewModel(Window window)
        {
            _window = window;
            CloseAction = new CommandHandler(() => _window?.Close(), () => true);     
        }


    }
}
