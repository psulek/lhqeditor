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

using System.Collections.Generic;
using LHQ.App.Code;
using Newtonsoft.Json;

namespace LHQ.App.Model
{
    public sealed class AppStateStorage: IStorage
    {
        public AppStateStorage()
        {
            AppStartedFirstTimeTicks = DateTimeHelper.UtcNow.Ticks;
            AppStartedFirstTime = true;
            TreeLegendExpanded = true;
            StateFinalLocksResourceNotified = false;
            ParametersUsageInfoVisible = true;
            LastImportedResourcesPath = string.Empty;
            LastExportedResourcesPath = string.Empty;
            LastSelectedImporterKey = string.Empty;
            CustomData = new Dictionary<string, string>();
        }

        [JsonProperty]
        public long AppStartedFirstTimeTicks { get; internal set; }

        [JsonProperty]
        public bool AppStartedFirstTime { get; internal set; }

        [JsonProperty]
        public bool StateFinalLocksResourceNotified { get; internal set; }

        [JsonProperty]
        public bool ParametersUsageInfoVisible { get; internal set; }

        [JsonProperty]
        public bool TreeLegendExpanded { get; internal set; }

        [JsonProperty]
        public string LastImportedResourcesPath { get; internal set; }

        [JsonProperty]
        public string LastSelectedImporterKey { get; internal set; }

        [JsonProperty]
        public string LastExportedResourcesPath { get; internal set; }

        [JsonProperty]
        public string LastSelectedResourceImportMergeMode { get; internal set; }

        [JsonProperty]
        public Dictionary<string, string> CustomData { get; }

        [JsonProperty]
        public bool? DoNotShowLicenseDowngradeWarning { get; set; }

        [JsonProperty]
        public uint LicenseDowngradeLastNotifyUID { get; set; }
    }
}
