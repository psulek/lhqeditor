using System.Globalization;
using test.localization;

Thread.CurrentThread.CurrentUICulture = new CultureInfo("sk");

var welcome = Strings.Welcome("peter", "BA");
Console.WriteLine(welcome);
Console.ReadLine();