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
using System.Text.RegularExpressions;
using LHQ.Utils.Extensions;

namespace LHQ.Utils
{
    [Flags]
    public enum NameValidatorFlags
    {
        None = 0,
        AllowEmpty = 1
    }

    public enum NameValidatorResult
    {
        Valid = 0,
        NameIsEmpty = 1,
        NameCannotBeginWithNumber = 2,
        NameCanContainOnlyAlphaNumeric = 3
    }

    public static class NameValidator
    {
        private static readonly Regex _regexStartedWithNumbers = new Regex(@"^[0-9]+[a-zA-Z0-9]*$",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex _regexValidCharacters = new Regex(@"^[a-zA-Z]+[a-zA-Z0-9_]*$",
            RegexOptions.Singleline | RegexOptions.Compiled);

        public static NameValidatorResult Validate(string name, NameValidatorFlags flags = NameValidatorFlags.None)
        {
            var result = NameValidatorResult.Valid;

            if (name.IsNullOrEmpty())
            {
                result = flags.IsFlagSet(NameValidatorFlags.AllowEmpty) ? NameValidatorResult.Valid : NameValidatorResult.NameIsEmpty;
            }
            else
            {
                if (_regexStartedWithNumbers.IsMatch(name))
                {
                    result = NameValidatorResult.NameCannotBeginWithNumber;
                }
                else if (!_regexValidCharacters.IsMatch(name))
                {
                    result = NameValidatorResult.NameCanContainOnlyAlphaNumeric;
                }
                else if (name.Trim().IsNullOrEmpty())
                {
                    result = NameValidatorResult.NameIsEmpty;
                }
            }

            return result;
        }
    }
}
