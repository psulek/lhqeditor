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
using Newtonsoft.Json;

namespace LHQ.App.Model.VsGallery
{
    public class VsGalleryResponse
    {
        [JsonProperty("results")]
        public Result[] Results { get; set; }
    }

    public class Result
    {
        [JsonProperty("extensions")]
        public Extension[] Extensions { get; set; }

        [JsonProperty("pagingToken")]
        public object PagingToken { get; set; }

        [JsonProperty("resultMetadata")]
        public ResultMetadatum[] ResultMetadata { get; set; }
    }

    public class Extension
    {
        [JsonProperty("publisher")]
        public Publisher Publisher { get; set; }

        [JsonProperty("extensionId")]
        public Guid ExtensionId { get; set; }

        [JsonProperty("extensionName")]
        public string ExtensionName { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("flags")]
        public string Flags { get; set; }

        [JsonProperty("lastUpdated")]
        public DateTimeOffset LastUpdated { get; set; }

        [JsonProperty("publishedDate")]
        public DateTimeOffset PublishedDate { get; set; }

        [JsonProperty("releaseDate")]
        public DateTimeOffset ReleaseDate { get; set; }

        [JsonProperty("shortDescription")]
        public string ShortDescription { get; set; }

        [JsonProperty("versions")]
        public Version[] Versions { get; set; }

        [JsonProperty("deploymentType")]
        public long DeploymentType { get; set; }
    }

    public class Publisher
    {
        [JsonProperty("publisherId")]
        public Guid PublisherId { get; set; }

        [JsonProperty("publisherName")]
        public string PublisherName { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("flags")]
        public string Flags { get; set; }

        [JsonProperty("domain")]
        public object Domain { get; set; }

        [JsonProperty("isDomainVerified")]
        public bool IsDomainVerified { get; set; }
    }

    public class Version
    {
        [JsonProperty("version")]
        public string VersionVersion { get; set; }

        [JsonProperty("flags")]
        public string Flags { get; set; }

        [JsonProperty("lastUpdated")]
        public DateTimeOffset LastUpdated { get; set; }
    }

    public class ResultMetadatum
    {
        [JsonProperty("metadataType")]
        public string MetadataType { get; set; }

        [JsonProperty("metadataItems")]
        public MetadataItem[] MetadataItems { get; set; }
    }

    public class MetadataItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }
    }
}
