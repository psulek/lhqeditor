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
using System.Collections.Generic;

namespace LHQ.App.Components.VirtualTreeListViewControl
{
    public class NodePath
    {
        private string _textPath;

        /// <summary>
        ///     Text path
        /// </summary>
        public string TextPath
        {
            get => _textPath;
            set => SetTextPath(value);
        }

        /// <summary>
        ///     Get the integer path
        /// </summary>
        public int[] IntPath { get; private set; }

        private void SetTextPath(string value)
        {
            if (_textPath == value)
            {
                return;
            }

            _textPath = null;

            if (value == null)
            {
                IntPath = null;
                return;
            }

            var intPath = new List<int>();
            string[] parts = value.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string part in parts)
            {
                if (!int.TryParse(part, out int index))
                {
                    throw new Exception("Invalid node path '" + _textPath + "'");
                }

                intPath.Add(index);
            }

            IntPath = intPath.ToArray();
            _textPath = string.Join(".", IntPath);
        }

        /// <summary>
        ///     Get the string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _textPath ?? "<empty>";
        }
    }
}
