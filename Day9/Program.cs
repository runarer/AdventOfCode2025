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

(int X, int Y)[] RedSquares = [.. lines.Select(ParseLine)];

long LargestRectangle = FindLargestRectangle(RedSquares);

Console.WriteLine($"Part 1: {LargestRectangle}");


return 0;

static (int, int) ParseLine(string line)
{
    int[] numbers = [.. line.Split(',').Select(int.Parse)];
    return (numbers[0], numbers[1]);
}

static long FindLargestRectangle((int, int)[] squares)
{
    long largest = 0;

    for (int i = 0; i < squares.Length; i++)
    {
        for (int j = i + 1; j < squares.Length; j++)
        {
            (int x1, int y1) = squares[i];
            (int x2, int y2) = squares[j];

            // MUST USE long so the multiplication becomes long.
            long size = (Math.Abs(x1 - x2) + 1L) * (Math.Abs(y1 - y2) + 1L);
            largest = Math.Max(largest, size);
        }
    }

    return largest;
}

/*  Part 2 For each point there's a vector pointing towards other points.
 *  Ignore 1 wide/high rectangles.
    First determine if it points out of the shape or invards. 
    
    This will give me to sets, squares inside and squares outside.
    
    Foreach inside square, see if any of the outside squares are inside the
    borders. If not it's a contender.
 */

// Find top horizontal line -> this is max in rows
// (int row, int col) Corners
// (int row, int start, int end) horizontalLines
// (int col, int start, int end) verticalLines

//foreach horizontalLine check downwards if it hits other horizontal lines
// Calculate area
//Do the same for vertical lines