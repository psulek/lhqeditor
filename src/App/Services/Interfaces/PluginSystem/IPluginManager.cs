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

using System.Collections.Generic;
using LHQ.App.Services.Implementation.PluginSystem;
using LHQ.Core.Interfaces.PluginSystem;

namespace LHQ.App.Services.Interfaces.PluginSystem
{
    public interface IPluginManager : IAppContextService
    {
        void Initialize();

        /// <summary>
        ///     Gets list of loaded plugin modules instances.
        /// </summary>
        List<PluginModuleRegistration> GetPluginModules();

        IPluginModule GetPluginModule(string pluginModuleKey);
        IPluginConfig GetPluginConfig(string pluginModuleKey);

        bool GetIsEnabledPluginModule(string pluginModuleKey);

        /// <summary>
        /// </summary>
        /// <param name="pluginsChanges">Tuple, item1 - plugin module key, item2 - enabled(bool), item3 - plugin config</param>
        /// <returns></returns>
        void UpdatePlugins(List<(string pluginKey, bool enabled, IPluginConfig config)> pluginsChanges);
    }
}
