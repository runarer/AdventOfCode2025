string[] lines;
try
{
    lines = File.ReadAllLines(args[0]);
} catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    return 1;
}

var (ingredients, ranges) = ParseIngredientsList(lines);

Console.WriteLine($"Part 1: {CountFreshIngredients(ingredients,ranges)}");
//Console.WriteLine($"Part 2: {}");

return 0;

(List<long>, List<(long,long)>) ParseIngredientsList(string[] lines)
{
    bool rangeLines = true;
    List<long> ingredients = [];
    List<(long, long)> ranges = [];

    foreach (var line in lines)
    {
        if (line == "")
        {
            rangeLines = false; 
            continue;
        }

        if(rangeLines)
        {
            var range = line.Split("-", 2);
            ranges.Add((long.Parse(range[0]), long.Parse(range[1])));
        }
        else
        {
            ingredients.Add(long.Parse(line));
        }
    }

    return (ingredients, ranges);
}

int CountFreshIngredients(List<long> ingredients, List<(long, long)> ranges)
{
    int freshIngredients = 0;

    foreach(var ingredient in ingredients)    
        foreach( var (start,end) in ranges)    
            if (ingredient >= start && ingredient <= end) {
                freshIngredients++;
                break;
            }


    return freshIngredients;
}

