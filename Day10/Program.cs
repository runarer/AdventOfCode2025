using Day10;
using System.Data;

await using var stream = new FileStream(args[0], FileMode.Open, FileAccess.Read);
using var reader = new StreamReader(stream);

List<IndicatorLights> lights = [];

string? line;
int fewestPresses = 0;
while ((line = await reader.ReadLineAsync()) is not null)
{
   fewestPresses += await FindFewestPresses(line);
}

Console.WriteLine($"Part 1: {fewestPresses}");

return 0;


static async Task<int> FindFewestPresses(string line)
{
    var lights = Parser.CreateIndicatorLights(line);

    // do a breath first search, need to keep track of wich values reached
    List<int> reachedLights = [];
    Queue<(int, int)> toVisit = new();    
    // We start with 0,
    toVisit.Enqueue((0, 0));

    while(toVisit.Count > 0)
    {
        var (currentLights, presses) = toVisit.Dequeue();
        if(currentLights == lights.TargetLights)
            return presses;

        

        foreach(var button in lights.Buttons)
        {
            int newLights = currentLights ^ button;
            if(!reachedLights.Contains(newLights))
            {
                reachedLights.Add(newLights);
                toVisit.Enqueue((newLights, presses + 1));
            }
        }
    }
    // Should never happen and returning negative value could go unnoticed, so throw an exception instead
    throw new InvalidOperationException("No path found to target lights");
}
