string[] lines;
try
{
    lines = File.ReadAllLines(args[0]);
} catch(Exception ex)
{
    Console.WriteLine(ex.ToString());
    return 1;
}

int[] numbers = [.. lines.Select(ParseLine)];


Console.WriteLine($"Part 1: {CountDialAtZero(numbers)}");
Console.WriteLine($"Part 2: {CountPassesAtZero(numbers)}");
return 0;

int CountDialAtZero(int[] numbers)
{
    int count = 0;
    int dial = 50;

    foreach(int number in numbers)
    {
        dial = (dial + number) % 100;
        if(dial == 0)
            count++;
    }

    return count;
}

int CountPassesAtZero(int[] numbers)
{
    int count = 0;
    int dial = 50;
    
    // Need this, so we dont count starting at 0 and go negative.
    int prevDial = dial; 

    foreach(int number in numbers)
    {
        int tempDial = dial + number;

        // Avoid counting negatives that start from zero.
        if (tempDial == 0|| (tempDial < 0 && prevDial != 0))
            count++;

        count += Math.Abs(tempDial / 100);
        
        dial = (tempDial % 100 + 100) % 100;
        prevDial = dial;
    }


    return count;
}



int ParseLine(string line)
{
    int deminer = line[0] == 'R' ? 1 : -1; 
    int number = int.Parse(line.Substring(1));
    return deminer * number;
}

