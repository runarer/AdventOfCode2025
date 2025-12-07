using System.Collections.Generic;

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

List<(long, long)> nonOverlappingRanges = GetNonOverlappingRanges(ranges);
long totalFresh = nonOverlappingRanges.Sum(i => i.Item2 - i.Item1 + 1);

Console.WriteLine($"Part 2: {totalFresh}");

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

List<(long, long)> GetNonOverlappingRanges(List<(long, long)> ranges)
{
    List<(long, long)> nonOverlappingRanges = [];

    foreach (var range in ranges)
    {
        long start = range.Item1;
        long end = range.Item2;

        bool insideAnother = false;

        for (var i = 0; i < nonOverlappingRanges.Count; i++)
        {
            long curStart = nonOverlappingRanges[i].Item1;
            long curEnd   = nonOverlappingRanges[i].Item2;

            // If new range is inside of current, skip new range
            if (start >= curStart && end <= curEnd)
            {
                insideAnother = true;
                break;
            }

            // If current is inside of new range, delete current
            if (start <=  curStart &&  end >= curEnd)
            {
                nonOverlappingRanges.RemoveAt(i);
                i--;
                continue;
            }

            // If current is oveerlapping on left side
            if( start < curStart && end >= curStart && end <= curEnd)
            {
                end = curStart-1;
                continue;
            }

            // If current is oveerlapping on right side
            if (start >= curStart && start <= curEnd && end > curEnd)
            {
                start = curEnd + 1;
                continue;
            }
        }
        
        if(!insideAnother)
        {
            nonOverlappingRanges.Add((start,end));
        }
    }

    return nonOverlappingRanges;
}