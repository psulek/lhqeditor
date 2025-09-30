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
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LHQ.Utils.Utilities
{
	public static class JsonUtils
	{
		private static readonly DefaultJsonSerializerSettings _jsonSerializerSettings = new DefaultJsonSerializerSettings();

		public static T CloneObject<T>(T obj)
		{
			if (obj == null)
			{
				return default;
			}

			string json = JsonConvert.SerializeObject(obj, Formatting.None);
			return (T)JsonConvert.DeserializeObject(json, obj.GetType());
		}

		public static string ToJsonString<T>(T obj, Formatting formatting = Formatting.None, JsonSerializerSettings serializerSettings = null, 
			bool writeSingleQuotes = false)
		{
			if (writeSingleQuotes)
			{
				StringBuilder sb = new StringBuilder();
				using (StringWriter sw = new StringWriter(sb))
				{
					using (JsonTextWriter writer = new JsonTextWriter(sw))
					{
						writer.QuoteChar = '\'';
	
						JsonSerializer ser = new JsonSerializer();
						ser.Serialize(writer, obj);

						return sw.ToString();
					}
				}
			}
			
			return JsonConvert.SerializeObject(obj, formatting, serializerSettings ?? _jsonSerializerSettings);
		}

		public static T FromJsonString<T>(string value, JsonSerializerSettings serializerSettings = null)
		{
			return string.IsNullOrEmpty(value) ? default : JsonConvert.DeserializeObject<T>(value, serializerSettings ?? _jsonSerializerSettings);
		}

		public static string DateToJsonString(DateTime date)
		{
			return JsonConvert.ToString(date, DateFormatHandling.IsoDateFormat, DateTimeZoneHandling.Utc);
		}

		public static DateTime DateFromJsonString(string source, JsonSerializerSettings serializerSettings = null)
		{
			return JsonConvert.DeserializeObject<DateTime>(source, serializerSettings ?? _jsonSerializerSettings);
		}

		public static string TokenToString(JToken token, Formatting formatting, JsonSerializerSettings serializerSettings = null)
		{
			JsonConverter[] jsonConverters = (serializerSettings ?? _jsonSerializerSettings).Converters.ToArray();
			return token.ToString(formatting, jsonConverters);
		}
	}
}
