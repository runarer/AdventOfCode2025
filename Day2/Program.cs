using System.ComponentModel.Design;

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
long allSillyNumbers = ranges.Sum(SumAllSillyNumbers);

Console.WriteLine($"Part 1: {sillyNumbers}");
Console.WriteLine($"Part 1: {allSillyNumbers}");

//Console.WriteLine($"Largest Number of digits: {LargestNumber(ranges)}");


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

    while (currentNumber <= range.End)
    {
        // If number of digits are odd, cant be silly number.
        // Jump to next non odd.
        int numberOfDigits = NumberOfDigits(currentNumber);
        if (numberOfDigits % 2 == 0)
            sum += SillyNumber(currentNumber, numberOfDigits, numberOfDigits/2);
        currentNumber++;
    }
    return sum;
}

/* For part 2 I think it might be better to produce silly numbers and 
 * check if they are in range. Then starting and stopping the production
 * at numbers based on the range. NO! Just a general solution fro part 1.
 */

//// First so research; find largest number -> It's 10
//long LargestNumber(Range[] ranges)
//{
//    return NumberOfDigits(ranges.Aggregate(0L, (a, b) => Math.Max(a, b.End)));
//}

long SumAllSillyNumbers(Range range)
{
    long sum = 0;
    long currentNumber = range.Start;

    while (currentNumber <= range.End)
    {        
        int numberOfDigits = NumberOfDigits(currentNumber);

        if(numberOfDigits == 2 || numberOfDigits == 3 || numberOfDigits == 5 || numberOfDigits == 7)
            sum += SillyNumber(currentNumber, numberOfDigits, 1);
        
        else if(numberOfDigits == 4 )
            sum += SillyNumber(currentNumber, numberOfDigits, 2);
        
        else if (numberOfDigits == 6 || numberOfDigits == 8 || numberOfDigits == 10)
        {
            long temp = SillyNumber(currentNumber, numberOfDigits, 2);
            if (temp == 0)
                sum += SillyNumber(currentNumber, numberOfDigits, numberOfDigits/2);
            else
                sum += temp;
        }
        
        else if(numberOfDigits == 9 )
            sum += SillyNumber(currentNumber, numberOfDigits, 3);
        
        currentNumber++;
    }

    return sum;
}

long SillyNumber(long number, int digits, int compare)
{    
    int first = (int)(number % Math.Pow(10,compare));
    int compared = compare;
    while(compared < digits)
    {
        int next = (int) ((number / Math.Pow(10,compared)) % Math.Pow(10, compare));
        if (first != next)
            return 0;
        compared += compare;
    }
    return number;
}

struct Range(long start, long end)
{
    public long Start = start; public long End = end;
}


