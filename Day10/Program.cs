using Day10;
using System.Data;

await using var stream = new FileStream(args[0], FileMode.Open, FileAccess.Read);
using var reader = new StreamReader(stream);

List<IndicatorLights> lights = [];

string? line;
int fewestPresses = 0;
int fewestPressesForJoltage = 0;
while ((line = await reader.ReadLineAsync()) is not null)
{
    var indicatorLights = Parser.CreateIndicatorLights(line);
    fewestPresses += await FindFewestPresses(indicatorLights);
    fewestPressesForJoltage += await FindFewestPressesForJoltage(indicatorLights);
}

Console.WriteLine($"Part 1: {fewestPresses}");
Console.WriteLine($"Part 2: {fewestPressesForJoltage}");

return 0;


static async Task<int> FindFewestPresses(IndicatorLights lights)
{
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

static async Task<int> FindFewestPressesForJoltage(IndicatorLights lights)
{
    // this is matrix with each row representing a button and each column representing a light, the value is 1
    // if the button toggles the light, 0 otherwise
    int[,] buttons = new int[lights.ButtonsArray.Length, lights.TargetJoltage.Length];

    // populate buttons matrix, we sort based on how many lights each button toggles, so we can try the buttons that
    // toggle more lights first, to hopefully reach the target faster
    var buttonsSorted = lights.ButtonsArray.OrderByDescending(lightList => lightList.Length).ToArray();
    for (int i = 0; i < buttonsSorted.Length; i++)
        for (int j = 0; j < buttonsSorted[i].Length; j++)
            buttons[i, buttonsSorted[i][j]] = 1;
    

    int[] target = lights.TargetJoltage;
    int[] current = new int[target.Length];
    int[] presses = new int[buttons.GetLength(0)];

    // set max presses
    int[] maxPresses = new int[buttons.GetLength(0)];
    for(int i = 0; i < maxPresses.Length; i++)
    {
        // For each button, the max presses is the minimum of the target joltage of the lights it toggles,
        // because pressing it more than that would add to much joltage.
        maxPresses[i] = target.Max();
        for(int j = 0; j < buttons.GetLength(1); j++)
        {
            if (buttons[i, j] == 1)
                maxPresses[i] = Math.Min(maxPresses[i], target[j]);
        }
    }


    // Finding the fewest presses is a combinatorial problem, we can solve it with a
    // backtracking algorithm, we try all combinations of button presses and keep
    // track of the minimum number of presses that reaches the target joltage.

    // We can optimize the backtracking by using the maxPresses array to limit the
    // number of presses for each button, and by trying the buttons that toggle more lights first,
    // to hopefully reach the target faster. This is why we sorted the buttons array.

    // We start by pressing buttons[0] max times. Then next button max times, and so on, until we reach
    // the target joltage or exceed the max presses for all buttons.


    return 0;
    // AI is usefull for commenting code.
}