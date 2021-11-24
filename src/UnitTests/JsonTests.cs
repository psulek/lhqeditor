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
using System.Collections.Generic;
using System.IO;
using LHQ.Utils.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace LHQ.UnitTests
{
    [TestFixture]
    public class JsonTests
    {
        public class LocData
        {
            public string Text { get; set; }
        }

        [Test]
        public void ManualDeserializeJSON_NET()
        {
            var list = new List<LocData>();
            for (int i = 0; i < 100; i++)
            {
                list.Add(new LocData
                {
                    Text = "text " + i
                });
            }

            string jsonData = JsonConvert.SerializeObject(list, Formatting.None);

            #region json

            string json = @"{
	'model' : {
		'uid' : '6ce4d54c-5dbd-415c-9301-9d315e278638',
		'modelVersion' : '1.0.0.0',
		'version' : '2',
		'key' : 'd78d22e817ec4801929ba26a46eb1c95',
		'name' : 'Default'
	},
	'languages' : {
		'en-US' : {
			'key' : '12f6bf8a1e8a4032addd6a622691f1ab',
			'name' : 'en-US',
			'order' : 1,
			'isPrimary' : true
		}
	},
	'categories' : {
		'category' : {
			'key' : 'c86691e1ed0a435ba5595c8e74da9dec',
			'name' : 'category',
			'order' : 1,
			'resources' : {
				'resource' : {
					'key' : '4bced078a0a4401b9b18103dbb660e50',
					'name' : 'resource',
					'order' : 1,
					'parameters' : {
						'p1' : {
							'key' : '87c9a8a75e794769b7c4673ea0bfc42c',
							'name' : 'p1',
							'order' : 1
						}
					},
					'values' : {
						'en-US' : {
							'key' : 'cc973f31786a46c29d45dbbe85265e70',
							'order' : 0,
							'lang' : 'en-US',
							'value' : 'sadsad'
						}
					}
				}
			}
		}
	}
}
";

            #endregion

            //var modelContext = new ModelContext(ModelContextOptions.Default);
            //var parser = new JsonParserModel(modelContext);
            //bool parsed = parser.Parse(new StringReader(json));

            Console.WriteLine("JSON:");
            Console.WriteLine(json);
            Console.WriteLine(new string('=', 100));
            
            JsonTextReader reader = new JsonTextReader(new StringReader(json));
            while (reader.Read())
            {
                var tokenType = reader.TokenType;
                if (reader.Value != null)
                {
                    Console.WriteLine("Token: {0} ; Value: {1}     Depth: {2}", tokenType, reader.Value, reader.Depth);
                }
                else
                {
                    Console.WriteLine("Token: {0} | Depth: {1}", tokenType, reader.Depth);

                    if (tokenType.In(JsonToken.EndObject, JsonToken.EndArray, JsonToken.EndConstructor))
                    {
                        Console.WriteLine();
                    }

                }
            }

            Console.ReadLine();
        }
    }
}