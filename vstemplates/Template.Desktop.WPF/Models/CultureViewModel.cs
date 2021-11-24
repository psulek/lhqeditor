using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Template.Desktop.WPF.Models
{
    public class CultureViewModel : INotifyPropertyChanged
    {
        private bool _active;
        private readonly MainWindowModel _parentViewModel;

        public CultureViewModel(string cultureName, bool active, MainWindowModel parentViewModel)
        {
            CultureInfo = new CultureInfo(cultureName);
            Active = active;
            _parentViewModel = parentViewModel;
        }

        public CultureInfo CultureInfo { get; set; }

        public string DisplayName
        {
            get
            {
                string localizedLanguageName = StringsContext.Instance.GetStringSafely(nameof(Strings.Languages) + CultureInfo.Name);
                return $"{localizedLanguageName} [{CultureInfo.Name}] ({CultureInfo.EnglishName})";
            }
        }
            

        public bool Active
        {
            get { return _active; }
            set
            {
                if (value != _active)
                {
                    _active = value;
                    OnPropertyChanged(nameof(Active));
                    if (_active)
                    {
                        StringsContext.Instance.Culture = CultureInfo;
                    }

                    _parentViewModel?.ActiveCultureChanged();
                }
            }
        }

        internal void RefreshProperties()
        {
            OnPropertyChanged(null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}