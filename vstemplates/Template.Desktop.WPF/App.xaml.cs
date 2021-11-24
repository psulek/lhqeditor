using System.Globalization;
using System.Windows;
using ScaleHQ.WPF.LHQ;

namespace Template.Desktop.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            LocalizationExtension.Initialize(new LocalizationContextFactoryDefault(() => StringsContext.Instance));
            // Set current culture to english
            StringsContext.Instance.Culture = new CultureInfo("en");
            base.OnStartup(e);
        }
    }
}
