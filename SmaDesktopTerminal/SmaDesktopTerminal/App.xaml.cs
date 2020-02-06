using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using SmaDesktopTerminal.Models;

namespace SmaDesktopTerminal
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\LiveCharts");

            //ServicePointManager
            //        .ServerCertificateValidationCallback +=
            //    (sender, cert, chain, sslPolicyErrors) => true;

            AppMainModel terminalMainModel = new AppMainModel();
        }

        //public ResourceDictionary ThemeDictionary
        //{
        //    // You could probably get it via its name with some query logic as well.
        //    get { return Resources.MergedDictionaries[0]; }
        //}

        //public void ChangeTheme(Uri uri)
        //{
        //    ThemeDictionary.MergedDictionaries.Clear();
        //    ThemeDictionary.MergedDictionaries.Add(new ResourceDictionary() { Source = uri });
        //}
    }
}
