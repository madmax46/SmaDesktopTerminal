using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SmaDesktopTerminal.Models
{
    public static class ThemesController
    {
        private static Dictionary<string, Uri> _urisToThemes = new Dictionary<string, Uri>();
        //private static string currentTheme = "light";
        static ThemesController()
        {
            _urisToThemes.Add("dark", new Uri(@"Themes\dark.xaml", UriKind.Relative));
            _urisToThemes.Add("light", new Uri(@"Themes\light.xaml", UriKind.Relative));
        }


        public static void ApplyDarkTheme()
        {
            var uri = _urisToThemes["dark"];
            ApplyTheme(uri);
        }
        public static void ApplyLightTheme()
        {
            var uri = _urisToThemes["light"];
            ApplyTheme(uri);
        }

        public static void ApplyTheme(Uri themeUri)
        {
            ResourceDictionary dictionary = GetThemeResourceDictionary(themeUri);
            if (dictionary == null)
                return;

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dictionary);
        }


        public static ResourceDictionary GetThemeResourceDictionary(Uri theme)
        {
            if (theme != null)
            {
                return Application.LoadComponent(theme) as ResourceDictionary;
            }
            return null;
        }
    }
}
