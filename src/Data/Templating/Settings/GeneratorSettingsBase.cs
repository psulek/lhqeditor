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

using LHQ.Data.Support;
using LHQ.Utils.Extensions;
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedMemberHierarchy.Global

namespace LHQ.Data.Templating.Settings
{
    public class GeneratorSettingsBase: OutputSettings
    {
        public bool Enabled { get; set; }

        public GeneratorSettingsBase()
        {
            // Default value must be true
            Enabled = true;
        }

        public virtual void AssignFrom(GeneratorSettingsBase other)
        {
            OutputFolder = other.OutputFolder;
            OutputProjectName = other.OutputProjectName;
            Enabled = other.Enabled;
        }

        public override void Serialize(DataNode node)
        {
            base.Serialize(node);

            // write only if Enabled is false
            if (!Enabled)
            {
                node.AddAttribute(nameof(Enabled), DataNodeValueHelper.ToString(Enabled));
            }
        }

        public override bool Deserialize(DataNode node)
        {
            bool result = base.Deserialize(node);
            if (result)
            {
                // Enabled, if does not exist then default value is 'true'
                if (node.Attributes.Contains(nameof(Enabled)))
                {
                    var attrEnabled = node.Attributes[nameof(Enabled)];
                    Enabled = attrEnabled.Value.IsNullOrEmpty() || DataNodeValueHelper.FromString(attrEnabled.Value, false);
                }
                else
                {
                    Enabled = true;
                }
            }
            
            return result;
        }
    }
}
