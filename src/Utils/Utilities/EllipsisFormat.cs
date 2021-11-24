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

namespace LHQ.Utils.Utilities
{
    [Flags]
    public enum EllipsisFormat
    {
        // Text is not modified.
        None = 0,

        // Text is trimmed at the end of the string. An ellipsis (...) 
        // is drawn in place of remaining text.
        End = 1,

        // Text is trimmed at the beginning of the string. 
        // An ellipsis (...) is drawn in place of remaining text. 
        Start = 2,

        // Text is trimmed in the middle of the string. 
        // An ellipsis (...) is drawn in place of remaining text.
        Middle = 3,

        // Preserve as much as possible of the drive and filename information. 
        // Must be combined with alignment information.
        Path = 4,

        // Text is trimmed at a word boundary. 
        // Must be combined with alignment information.
        Word = 8
    }
}
