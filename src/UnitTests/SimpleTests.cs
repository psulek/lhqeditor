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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Force.Crc32;
using LHQ.Utils.Extensions;
using LHQ.Data;
using LHQ.Data.Comparers;
using LHQ.Data.ModelStorage;
using LHQ.Data.Templating.Templates.WinForms;
using LHQ.Utils.Utilities;
using Newtonsoft.Json;
using NUnit.Framework;

namespace LHQ.UnitTests
{
    [TestFixture]
    public class SimpleTests: TestsBase
    {
        [Test]
        public void T4Template_Test()
        {
            var template = new WinFormsResxCsharp01Template();
            //template.CSharp.ParamsMethodsSuffix = "ab\"cd";
            var serializerSettings = new DefaultJsonSerializerSettings(StringEscapeHandling.EscapeHtml);
            string jsonString = JsonUtils.ToJsonString(template, Formatting.None, serializerSettings, true);
            Console.WriteLine(jsonString);

        }

        [Test]
        public void Crc_Test()
        {
            string token1 =
                "eyJhbGciOiJkaXIiLCJlbmMiOiJBMTI4Q0JDLUhTMjU2IiwidHlwIjoiSldUIn0..QM1sXof2fKbnPKxICc5cLg.pouZ7KXLqiToaR7u1xxVy8dclQZ1JfUhDTVV6AWGlx0q6hDqfIX6bqZPRVsYdZu84qCpStSdZVzzVNhMxjEKiHI3HMMlnov6kONb6QihSCB5zWTd-CdW6KHvjYsliXNBLD_9uAzkq4q1Eri-ppFOENCqn2bpFpglq8HLLWU-xDHmLtQu8FT2HZvacWrXwgtoOBvjUDYi2tlYvziuhsRDMQRAyuA45F_GT_60HSEPYitwf4o3KJ9UHlMV3GLcGrhaWgb2yHeta_W1xYNoYI5hYUDWtiMwl8cOL5afr9yxGOFoWUoX3AID7o8Ewfq7Lkn86Sw7Lwozff3N6fP7QQm4-jBMeuk_HXzbM5xBtG-Xl0wp0fSycw0f7aO6dWdR2uC5TVo9Z_YB9sYTG880Zk6PWJ9zTdtNPUMhz2rTDFKBgVrwSmD2r4zI7lKn_2-TSeiLsj_YfqbgFLB_D7Dlcm_jrKRfBWYm-7VeTFiYuIaF_ikgkZTc4-Zx56qAuVmf2bIj7H7Y7eVD0vv4NVp0TZsEr13rdfh8rJ30tS10zYouWzzIw5GfpJNCfZHauC4X-xRgdMKUzvS2a_o6r8iJ4AY4R81QoQBwceiaqWGFDhvXFAK8K4Yi2mLIem46nZICoNHYE8AGEQS7ivViFdLxtS7rs61FFbVPtVX1zd6ea5FVY9eVqx0BOXGxlwisFzQirCVE9sulLvQyDPr2Uv7b390C0M5rL4f2bqtRH99kKtCAOcmxyxFaPTop8CDuReMQNCuz4kKXe0FDKdvCBTteGC8AKKDUzrHFRKC5G73On281cFdj28g5UDPVvDKPIawq6bj8dBJ88xXPe5AA5H5t8lO5Htykq3Ri-4V8pytH5KcNQ2oReDHDU8uhe08NI8hodisbXyLnHtk3grIcbmT_jmkGU2MgadvVFNxg3w6eH1E70dyQehxgXczYEjo74RTYn8Gf81v2Xxk-xY_qcjGgZYLaHq-9VPHcCzLRq4LNvcFjlgWC7U5UCDIR0Q2wtqmq9vQpSfMZMdjVpt5kF-Ow0Rjg1l7_3JHj9cYQEP_uLYGaEqnvZxpkT8Si3e6m5OEn-J9SadduF_LcsORcc3T_H2q-hn5GPwHwJTvofFgbG0D5fodqKof9ch002F2fcSl7nuXRcDiLwbS9JWXM3UHzHgM6Quft8S6jUGmOrvw1PV8FgT_uid0eKRbPMX65c5UVTMBH8SkHvGyxNh7wwJw39nBtG7V7taiqRjbYgo0V5xQY030v7wqF3XHWgZTza7eMGTaKmgcI7wOS7Z8oMYRtfIQac17HEZi1vBXhEOQ8y7BTVjMeWixBmYP9W5eJyGWAtlKC.5Q_xVrQodXrqjvIa2ITT4g";

            string token2 =
                "eyJhbGciOiJkaXIiLCJlbmMiOiJBMTI4Q0JDLUhTMjU2IiwidHlwIjoiSldUIn0..fUCjQXHbWN-BEui9_Syz2Q.WsxtEQgNrXP7e8VSL7Vx8Vsc6IouDe3GkH5XPRd0tuudy3Tuc3hzM7TRuoUf_Tn_9FE3cT9O29CaIyXJ9ia6BmFXH-BnQGa3sbU-60fqY1-3z5N8uwUMqwOspEgRa7t_OSL8Cl0Tod_K2K6tTqh6MLvDjrqmzy5FFpfL98SQXnWuQkJTcUMvu698ParBx6_xgUe2g9_--_EI2rDNo-teh-Cyv98T8uUIaWDFmxcS6ncHn6fgTjSmZPgy0al_eD_kqPFKd0OPy2IDJ-1GRJwv5XlxoxyZ6UfMGWre4zbhWkd4QwFxA23Sjv5fRQ3mmIBL6EnEiCbfLolrt3cFR-4nVOTHNv57CNeA1t9eqqPfglhzuBcJMWswBmOca8UYrnT3uu2Svf1PjAQRHbB5s1tQsQNK9UApwQzxx851c-AlYdpawEEcX88azWgJP-klDoIZvh3Yg6Ij88wuBts6GPAujPQArpUo_Y0NAn6giMWKCFLGRSjA86Euq4hPNZvoTeBdrj2VqFsdwlG5FurwFBwWozcB8La2e5zFMzmYDKkTRxnl5Ko07iqTEePCyP5T62B2U0qbEPtbQ_uG6Gn4IwAj9_FDHNYA5Ne9tVKbnF3-P3KYq1PwJaG8PHpl7Gij99jJM9SryZqk7eOjj0zBMhgEGV9INu69qmQnbeOPGhJJQpUCx1SvUraTCzjjmt_-TIe6jkFPLbCn2Ln7pEaIJB14fjx0sW1hD4_mFDpjkDVpi2Cpbhmvw7urgitUznkT2kdygNrWy0O0JzJaTDO1C2zUiz5Xq_pCLGfzevKoz9e7I_vwrlIWLO12IHki4QuYZGJLoRI9R_UpYkSzL0DGVaLxCN15dQrw3dB1ODGG8P2RYLmt_5xV1PQ4Cer7aLvhgHTYlVJp-2ymXj3oABiR5NpB0jQwkAV5ao2NmZXzJtm-mQsIh3lzGOenNbZsgItiMci-7QHtuujAZKOD-vMkmy5cOJXysHAfG5M7fBMQHBLrywd43ZkRjW7N6zGVeSeXCjv_O228bciQL3e-Y6j7DeSltn1G0vVYdF7wNn9Wl4Cm-utoFidsRlo8iwVUdxl40z0-sOsjtSpAF8Lams78sghe5QSv5zEAgqfUilfIOy-f6oVU26RDMCJ6oHzWEXYCCjO2pkF78XfA9HXO71BKByBHBpZVQz-q2EnuYm3Fh8XNNU1B0M_hHbuS7CM-RF9WgXVxhW12Acg9pXwa5ngvVHkLzWEJFzd4hBIm8AleSg5G85uptowrJGHSiQrumolNZI2ag9dmTBG1w9Y2EwX5TxPjnyl4F-kPVhxyl4Cp7rVmNfNFrdxHSGqbpLAvWajUO18U.cBWWrGP02QfJDnZYAgR4vg";

            string uid = $"{token1}_{token2}";

            byte[] buffer = Encoding.ASCII.GetBytes(uid);
            uint compute = Crc32Algorithm.Compute(buffer);
        }

        [Test]
        public void OrderingTest()
        {
            LanguagePrimaryComparer comparer = new LanguagePrimaryComparer();
            var languages = new List<LanguageElement>
            {
                new LanguageElement
                {
                    Name = "XN",
                    IsPrimary = false
                },
                new LanguageElement
                {
                    Name = "EN",
                    IsPrimary = false
                },
                new LanguageElement
                {
                    Name = "SK",
                    IsPrimary = true
                }
            };

            WriteLine(languages.Select(x => x.Name).ToArray().ToDelimitedString());

            languages.Sort(comparer);

            WriteLine("After sort by primary:");
            WriteLine(languages.Select(x=>x.Name).ToArray().ToDelimitedString());
        }

        [Test]
        public void Test_PartitionTexts()
        {
            var texts = new List<string>
            {
                "1234",
                "5678",
                "9",
                "abcd",
                "efg"
            };

            int pid = -1;
            foreach (var partition in PartitionTexts(texts, 8))
            {
                pid++;
                Console.WriteLine($"Partition {pid} values:");
                foreach (string item in partition)
                {
                    Console.WriteLine("\t {0}", item);
                }
            }
        }

        private IEnumerable<IEnumerable<string>> PartitionTexts(IList<string> texts, int maxCharsInTexts)
        {
            bool hasData = texts.Count > 0;
            int charCount = 0;
            bool allowMoveNext = true;

            var enumerator = texts.GetEnumerator();

            IEnumerable<string> GetBatch()
            {
                do
                {
                    hasData = !allowMoveNext || enumerator.MoveNext();
                    if (hasData)
                    {
                        string current = enumerator.Current;
                        int newCharCount = charCount + (current ?? string.Empty).Length;

                        allowMoveNext = true;

                        if (newCharCount <= maxCharsInTexts)
                        {
                            charCount = newCharCount;
                            yield return current;
                        }
                        else
                        {
                            allowMoveNext = false;
                            charCount = 0;
                            break;
                        }
                    }
                } while (hasData && charCount <= maxCharsInTexts);
            }

            do
            {
                yield return GetBatch();
            } while (hasData);
        }

        [Test]
        public async Task SerializeSampleModelToFile()
        {
            var modelContext = CreateSampleModel(DefaultModelName, ModelContextOptions.Default);

            var category = modelContext.Model.Categories.FindByName("Cars", true, CultureInfo.InvariantCulture);
            var resource = category.Resources.First();
            modelContext.AddResourceParameter("param1", resource);
            modelContext.AddResourceParameter("param2", resource);

            modelContext.AddResource("TestResource1", null);

            string serializedModel = SerializeModel(modelContext, new ModelSaveOptions(true));
            WriteLine("Serialized model:");
            WriteLine(serializedModel);
            
            var currentDirectory = Environment.CurrentDirectory;
            string fileName = Path.Combine(currentDirectory, "model1.lhq");
            await FileUtils.WriteAllTextAsync(fileName, serializedModel);

            WriteLine("Serialized model was saved to: " + fileName);
        }

        [Test]
        public void DeserializeSampleModelFromXml()
        {
            var modelContext = CreateSampleModel(DefaultModelName, ModelContextOptions.Default);
            string modelXml = SerializeModel(modelContext);
            ModelLoadResult loadResult = DeserializeModel(modelXml, ModelContextOptions.Default, out modelContext);
            loadResult.Status.Should().Be(ModelLoadStatus.Success);
            WriteLine("Deserialization from xml test passed!");
        }

        [Test]
        public void VisualizeSampleModel()
        {
            var modelContext = CreateSampleModel(DefaultModelName, ModelContextOptions.Default);

            string json = @"{
	debug: {
		displaysMock: {
			enabled: false,
			fileName: ''
		},
		windowsMock: {
			enabled: false,
			fileName: ''
		}
	},
	profiling: {
		memUsage: {
			enabled: false,
			snapshotInterval: 5000
		}
	}
}";

            object obj = JsonConvert.DeserializeObject(json);

            VisualizeModel(modelContext);
        }
    }
}