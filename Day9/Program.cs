string[] lines = [];

try
{
    lines = File.ReadAllLines(args[0]);
} catch (Exception ex)
{
    Console.WriteLine(ex.Message); 
    return 1;
}

(int X, int Y)[] RedSquares = [.. lines.Select(ParseLine)];

long LargestRectangle = FindLargestRectangle(RedSquares);

Console.WriteLine($"Part 1: {LargestRectangle}");


return 0;

static (int, int) ParseLine(string line) 
{
    int[] numbers = [..line.Split(',').Select(int.Parse)];
    return (numbers[0], numbers[1]);
}

static long FindLargestRectangle((int, int)[] squares)
{
    long largest = 0;

    for(int i = 0; i < squares.Length; i++)
    {
        for(int j = i+1; j < squares.Length; j++)
        {
            (int x1, int y1) = squares[i];
            (int x2, int y2) = squares[j];

            // MUST USE long so the multiplication becomes long.
            long size = (Math.Abs(x1-x2) + 1L) * (Math.Abs(y1-y2) + 1L);
            largest = Math.Max(largest, size);
        }
    }

    return largest;
}

// 2147440389 to low