// The border is clockwise for my input

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

    //1. The start index is the most norther and western tile.
    int row = squares.Min(s => s.Y);
    int col = squares.Where(s => s.Y == row).Min(s => s.X);
    int startIndex = Array.FindIndex(squares, s => s.X == col && s.Y == row);

    //2. Direction is determined by the next tile in the array. If it's to the right,
    //   clockwive, if it's down, counterclockwise.
    int nextIndex = (startIndex + 1) % squares.Length; // Wrap around to the beginning of the array if necessary

    //3. Create a list of walls for each direction.
    List<Wall> horizontal = [];
    List<Wall> vertical = [];

    int currentIndex = startIndex;
    for (int i = startIndex + 1; i != startIndex; i = (i + 1) % squares.Length)
    {
        (int x1, int y1) = squares[currentIndex];
        (int x2, int y2) = squares[i];
        if (x1 == x2) // Vertical wall
        {
            int start = Math.Min(y1, y2);
            int end   = Math.Max(y1, y2);
            if (y1 < y2) // East facing
                vertical.Add(new Wall(start, end, x1, Direction.East));
            else // West facing
                vertical.Add(new Wall(start, end, x1, Direction.West));
        }
        else //if (y1 == y2) // Horizontal wall
        {
            int start = Math.Min(x1, x2);
            int end   = Math.Max(x1, x2);
            if (x1 < x2) // North facing
                horizontal.Add(new Wall(start, end, y1, Direction.North));
            else // South facing
                horizontal.Add(new Wall(start, end, y1, Direction.South));
        }
        currentIndex = i;
    }

    //4. For each possible rectangle, make sure any wall inside the rectangle is only
    //   at the borders and that it's facing outwards.
    //   If so, calculate the area and update if it's the largest.
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
                if(yStart < wall.Fixed && yEnd > wall.Fixed)
                {
                    if ((wall.Start <= xStart && wall.End >= xEnd)||(wall.End > xStart && wall.End < xEnd) || ( wall.Start > xStart && wall.Start < xEnd) )
                    {
                        conflictingWall = true;
                        break;
                    }
                }                   
            }
            if(conflictingWall)
                continue;
            foreach (var wall in vertical)
            {
                if (xStart < wall.Fixed && xEnd > wall.Fixed)
                {
                    if ((wall.Start <= yStart && wall.End >= yEnd) || (wall.End > yStart && wall.End < yEnd) || (wall.Start > yStart && wall.Start < yEnd))
                    {
                        conflictingWall = true;
                        break;
                    }
                }
            }
                
            if (!conflictingWall)
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

readonly struct Wall(int Start, int End, int Fixed, Direction Direction)
{
    public int Start { get; } = Start;
    public int End { get; } = End;
    public int Fixed { get; } = Fixed;
    public Direction Direction { get; } = Direction;
}

enum Direction
{
    North,
    East,
    South,
    West
}

