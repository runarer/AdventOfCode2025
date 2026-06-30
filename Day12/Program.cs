using System.Text.RegularExpressions;

string[] lines = [];

try
{
    lines = File.ReadAllLines(args[0]);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    return 1;
}

List<Puzzle> puzzles = [];

foreach(string line in lines.Skip(30))
{
    puzzles.Add(ParseLine(line));
}

return 0;


static Puzzle ParseLine(string line)
{
    string pattern = @"(\d+)x(\d+): (\d+) (\d+) (\d+) (\d+) (\d+) (\d+)";
    var result = Regex.Match(line, pattern);
    if (result.Success)
    {
        var values = result.Groups[0];

        return new Puzzle(
            int.Parse(result.Groups[1].Value),
            int.Parse(result.Groups[2].Value),
            int.Parse(result.Groups[3].Value),
            int.Parse(result.Groups[4].Value),
            int.Parse(result.Groups[5].Value),
            int.Parse(result.Groups[6].Value),
            int.Parse(result.Groups[7].Value),
            int.Parse(result.Groups[8].Value)
            );

    }
    
    throw new Exception("A line did not match regex.");
}

record Puzzle(int Height, int Width, int Present0, int Present1, int Present2, int Present3, int Presesnt4, int Present5);