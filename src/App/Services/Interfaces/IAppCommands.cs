#region License
// Copyright (c) 2025 Peter Šulek / ScaleHQ Solutions s.r.o.
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

using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Model;
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace LHQ.App.Services.Interfaces
{
    public interface IAppCommands
    {
        MenuDelegateCommand FocusTreePanelCommand { get; }

        MenuDelegateCommand SaveProjectCommand { get; }

        ICommand SaveProjectAsCommand { get; }

        MenuDelegateCommand NewCategoryCommand { get; }

        MenuDelegateCommand NewResourceCommand { get; }

        MenuDelegateCommand RenameCommand { get; }

        MenuDelegateCommand DeleteElementCommand { get; }

        MenuDelegateCommand CutElementCommand { get; }

        MenuDelegateCommand CopyElementCommand { get; }

        MenuDelegateCommand PasteElementCommand { get; }

        ICommand LanguageSettingsCommand { get; }

        MenuDelegateCommand ExportCommand { get; }

        MenuDelegateCommand ImportCommand { get; }

        ICommand ProjectSettingsCommand { get; }

        MenuDelegateCommand ExpandAllElementCommand { get; }

        ICommand ExpandElementCommand { get; }

        MenuDelegateCommand CollapseAllElementCommand { get; }

        ICommand CollapseElementCommand { get; }

        MenuDelegateCommand UndoCommand { get; }

        MenuDelegateCommand RedoCommand { get; }

        ICommand NewProjectCommand { get; }

        ICommand OpenProjectCommand { get; }

        ICommand CloseProjectCommand { get; }

        ICommand ExitApplication { get; }

        DelegateCommand<RecentFileInfo> RecentProjectCommand { get; }

        ICommand PreferencesCommand { get; }

        MenuDelegateCommand FindElementCommand { get; }

        ICommand AboutCommand { get; }
    }
}