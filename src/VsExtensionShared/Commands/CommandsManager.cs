﻿#region License
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
using Microsoft.VisualStudio.Shell;

namespace LHQ.VsExtension.Commands
{
    public class CommandsManager
    {
        private static readonly Lazy<CommandsManager> _instance = new Lazy<CommandsManager>(() => new CommandsManager());

        public static CommandsManager Instance => _instance.Value;

        private readonly List<CommandBase> _commands;

        public CommandsManager()
        {
            _commands = new List<CommandBase>();
        }

        public void Initialize(VsPackage package, OleMenuCommandService commandService)
        {
            Register<OptionsCommand>();
            Register<InputBindingsCommand>();
            Register<ShowMainHelpCommand>();

            foreach (var command in _commands)
            {
                command.Initialize(package, commandService);
            }
        }

        private void Register<T>()
            where T : CommandBase, new()
        {
            _commands.Add(new T());
        }
    }
}