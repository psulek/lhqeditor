using System.Windows;
using Template.Desktop.WPF.Models;

namespace Template.Desktop.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowModel();
        }
    }
}
