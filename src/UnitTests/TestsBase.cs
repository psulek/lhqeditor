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
using System.Globalization;
using System.IO;
using LHQ.Data;
using LHQ.Data.ModelStorage;
using LHQ.UnitTests.Visualizer;
// ReSharper disable UnusedMember.Global

namespace LHQ.UnitTests
{
    public abstract class TestsBase
    {
        //protected readonly Encoding Encoding = Encoding.UTF8;
        protected const string DefaultModelName = "Default";

        // ReSharper disable once MemberCanBePrivate.Global
        protected readonly CultureInfo CultureSlovak = new CultureInfo(1051);
        protected readonly CultureInfo CultureEnglish = new CultureInfo(1033);

        protected string GetSampleModelFileName(string fileName)
        {
            return Path.Combine(Environment.CurrentDirectory, "SampleModels", fileName);
        }

        protected void WriteNewLine()
        {
            Console.WriteLine(string.Empty);
        }

        protected void WriteLine(string msg)
        {
            Console.WriteLine(msg);
        }

        protected void WriteLineSeparator()
        {
            Console.WriteLine(new string('-', 80));
        }

        protected string SerializeModel(ModelContext modelContext)
        {
            return SerializeModel(modelContext, ModelSaveOptions.Default);
        }

        protected string SerializeModel(ModelContext modelContext, ModelSaveOptions modelSaveOptions)
        {
            throw new NotImplementedException();

            /*ArgumentValidator.EnsureArgumentNotNull(modelContext, "modelContext");

            return modelContext.Save(modelSaveOptions);*/
        }

        protected ModelLoadResult DeserializeModel(string modelXml, ModelContextOptions options, out ModelContext modelContext)
        {
            throw new NotImplementedException();
            /*modelContext = new ModelContext(options);
            ModelLoadResult loadResult = modelContext.Load(modelXml);
            return loadResult;*/
        }

        protected void VisualizeModel(ModelContext context)
        {
            VisualizeModel(context, new ModelVisualizerOptions(false, false));
        }

        protected void VisualizeModel(ModelContext context, ModelVisualizerOptions visualizerOptions)
        {
            ModelVisualizer visualizer = new ModelVisualizer(visualizerOptions);
            string visualization = visualizer.VisualizeModel(context.Model);
            Console.WriteLine("\n{0}", visualization);
        }

        protected void VisualizeModel(Model model, ModelVisualizerOptions visualizerOptions)
        {
            ModelVisualizer visualizer = new ModelVisualizer(visualizerOptions);
            string visualization = visualizer.VisualizeModel(model);
            Console.WriteLine("\n{0}", visualization);
        }

        protected ModelContext CreateEmptyModel(string modelName, ModelContextOptions modelOptions,
            bool onlyEnglishLanguage = false)
        {
            var ctx = new ModelContext(modelOptions);
            ctx.CreateModel(modelName);
            ctx.AddLanguage(CultureEnglish, false);

            if (!onlyEnglishLanguage)
            {
                ctx.AddLanguage(CultureSlovak, true);
            }
            return ctx;
        }

        protected ModelContext CreateSampleModel(string modelName, ModelContextOptions modelOptions)
        {
            var ctx = CreateEmptyModel(modelName, modelOptions);

            // add root categories
            var categAnimals = ctx.AddCategory("Animals");
            var categCars = ctx.AddCategory("Cars");
            var categMovieGenres = ctx.AddCategory("MovieGenres");

            // add some birds
            var categBirds = ctx.AddCategory("Birds", categAnimals);
            var resSeagull = ctx.AddResource("Seagull", categBirds);
            var resBlackbird = ctx.AddResource("Blackbird", categBirds);
            var resSparrow = ctx.AddResource("Sparrow", categBirds);

            // add some cars
            var resAudi = ctx.AddResource("Audi", categCars);
            var resBMW = ctx.AddResource("BMW", categCars);
            var resHyundai = ctx.AddResource("Hyundai", categCars);


            // add movie genres categories
            var categSciFi = ctx.AddCategory("Sci-fi", categMovieGenres);
            var categDrama = ctx.AddCategory("Drama", categMovieGenres);
            var categAction = ctx.AddCategory("Action", categMovieGenres);
            var categAdventure = ctx.AddCategory("Adventure", categMovieGenres);
            var categHorror = ctx.AddCategory("Horror", categMovieGenres);

            // add some movies
            var resAvengers = ctx.AddResource("Avengers", categSciFi);
            var resDracula = ctx.AddResource("Dracula", categHorror);

            ctx.AddResourceValue(CultureSlovak, "<dataXml>test</dataXml>", resDracula);

            // add resource parameters
            var rpBMWColor = ctx.AddResourceParameter("color", resBMW);

            //add resource values
            ctx.AddResourceValue(CultureSlovak, "BMW je najlepsie auto", resBMW);
            ctx.AddResourceValue(CultureEnglish, "BMW is the best car", resBMW);

            return ctx;
        }

        protected string SerializeJSONTest(ModelContext modelContext, ModelSaveOptions saveOptions, out bool isValid)
        {
            throw new NotImplementedException();
/*
            var serializer = new ModelSerializer_v1(modelContext);
            string json = serializer.Serialize(saveOptions);
            var loadResult = serializer.Deserialize(json);
            string result = null;
            if (loadResult.Status == ModelLoadStatus.Success)
            {
                result = serializer.Serialize(saveOptions);
            }

            isValid = result == json;
            return result;
*/
        }

        protected string SerializeToJSON(ModelContext modelContext, ModelSaveOptions saveOptions)
        {
            /*var dataNode = modelContext.SaveToDataNode();
            return DataNodeJsonSerializer.Serialize(dataNode, formatting);*/

            /*var serializer = new ModelSerializer_v1(modelContext);
            return serializer.Serialize(saveOptions);*/

            throw new NotImplementedException();


            /*var parameters = new JSONParameters
            {
                KVStyleStringDictionary = true,
                SerializeNullValues = false,
                UseUTCDateTime = true,
                UseValuesOfEnums = true,
                UseExtensions = false
                /*SerializerMaxDepth = 100,
                InlineCircularReferences = false,
                UsingGlobalTypes = false#1#
            };
            
            return JSON.ToJSON(model,parameters);*/
        }
    }
}