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

using LHQ.Utils.Extensions;

namespace LHQ.Data.ModelStorage
{
    /// <summary>
    ///     Model loading result.
    /// </summary>
    public sealed class ModelLoadResult
    {
        internal ModelLoadResult()
        { }

        /// <summary>
        ///     Status of model load process.
        /// </summary>
        public ModelLoadStatus Status { get; internal set; }


        public string RawJson { get; internal set; }

        /// <summary>
        ///     Version of loaded model.
        /// </summary>
        /// <remarks>
        ///     In case when <see cref="Status" /> equals to <see cref="ModelLoadStatus.ModelUpgraderRequired" /> this property
        ///     holds proper value of loaded model version which needs to be upgraded before properly loading.
        /// </remarks>
        public int ModelVersion { get; internal set; }

        public override string ToString()
        {
            return "Status: {0}, ModelVersion: {1}".FormatWith(Status, ModelVersion);
        }
    }
}
