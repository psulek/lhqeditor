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

using Newtonsoft.Json;

namespace LHQ.App.Model.VsGallery
{
    public class VsGalleryRequest
    {
        [JsonProperty("filters")]
        public Filter[] Filters { get; set; }

        [JsonProperty("assetTypes")]
        public string[] AssetTypes { get; set; }

        [JsonProperty("flags")]
        public long Flags { get; set; }
    }

    public class Filter
    {
        [JsonProperty("criteria")]
        public Criterion[] Criteria { get; set; }

        [JsonProperty("pageNumber")]
        public long PageNumber { get; set; }

        [JsonProperty("pageSize")]
        public long PageSize { get; set; }

        [JsonProperty("sortBy")]
        public long SortBy { get; set; }

        [JsonProperty("sortOrder")]
        public long SortOrder { get; set; }
    }

    public class Criterion
    {
        [JsonProperty("filterType")]
        public long FilterType { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

}
