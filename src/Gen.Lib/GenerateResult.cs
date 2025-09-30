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

namespace LHQ.Gen.Lib
{
    /// <summary>
    /// Represents result of code generator process of <see cref="Generator.Generate"/>. 
    /// </summary>
    public sealed class GenerateResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateResult"/> class.
        /// </summary>
        /// <param name="generatedFiles">List of <see cref="GeneratedFile"/> files.</param>
        /// <param name="modelGroupSettings">List of settings used in code generator process.</param>
        //public GenerateResult(List<GeneratedFile> generatedFiles, List<LhqModelGroupSettings> modelGroupSettings)
        public GenerateResult(List<GeneratedFile> generatedFiles)
        {
            GeneratedFiles = generatedFiles;
            //ModelGroupSettings = modelGroupSettings;
            // ModelGroupSettings = new List<LhqModelGroupSettings>();
        }

        /// <summary>
        /// Gets list of <see cref="GeneratedFile"/> files.
        /// </summary>
        public IReadOnlyList<GeneratedFile> GeneratedFiles { get; }
        
        /// <summary>
        /// Gets list of settings used in code generator process.
        /// </summary>
        // public IReadOnlyList<LhqModelGroupSettings> ModelGroupSettings { get; }
    }
}
