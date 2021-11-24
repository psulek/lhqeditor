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

using System.Threading.Tasks;
using LHQ.Data;
using LHQ.Utils.Utilities;
using NUnit.Framework;

namespace LHQ.UnitTests
{
    [TestFixture]
    public class MergerTests : TestsBase
    {
        [Test]
        public void AnalyzeHierarchyChangesTest()
        {
            //var visualizerOptions = new ModelVisualizerOptions(false, true);
            string categoryNameCars = "Cars";
            string categoryNameDiesel = "Diesel";
            string categoryNamePetrol = "Petrol";
            //string resourceNameSkodaSuperB = "Skoda Superb 2.0 TDI";
            string resourceNameToyotaYaris = "Toyota Yaris 1.3";

            // prepare original model...
            ModelContext originalVersion = new ModelContext(ModelContextOptions.Default);
            originalVersion.CreateModel("Model1");
            originalVersion.Builder.AddLanguage(CultureEnglish, true);
            originalVersion.Builder.AddCategory(categoryNameCars, c =>
            {
                c.AddCategory(categoryNameDiesel, diesel => diesel.AddResource(resourceNameToyotaYaris));
                c.AddCategory(categoryNamePetrol);
            });

            // prepare branched model...
            /*ModelContext branchVersion = originalVersion.CreateBranch();

            var branchCategoryDiesel = branchVersion.FindCategory(ModelPath.Create(categoryNameCars, categoryNameDiesel));
            // add new resource 'Skoda SuperB...'
            branchVersion.AddResource(resourceNameSkodaSuperB, branchCategoryDiesel);
            // move resource from parent category 'diesel...' to new parent category 'petrol...'

            var branchResourceToyota = branchVersion.FindResourceByName(resourceNameToyotaYaris, branchCategoryDiesel);
            var branchCategoryPetrol = branchVersion.FindCategory(ModelPath.Create(categoryNameCars, categoryNamePetrol));
            branchVersion.MoveResource(branchResourceToyota, branchCategoryPetrol);


            //remove resourceNameToyotaYaris from original model
            /*var originalCategoryDiesel = originalVersion.FindCategory(ModelPath.Create(categoryNameCars, categoryNameDiesel));
            originalVersion.RemoveResource(resourceNameToyotaYaris, originalCategoryDiesel);#1#

            // do merge and visualize result...
            MergeModels(originalVersion, branchVersion, visualizerOptions);*/
        }

        [Test]
        public async Task MergeTwoFilesTest()
        {
            string branchFile = GetSampleModelFileName("BranchModelA.lhqt");
            string branchFileContent = await FileUtils.ReadAllTextAsync(branchFile);

            string originFile = GetSampleModelFileName("OriginModelA.lhq");
            string originFileContent = await FileUtils.ReadAllTextAsync(originFile);

            /*var branchContext = new ModelContext(ModelContextOptions.Default);
            branchContext.Load(branchFileContent);

            var originalContext = new ModelContext(ModelContextOptions.Default);
            originalContext.Load(originFileContent);*/

            //var visualizerOptions = new ModelVisualizerOptions(false, true);
            //MergeModels(originalContext, branchContext, visualizerOptions);
        }

        [Test]
        public void AnalyzeSimpleChangesTest()
        {
            //var visualizerOptions = new ModelVisualizerOptions(false, true);

            string originalCategoryNameCars = "Cars";
            string originalCategoryNameMovies = "Movies";
            string originalCategoryNameButtons = "Buttons";
            string originalCategoryNameLego = "Lego";
            string originalCategoryNameBuggati = "Buggati";

            //string branchCategoryNameDevelopers = "Developers";

            // prepare original model...
            ModelContext originalVersion = new ModelContext(ModelContextOptions.Default);
            originalVersion.CreateModel("Model1");
            originalVersion.Builder.AddLanguage(CultureEnglish, true);
            originalVersion.Builder.AddCategory(originalCategoryNameCars);
            originalVersion.Builder.AddCategory(originalCategoryNameMovies);
            originalVersion.Builder.AddCategory(originalCategoryNameButtons);
            originalVersion.Builder.AddCategory(originalCategoryNameLego);
            originalVersion.Builder.AddCategory(originalCategoryNameBuggati);

            WriteLine("Original model:");
            VisualizeModel(originalVersion);

            // prepare branched model...
            /*ModelContext branchVersion = originalVersion.CreateBranch();

            WriteLine("Branch created..");
            WriteNewLine();

            // rename category 'cars' to 'cars-new'
            Category branchCategoryCars = branchVersion.FindCategoryByName(originalCategoryNameCars, null);
            branchCategoryCars.Name += "-new";
            branchVersion.IncreaseModelVersion();

            // rename category 'movies' in original model to 'movies-orig'
            var originalCategoryMovies = originalVersion.FindCategoryByName(originalCategoryNameMovies, null);
            originalCategoryMovies.Name += "-orig";
            originalVersion.IncreaseModelVersion();

            // add new category 'Developers' on branched version of model
            var branchCategoryDevelopers = branchVersion.Builder.AddCategory(branchCategoryNameDevelopers);

            // remove category 'Buttons' from original model
            originalVersion.RemoveCategory(originalCategoryNameButtons, null);
            // update name of category 'Buttons' in branch model to new 'Buttons-new'
            var branchCategoryButtons = branchVersion.FindCategoryByName(originalCategoryNameButtons, null);
            branchCategoryButtons.Name += "-new";
            branchVersion.IncreaseModelVersion();

            // remove category 'Lego' from original model
            originalVersion.RemoveCategory(originalCategoryNameLego, null);

            // remove category 'Buggati' from both models
            originalVersion.RemoveCategory(originalCategoryNameBuggati, null);
            branchVersion.RemoveCategory(originalCategoryNameBuggati, null);

            // do merge and visualize result...
            MergeModels(originalVersion, branchVersion, visualizerOptions);*/
        }

//        private void MergeModels(ModelContext originalModelContext, ModelContext branchedModelContext, ModelVisualizerOptions visualizerOptions)
//        {
//            WriteNewLine();
//            WriteLine("Original Model:");
//            VisualizeModel(originalModelContext, visualizerOptions);
//
//            WriteLineSeparator();
//            WriteLine("Branched Model:");
//            VisualizeModel(branchedModelContext, visualizerOptions);
//            WriteLineSeparator();
//            WriteLineSeparator();
//
//            WriteLine("Merging branched model (merge source) -> original model (merge target)...");
//            WriteNewLine();
//
//            using (var modelMerger = ModelContext.CreateMerger(branchedModelContext, originalModelContext))
//            {
//                ModelMergeResult mergeResult = modelMerger.Start(ModelMergeAction.Analyze);
//                WriteLine("mergeResult status: " + mergeResult.Status);
//                WriteLineSeparator();
//                foreach (var analyzeResult in mergeResult.AnalyzeResults.GroupBy(x => x.AnalyzeItemStatus))
//                {
//                    ModelMergeAnalyzeItemStatus analyzeItemStatus = analyzeResult.Key;
//                    WriteLine("Analyze Status: {0}, {1} elements".FormatWith(analyzeItemStatus, analyzeResult.Count()));
//
//                    foreach (var analyzeResultItem in analyzeResult)
//                    {
//                        PrintMergeAnalyzeItem("Source", analyzeResultItem.SourceItem);
//                        WriteNewLine();
//                        PrintMergeAnalyzeItem("Target", analyzeResultItem.TargetItem);
//                        WriteLineSeparator();
//                    }
//                }
//            }
//        }

//        private void PrintMergeAnalyzeItem(string title, ModelMergeAnalyzeItem item)
//        {
//            var itemElement = item.ModelElement;
//
//            if (itemElement == null)
//            {
//                WriteLine("\t{0}: N/A".FormatWith(title));
//            }
//            else
//            {
//                WriteLine("\t{0}: {1}, {2}".FormatWith(title, itemElement.ElementType.ToShortString(), itemElement.GetDisplayName()));
//            }
//
//            var itemChanges = item.Changes;
//            if (itemChanges.Count == 0)
//            {
//                WriteLine("\tNO changes");
//            }
//            else
//            {
//                WriteLine("\t{0} changes:".FormatWith(itemChanges.Count));
//                foreach (var changeInfo in itemChanges)
//                {
//                    WriteLine("\t   Field:{0}, Change:{1}, Accept:{2} {3}".FormatWith(changeInfo.FieldName, changeInfo.ChangeType, changeInfo.AcceptOption, 
//                        changeInfo.ReadonlyAcceptOption ? "  [Readonly accept]" : string.Empty));
//                }
//            }
//        }

//        [Test]
//        public void VisualizeBranchedModelTest()
//        {
//            ModelContext originalVersion = CreateSampleModel("SampleModel", ModelContextOptions.Default);
//            ModelContext branchVersion = originalVersion.CreateBranch();
//
//            WriteLine("Original Model:");
//            WriteNewLine();
//            VisualizeModel(originalVersion);
//
//            WriteLineSeparator();
//            WriteLine("Branched Model:");
//            WriteNewLine();
//            VisualizeModel(branchVersion);
//        }

//        [Test]
//        public void SerializeBranchedModelTest()
//        {
//            ModelContext originalVersion = CreateSampleModel("SampleModel", ModelContextOptions.Default);
//            var authoring = originalVersion.GetMetadata().GetAuthoring(originalVersion.Model);
//            var utcNow = DateTime.UtcNow;
//            
//            authoring.Create("Peter", utcNow.AddHours(-1), false);
//            authoring.Modify("Michael", utcNow);
//            
//            ModelContext branchVersion = originalVersion.CreateBranch();
//            branchVersion.Should().NotBeNull();
//
//            branchVersion.Should().NotBe(originalVersion);
//
////            var options = new DefaultXmlModelSerializationOptions
////            {
////                Formatting = Formatting.Indented
////            };
//
//            ModelSaveOptions options = new ModelSaveOptions(true);
//            string content = originalVersion.Save(options);
//            WriteLine("Original Model (length: {0})".FormatWith(content.Length));
//            WriteNewLine();
//            WriteLine(content);
//            
//            WriteLineSeparator();
//
//            content = branchVersion.Save(options);
//            WriteLine("Branched Model (length: {0})".FormatWith(content.Length));
//            WriteNewLine();
//            WriteLine(content);
//
//
//            //string json = SerializeToJSON(branchVersion, Newtonsoft.Json.Formatting.None);
//            //bool isValid;
//
//
//            /*var modelSaveOptions = ModelSaveOptions.Default;
//
//            string json = SerializeJSONTest(branchVersion, modelSaveOptions, out isValid);
//            WriteLine("Serialize/Deserialize JSON test, valid: " + isValid);
//            WriteNewLine();
//            WriteLine("Length: " + json.Length);
//            WriteNewLine();
//            WriteLine(json);*/
//
//            
//            // deserialize
//            //var xx = branchVersion.LoadFromXml(xml, Encoding.UTF8);
//
//
//            dynamic dyn = new ExpandoObject();
//            dyn.Languages = new Dictionary<string, Language>();
//
//            foreach (Language language in branchVersion.Model.Languages)
//            {
//                dyn.Languages[language.Name] = language;
//            }
//
//            var dynJson = JSON.ToJSON(dyn);
//            WriteLine("Dynamic JSON: " + dynJson);
//*/
//
//
//            
//        }
    }
}