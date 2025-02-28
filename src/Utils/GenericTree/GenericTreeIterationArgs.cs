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

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace LHQ.Utils.GenericTree
{
    public class GenericTreeIterationArgs<TItem>
    {
        public GenericTreeIterationArgs(int level, TItem parent, TItem current)
        {
            Level = level;
            Parent = parent;
            Current = current;
            CurrentStage = GenericTreeIterationStage.Enter;
        }

        public int Level { get; }

        public TItem Parent { get; }

        public TItem Current { get; }

        public bool Cancel { get; set; }

        public bool CancelBreaksChildLoop { get; set; }

        public GenericTreeIterationStage CurrentStage { get; internal set; }
    }

    public class GenericTreeIterationArgs<TItem, TData> : GenericTreeIterationArgs<TItem>
    {
        public GenericTreeIterationArgs(int level, TItem parent, TItem current) : base(level, parent, current)
        { }

        public TData Data { get; set; }
    }
}
