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
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Input;
using LHQ.App.ViewModels;
using Microsoft.VisualStudio.Shell;

namespace LHQ.VsExtension.Commands
{
    public class InputBindingsCommand: CommandBase
    {
        private const string PropertyKeyGesture = "KeyGesture";

        private readonly List<CommandID> _commandIds = new List<CommandID>();

        public override void Initialize(Package package, OleMenuCommandService commandService)
        {
            base.Initialize(package, commandService);

            AddCommand(PackageIds.DuplicateElementCommand, "Ctrl+D");
            AddCommand(PackageIds.CopyElementAsSpecial1Command, "Ctrl+Shift+C");
            AddCommand(PackageIds.CopyElementAsSpecial2Command, "Ctrl+Alt+C");
            AddCommand(PackageIds.FindElementCommand, "Ctrl+F");
            AddCommand(PackageIds.NewCategoryCommand, "Ctrl+Ins");
            AddCommand(PackageIds.NewResourceCommand, "Insert");
        }

        private void AddCommand(int commandId, string keyGesture)
        {
            var command = new CommandID(PackageGuids.guidPackageCmdSet, commandId);
            _commandIds.Add(command);

            _menuCommand = new MenuCommand(ExecuteCommand, command);
            _menuCommand.Properties.Add(PropertyKeyGesture, keyGesture);
            _commandService.AddCommand(_menuCommand);
        }

        private void ExecuteCommand(object sender, EventArgs e)
        {
            try
            {
                if (sender is MenuCommand menuCommand && menuCommand.Properties.Contains(PropertyKeyGesture))
                {
                    string menuKeyGesture = menuCommand.Properties[PropertyKeyGesture] as string;

                    ShellViewModel shellViewModel = VsPackageService.GetActiveShellView();

                    if (shellViewModel != null)
                    {
                        ICommand shellViewCommand = null;
                        foreach (var bindingItem in shellViewModel.InputBindings.Where(x=>!x.applyToView))
                        {
                            InputBinding binding = bindingItem.binding;
                            var bindingGesture = binding.Gesture as KeyGesture;
                            if (bindingGesture?.DisplayString == menuKeyGesture && binding.Command != null)
                            {
                                shellViewCommand = binding.Command;
                                break;
                            }
                        }

                        if (shellViewCommand != null && shellViewCommand.CanExecute(null))
                        {
                            try
                            {
                                shellViewCommand.Execute(null);
                            }
                            catch (Exception exc)
                            {
                                shellViewModel.AppContext.Logger.Error($"Execute menu command '{menuKeyGesture}' failed.", exc);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
