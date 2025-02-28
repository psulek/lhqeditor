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
    public sealed class DialogShowInfo
    {
        public DialogShowInfo(string caption, string message, string detail = null)
        {
            Caption = caption;
            Message = message;
            Detail = detail;
        }

        public DialogShowInfo()
        { }

        public string Caption { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }
        
        //public TimeSpan? DelayTimeout { get; set; }
        
        public bool? CheckValue { get; set; }
        public string CheckHeader { get; set; }
        public string CheckHint { get; set; }
        
        public string CancelButtonHeader { get; set; }
        public string YesButtonHeader { get; set; }
        public string NoButtonHeader { get; set; }
        
        //public Action<bool> OnSubmit { get; set; }

        public string ExtraButtonHeader { get; set; }
        public Action ExtraButtonAction { get; set; }
    }
}