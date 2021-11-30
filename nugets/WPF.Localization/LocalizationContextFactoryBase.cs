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

namespace ScaleHQ.WPF.LHQ
{
    /// <summary>
    /// Factory which provide LHQ strings context as singleton.
    /// </summary>
    public abstract class LocalizationContextFactoryBase
    {
        /// <summary>
        /// Gets LHQ strings context singleton.
        /// </summary>
        /// <returns></returns>
        public abstract IFormattable GetSingleton();
    }

    /// <summary>
    /// Default implementation of localization context factory which late-bound return of strings context singleton.
    /// </summary>
    public class LocalizationContextFactoryDefault : LocalizationContextFactoryBase
    {
        private readonly Func<IFormattable> _stringsContextFactory;

        /// <summary>
        /// Creates new instance of <see cref="LocalizationContextFactoryDefault"/> class.
        /// </summary>
        /// <param name="stringsContextFactory">Delegate which returns LHQ strings context singleton when required.</param>
        public LocalizationContextFactoryDefault(Func<IFormattable> stringsContextFactory)
        {
            _stringsContextFactory = stringsContextFactory ?? throw new ArgumentNullException(nameof(stringsContextFactory));
        }

        /// <summary>
        /// Gets LHQ strings context singleton.
        /// </summary>
        /// <returns></returns>
        public override IFormattable GetSingleton()
        {
            return _stringsContextFactory();
        }
    }
}