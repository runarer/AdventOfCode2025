using Day10;

string[] lines = [];

try
{
    lines = await File.ReadAllLinesAsync(args[0]);
} catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    return 1;
}

IndicatorLights[] lights = [..lines.Select(Parser.CreateIndicatorLights)];


return 0;

