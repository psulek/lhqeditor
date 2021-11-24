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
using System.IO;
using System.Threading.Tasks;
using LHQ.Utils.Utilities;
using NUnit.Framework;

namespace LHQ.UnitTests
{
    [TestFixture]
    public class SerializationTests : TestsBase
    {
        private const string ModelJson = @"{
  ""model"": {
    ""uid"": ""6ce4d54c5dbd415c93019d315e278638"",
    ""version"": 1,
    ""options"": {
      ""categories"": true,
      ""resources"": ""All""
    },
    ""name"": ""Localization"",
    ""primaryLanguage"": ""sk-SK""
  },
  ""languages"": [
    ""en-US"",
    ""sk-SK""
  ],
  ""categories"": {
    ""Category1"": {
      ""resources"": {
        ""res2"": {
          ""state"": ""New"",
          ""locked"": false,
          ""parameters"": {
            ""p1a"": {
              ""description"": ""p1adec"",
              ""order"": 1
            }
          }
        },
        ""res2"": {
          ""state"": ""Final"",
          ""locked"": false,
          ""parameters"": {
            ""p1b"": {
              ""description"": ""p1bdec"",
              ""order"": 1
            }
          }
        }
      }
    }
  },
  ""resources"": {
    ""RootResource1"": {
      ""state"": ""New"",
      ""locked"": false
    }
  }
}";

        [Test]
        public async Task JsonNetFromObjectBugTest()
        {
            try
            {
                JsonNetBug bug = new JsonNetBug();
                IBugData bugData = bug.CreateDefault();
                await bug.Save(bugData);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        [Test]
        public async Task SerializeDefaultModelAsJson()
        {
/*
            string testDirectory = TestContext.CurrentContext.TestDirectory;
            string fileName = Path.Combine(testDirectory, "Strings.lhq");
            var modelContext = CreateEmptyModel("Localization", ModelContextOptions.Default);
            modelContext.AddResource("RootResource1", null);
            CategoryElement category = modelContext.AddCategory("Category1");
            ResourceElement resource = modelContext.AddResource("res2", category);
            var resourceParameter = modelContext.AddResourceParameter("p1", resource);
            resourceParameter.Description = "p1dec";
            resourceParameter.Order = 1;

            modelContext.SaveToFile(fileName, ModelSaveOptions.Indent);
            Console.WriteLine("Default model was saved to: " + fileName);
            Console.WriteLine();
            string jsonFile = await FileUtils.ReadAllTextAsync(fileName);
            Console.WriteLine(jsonFile);
*/
        }

        [Test]
        public void GenerateModelWithDuplicity()
        {
            string testDirectory = TestContext.CurrentContext.TestDirectory;
            string fileName = Path.Combine(testDirectory, "Strings-Duplicity.lhq");
            //File.WriteAllText(fileName, ModelJson);
            FileUtils.WriteAllText(fileName, ModelJson);
            Console.WriteLine("Model was saved to: " + fileName);
        }

        [Test]
        public void DeserializeModelFromJson()
        {
/*
            string testDirectory = TestContext.CurrentContext.TestDirectory;
            string fileName = Path.Combine(testDirectory, "Localization.lhq");
            var modelContext = CreateEmptyModel("", ModelContextOptions.Default, true);
            string json = File.ReadAllText(fileName);
            modelContext.Load(json);

            VisualizeModel(modelContext, new ModelVisualizerOptions(true, true));
*/
        }

        [Test]
        public void DeserializeFromFile()
        {
/*
            string fileName = GetSampleModelFileName("Strings.lhq");
            var modelContext = CreateEmptyModel("", ModelContextOptions.Default, true);
            string json = File.ReadAllText(fileName);
            ModelLoadResult modelLoadResult = modelContext.Load(json);
            WriteLine("Load result: " + modelLoadResult);
            if (modelLoadResult.Status == ModelLoadStatus.Success)
            {
                VisualizeModel(modelContext, new ModelVisualizerOptions(true, true));
            }
*/
        }

        //        [Test]
//        public void SerializeDefaultModelAsXml()
//        {
//            var modelContext = CreateEmptyModel("Localization", ModelContextOptions.Default, true);
//            string file = @"Localization_xml.lhq";
//            //string xml = modelContext.SaveAsXml(ModelSaveOptions.Indent);
//            //File.WriteAllText(file, xml);
//            Console.WriteLine("Default model was saved to: " + Path.GetFullPath(file));
//        }

        [Test]
        public void SerializeToJsonTest()
        {
/*
            var modelContext = CreateEmptyModel("Sample1", ModelContextOptions.Default);
            modelContext.Builder
                .AddCategory("Cars", cars =>
                {
                    cars.AddResource("Audi", audi => audi.AddValue("sk-SK", "Slovak Audi"));
                    cars.AddResource("BMW", bmw => bmw.AddValue("sk-SK", "Slovak BMW"));
                });

            var serializer = new ModelSerializer_v1();
            var modelSaveOptions = ModelSaveOptions.Default;

            string json = serializer.Serialize(modelContext, modelSaveOptions);
            WriteLine("Serialize 1 test json: ");
            WriteLine(json);

            var modelLoadResult = serializer.Deserialize(json);
            if (modelLoadResult.Status == ModelLoadStatus.Success)
            {
                string json2 = serializer.Serialize(modelSaveOptions);

                bool equal = json == json2;

                WriteLine("");
                WriteLine("Serialize/Deserialize test completed: " + (equal ? "successfully" :"failed"));
                if (!equal)
                {
                    WriteLine("Serialized model after deserialize: ");
                    WriteLine(json2);
                }
            }
*/
        }
    }
}