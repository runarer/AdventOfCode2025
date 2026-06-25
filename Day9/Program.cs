// Part 2 check all rectangles for walls crossing. And select the largest one
// A possible optimazation is to change part one to return a list of all the rectangles
// and then sort it with the larges first. Return top for part 1 answer, and check
// list form the top for crossing walls, the first one without any is the answer to part 2.

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

(int X, int Y)[] redSquares = [.. lines.Select(ParseLine)];

long LargestRectangle = FindLargestRectangle(redSquares);
long LargestInnerRectangle = FindLargestInnerRectangle(redSquares);

Console.WriteLine($"Part 1: {LargestRectangle}");
Console.WriteLine($"Part 2: {LargestInnerRectangle}");

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

static long FindLargestInnerRectangle((int X, int Y)[] squares)
{
    long largest = 0;

    //1. Create a list of walls for horizontal and vertical walls.
    List<Wall> horizontal = [];
    List<Wall> vertical = [];

    int currentIndex = 0;
    for (int i = 1; i < squares.Length; i++)
    {
        (int x1, int y1) = squares[i];
        (int x2, int y2) = squares[i-1];
        if (x1 == x2) // Vertical wall
        {
            int start = Math.Min(y1, y2);
            int end   = Math.Max(y1, y2);
            vertical.Add(new Wall(start, end, x1));
        }
        else // Horizontal wall
        {
            int start = Math.Min(x1, x2);
            int end   = Math.Max(x1, x2);
            horizontal.Add(new Wall(start, end, y1));
        }
        currentIndex = i;
    }

    //2. For each possible rectangle, check for walls crossing the rectangle.
    //   If there are no walls, check if it's the largest rectangle found so far.
    for (int i = 0; i < squares.Length; i++)
    {
        for (int j = i + 1; j < squares.Length; j++)
        {
            (int x1, int y1) = squares[i];
            (int x2, int y2) = squares[j];

            // MUST USE long so the multiplication becomes long.
            long size = (Math.Abs(x1 - x2) + 1L) * (Math.Abs(y1 - y2) + 1L);
            // If it's smaller than the largest found so far, skip it.
            if (size < largest)
                continue;
            
            bool conflictingWall = false;
            
            int xStart = Math.Min(x1, x2);
            int xEnd = Math.Max(x1, x2);
            int yStart = Math.Min(y1, y2);
            int yEnd = Math.Max(y1, y2);

            foreach(var wall in horizontal)
            {
                if( yStart < wall.Fixed && yEnd > wall.Fixed   &&  
                   ((wall.Start <= xStart && wall.End >= xEnd) ||
                    (wall.End > xStart && wall.End < xEnd)     || 
                    ( wall.Start > xStart && wall.Start < xEnd)))
                {
                    conflictingWall = true;
                    break;
                }
            }                  
            if(conflictingWall)
                continue;

            foreach (var wall in vertical)
            {
                if (xStart < wall.Fixed && xEnd > wall.Fixed &&
                  ((wall.Start <= yStart && wall.End >= yEnd)|| 
                   (wall.End > yStart && wall.End < yEnd)    || 
                   (wall.Start > yStart && wall.Start < yEnd)))
                {
                    conflictingWall = true;
                    break;
                }
            }
                
            if (!conflictingWall)
                largest = Math.Max(largest, size);
        }
    }

    return largest;
}

readonly struct Wall(int Start, int End, int Fixed)
{
    public int Start { get; } = Start;
    public int End { get; } = End;
    public int Fixed { get; } = Fixed;
}
