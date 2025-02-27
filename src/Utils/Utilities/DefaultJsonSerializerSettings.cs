﻿#region License
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

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace LHQ.Utils.Utilities
{
    internal class DefaultJsonSerializerSettings : JsonSerializerSettings
    {
        public DefaultJsonSerializerSettings(StringEscapeHandling stringEscapeHandling = StringEscapeHandling.Default)
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat;
            DateParseHandling = DateParseHandling.DateTime;
            DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
            DefaultValueHandling = DefaultValueHandling.Include;
            FloatFormatHandling = FloatFormatHandling.String;
            FloatParseHandling = FloatParseHandling.Double;
            Formatting = Formatting.None;
            NullValueHandling = NullValueHandling.Ignore;
            ObjectCreationHandling = ObjectCreationHandling.Auto;
            PreserveReferencesHandling = PreserveReferencesHandling.None;
            ReferenceLoopHandling = ReferenceLoopHandling.Error;
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            TypeNameHandling = TypeNameHandling.None;
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                new IsoDateTimeConverter()
            };
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    ProcessDictionaryKeys = true,
                    OverrideSpecifiedNames = true
                }
            };
            StringEscapeHandling = stringEscapeHandling;
        }
    }
}
