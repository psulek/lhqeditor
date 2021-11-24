using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Template.Desktop.WPF.Models
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        public MainWindowModel()
        {
            WindowsUser = Environment.UserName;

            var activeCultureName = StringsContext.Instance.Culture.Name;

            AvailableCultures = new ObservableCollection<CultureViewModel>(StringsContext.Instance.AvailableCultures
                .Select(x => new CultureViewModel(x, x == activeCultureName, this)));
        }

        public string WindowsUser { get; set; }

        public ObservableCollection<CultureViewModel> AvailableCultures { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void ActiveCultureChanged()
        {
            foreach (var cultureViewModel in AvailableCultures)
            {
                cultureViewModel.RefreshProperties();
            }
        }
    }
}