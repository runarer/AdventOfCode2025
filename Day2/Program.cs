string line;
try
{
    line = File.ReadAllText(args[0]).Trim();
} catch(Exception ex)
{
    Console.WriteLine(ex.Message);
    return 1;
}

Range[] ranges = [..line.Split(',').Select(CreateRange)];
long sillyNumbers = ranges.Sum(SumSillyNumbers);

Console.WriteLine($"Part 1: {sillyNumbers}");

return 0;



Range CreateRange(string rangeLine)
{    
    long[] result = [..rangeLine.Split('-').Select(long.Parse)];
    return new Range(result[0], result[1]);
}

int NumberOfDigits(long number)
{
    return (int)Math.Floor(Math.Log10((double)number) + 1);
}

long SumSillyNumbers(Range range)
{
    long sum = 0;
    long currentNumber = range.Start;

    while(currentNumber <= range.End)
    {
        // If number of digits are odd, cant be silly number.
        // Jump to next non odd.
        int numberOfDigits = NumberOfDigits(currentNumber);
        if (numberOfDigits % 2 != 0 ) {
            currentNumber = (long)Math.Pow(10 ,numberOfDigits);
            continue;
        }
        // integer division gives us the first n/2 digits,
        // mod gives us the second n/ digits.
        int splitter = (int)Math.Pow(10,(numberOfDigits / 2));
        if ((currentNumber % splitter) == (currentNumber / splitter)) 
            sum += currentNumber;
        currentNumber++;
    }

    return sum;
}

struct Range(long start, long end)
{
    public long Start = start; public long End = end;
}


