#region License
// Copyright (c) 2021 Peter Šulek / ScaleHQ Solutions s.r.o.
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Dialogs;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Dialogs;
using LHQ.Utils.Extensions;

namespace LHQ.App.ViewModels
{
    public class HintPanelViewModel : ShellViewModelBase
    {
        private readonly AppHintType _appHintType;
        private bool _visible;
        private List<BulletListItem> _itemsSource;
        private bool _closeButtonVisible;

        public HintPanelViewModel(IShellViewContext shellViewContext, AppHintType appHintType)
            : base(shellViewContext, false)
        {
            _appHintType = appHintType;
            CloseCommand = new DelegateCommand(CloseCommandExecute);
            ReadUpdateChangedCommand = new DelegateCommand(ReadUpdateChangedExecute);
            AppContext.OnAppEvent += HandleAppEvent;
            ShellViewModel.PropertyChanged += ShellViewOnPropertyChanged;
            CloseButtonVisible = true;
            DataBind();
        }

        public ICommand CloseCommand { get; }

        public ICommand ReadUpdateChangedCommand { get; }

        public List<BulletListItem> ItemsSource
        {
            get => _itemsSource;
            set
            {
                _itemsSource = value;
                RaisePropertyChanged(nameof(ItemsSource));
            }
        }

        public bool CloseButtonVisible
        {
            get => _closeButtonVisible;
            set => SetProperty(ref _closeButtonVisible, value);
        }

        public bool Visible
        {
            get => _visible;
            set => SetProperty(ref _visible, value);
        }

        private void DataBind()
        {
            Visible = AppContext.AppConfigFactory.Current.AppHints.IsFlagSet(_appHintType);

            if (_appHintType == AppHintType.NewVersion)
            {
                Visible = AppContext.UpdateInfo?.HasNewerVersion == true;
                CloseButtonVisible = false;
            }

            var items = new List<BulletListItem>();
            switch (_appHintType)
            {
                case AppHintType.CodeGenerator:
                {
                    if (!AppContext.RunInVsPackage)
                    {
                        items.Add(BulletListItem.Item(Strings.ViewModels.HintPanel.CodeGenerator.HintTextOnlyInVS, FontWeights.DemiBold));
                        items.Add(BulletListItem.Separator());
                    }

                    foreach (string line in Strings.ViewModels.HintPanel.CodeGenerator.HintText.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
                    {
                        items.Add(BulletListItem.Item(line));
                    }

                    break;
                }
                case AppHintType.ResourceParameters:
                {
                    items.Add(BulletListItem.Item(Strings.Dialogs.ResourceParameters.ParameterUsage));
                    break;
                }
                case AppHintType.ResourceValue:
                {
                    foreach (string line in Strings.ViewModels.ResourceModel.HintText.Split(new []{Environment.NewLine}, StringSplitOptions.None))
                    {
                        items.Add(BulletListItem.BulletItem(line));
                    }

                    break;
                }
                case AppHintType.NewVersion:
                {
                    var updateCheckInfo = AppContext.UpdateInfo;

                    if (updateCheckInfo?.HasNewerVersion == true)
                    {
                        var text = Strings.Services.Updates.NewVersionAvailableMessage(updateCheckInfo.NewerVersion.ToString());
                        items.Add(BulletListItem.Item(text));

                        if (updateCheckInfo.NewVersionDescriptions.Length > 0)
                        {
                            items.Add(BulletListItem.Hyperlink(Strings.Services.Updates.ReadWhatsNew, ReadUpdateChangedCommand));
                        }
                    }
                    else
                    {
                        Visible = false;
                    }

                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

            ItemsSource = items;
        }


        private void CloseCommandExecute(object obj)
        {
            Visible = false;
            AppContext.AppConfigFactory.Current.RemoveAppHint(_appHintType);
            AppContext.ApplicationService.SaveAppConfig();
        }

        private void ReadUpdateChangedExecute(object obj)
        {
            var updateCheckInfo = AppContext.UpdateInfo;
            if (updateCheckInfo?.HasNewerVersion == true)
            {
                string newVersionNumber = updateCheckInfo.NewerVersion.ToString();
                var helpItems = updateCheckInfo.NewVersionDescriptions
                    .Select(x => x.IsNullOrEmpty() ? BulletListItem.Separator() : BulletListItem.CheckItem(x)).ToList();
                using (var viewModel = new HelpItemsDialogViewModel(AppContext, helpItems,
                    Strings.Services.Updates.UpdateAvailable,
                    Strings.Services.Updates.WhatsNewInVersion(newVersionNumber)))
                {
                    DialogWindow.DialogShow<HelpItemsDialogViewModel, HelpItemsDialog>(viewModel);
                }
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            ShellViewModel.PropertyChanged -= ShellViewOnPropertyChanged;
            AppContext.OnAppEvent -= HandleAppEvent;
        }

        private void ShellViewOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ShellViewModel.CodeGeneratorItemTemplate))
            {
                DataBind();
            }
        }

        private void HandleAppEvent(object sender, ApplicationEventArgs e)
        {
            if (e is ApplicationEventOptionsChanged || e is ApplicationEventCheckForUpdateCompleted)
            {
                DataBind();
            }
        }
    }
}
