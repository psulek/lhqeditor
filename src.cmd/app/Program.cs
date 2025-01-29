using LHQ.Cmd;

var testDataFolder = Path.GetFullPath("..\\..\\..\\Test.Localization");

using var generator = new Generator();
var extraData = new Dictionary<string, object>
{
    { "rootOutputFolder", Path.Combine(testDataFolder, "GenOutput") },
    { "namespace", "test.localization" }
};

var modelFile = Path.Combine(testDataFolder, "Strings.lhq");


generator.Generate(modelFile, extraData);

// File.WriteAllText("output.cs", result);