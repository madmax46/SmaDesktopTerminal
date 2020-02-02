using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SmaDesktopTerminal.Models;

namespace SmaDesktopTerminal.Windows
{
    /// <summary>
    /// Логика взаимодействия для LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        private LogInWindowModel model;
        public LogInWindow(LogInWindowModel model)
        {
            InitializeComponent();
            this.model = model;
        }

        private void LogInWindow_OnClosing(object sender, CancelEventArgs e)
        {
            model?.ShutDownApp();
        }
    }
}
