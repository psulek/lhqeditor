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

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace LHQ.Data.Templating.Settings
{
    public sealed class SettingsValidationError
    {
        public bool IsWarning { get; set; }

        public string Message { get; set; }

        public string Detail { get; set; }

        public bool IsConfirmation { get; set; }

        private SettingsValidationError(string message, string detail, bool isWarning, bool isConfirmation = false)
        {
            Message = message;
            Detail = detail;
            IsWarning = isWarning;
            IsConfirmation = isConfirmation;
        }

        public static SettingsValidationError Error(string message, string detail = null) => new SettingsValidationError(message, detail, false);

        public static SettingsValidationError Warning(string message, string detail = null, bool isConfirmation = false) =>
            new SettingsValidationError(message, detail, true, isConfirmation);
    }
}
