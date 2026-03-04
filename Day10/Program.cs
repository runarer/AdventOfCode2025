using Day10;
using System.Diagnostics;

var stopWatch = Stopwatch.StartNew();

string[] lines = [];

//try
//{
//    lines = await File.ReadAllLinesAsync(args[0]);
//}
//catch (Exception ex)
//{
//    Console.WriteLine(ex.Message);
//    return 1;
//}

//IndicatorLights[] lights = [.. lines.Select(Parser.CreateIndicatorLights)];




await using var stream = new FileStream(args[0], FileMode.Open, FileAccess.Read);
using var reader = new StreamReader(stream);

List<IndicatorLights> lights = [];

string? line;
int fewestPresses = 0;
while ((line = await reader.ReadLineAsync()) is not null)
{
    fewestPresses += await FindFewestPresses(line);
}


stopWatch.Stop();
Console.WriteLine($"Time Async: {stopWatch.ElapsedMilliseconds}");

return 0;


static async Task<int> FindFewestPresses(string line)
{
    var lights = Parser.CreateIndicatorLights(line);

    return 0;
}

