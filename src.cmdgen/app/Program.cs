using CmdGen.Code;

using var generator = new Generator();
var extraData = new Dictionary<string, object>
{
    { "rootOutputFolder", "C:\\tmp\\" },
    { "namespace", "test.localization" }
};

var modelFile = "..\\..\\..\\Test.Localization\\Strings.lhq";

generator.Generate(modelFile, extraData);

// File.WriteAllText("output.cs", result);