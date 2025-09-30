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

using System;

namespace LHQ.App.Model
{
    [Flags]
    public enum SaveProjectFlags
    {
        None = 0,
        
        /// <summary>
        /// Flag enforce save via host environment (VS) and not directly to file.
        /// When this flag is not set, project is saved directly to file.
        /// But in some situations when this flag is set, eg. in VS IDE to trigger save on envdte document object
        /// which will in turn call associated Source Control (if any) to checkout file if is readonly (TFS server for example, not GIT).
        /// </summary>
        HostEnvironmentSave = 1,
        
        /// <summary>
        /// When this flag is set, code generation is skipped after save (even if 'run code generator after save' is checked). 
        /// </summary>
        SkipCodeGeneration = 2,
        
        All = HostEnvironmentSave | SkipCodeGeneration
    }
}
