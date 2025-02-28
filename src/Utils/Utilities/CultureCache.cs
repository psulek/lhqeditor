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
using System.Globalization;
using System.Linq;
// ReSharper disable UnusedMember.Global

namespace LHQ.Utils.Utilities
{
    public class CultureCache : IDisposable
    {
        private static CultureCache _instance;

        public static readonly CultureInfo English = new CultureInfo("en");
        public static readonly CultureInfo EnglishUS = new CultureInfo("en-us");
        private readonly Dictionary<string, CultureInfo> _cache;
        private readonly Dictionary<string, string> _cacheDisplayNames;
        private bool _disposed;

        private CultureCache()
        {
            _cache = new Dictionary<string, CultureInfo>();
            _cacheDisplayNames = new Dictionary<string, string>();

            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                string cultureName = culture.Name.ToLowerInvariant();
                _cache.Add(cultureName, culture);
                _cacheDisplayNames.Add(cultureName, culture.EnglishName);
            }
        }

        public static CultureCache Instance => _instance ?? (_instance = new CultureCache());

        public IEnumerable<CultureInfo> GetAll()
        {
            return _cache.Values.AsEnumerable();
        }

        public string GetCultureDisplayName(string name)
        {
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(name, "name");
            name = name.ToLowerInvariant();
            _cacheDisplayNames.TryGetValue(name, out string result);
            return result;
        }

        public bool ExistCulture(string name)
        {
            return GetCulture(name) != null;
        }

        public CultureInfo GetCulture(string name)
        {
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(name, "name");
            name = name.ToLowerInvariant();
            _cache.TryGetValue(name, out CultureInfo result);
            return result;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _cache.Clear();
                _cacheDisplayNames.Clear();
                _disposed = true;
            }
        }
    }
}
