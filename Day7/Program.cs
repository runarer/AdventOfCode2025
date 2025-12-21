string[] lines = [];
try
{
    lines = File.ReadAllLines(args[0]);
} catch ( Exception ex)
{
    Console.WriteLine(ex.Message);
    return 1;
}

char[,] map = CreateMap(lines);
int startIndex = FindStartIndex(map);

// Set first beam, I know from input that it's not ^
map[1,startIndex] = '|';

int beamSplitTotal = FillInBeams(map);
long timelines = CountTimeLines(map, startIndex);

Console.WriteLine($"Part 1: {beamSplitTotal}");
Console.WriteLine($"Part 2: {timelines}");

return 0;

static char[,] CreateMap(string[] lines)
{
    char[,] map = new char[lines.Length, lines[0].Length];

    for (int i = 0; i < lines.Length; i++)
    {
        for (int j = 0; j < lines[i].Length; j++)
        {
            map[i, j] = lines[i][j];
        }
    }
    return map;
}

static int FindStartIndex(char[,] map)
{
    for(int i = 0;i < map.GetLength(0);i++)
    {
        if (map[0,i] == 'S') return i;
    }
    return -1;
}

static int FillInBeams(char[,] map)
{
    int splits = 0;
    int rows = map.GetLength(0);
    int cols = map.GetLength(1);

    // Skip first, it doesn't contain anything but the startpostion.
    // Skip last, the beam is done travling.
    for(int i = 1; i < rows-1; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            if (map[i,j] == '|') {
                if (map[i + 1, j] == '^')
                {
                    splits++;
                    if(j - 1  >= 0)
                        map[i + 1, j - 1] = '|';
                    if(j + 1 <  cols)
                        map[i + 1, j + 1] = '|';
                }
                else
                {
                    map[i + 1,j] = '|';
                }
            }
        }
    }
    return splits;
}

static long CountTimeLines(char[,] map, int startIndex)
{
    int rows = map.GetLength(0);
    int cols = map.GetLength(1);

    // When processing each line, we only need timelines from the line above.
    long[] oldTimeLines = new long[cols];
    Array.Fill(oldTimeLines, 0);
    oldTimeLines[startIndex] = 1;
    
    long[] newTimeLines = new long[cols];
    Array.Fill(newTimeLines, 0);

    //
    for(int i = 1; i < rows - 1; i++)
    {
        for(int j = 0; j < cols; j++)
        {
            if (oldTimeLines[j] > 0)
            {
                //We have a beam, check map if it's a spliter
                if (map[i+1, j] == '^')
                {
                    if (j - 1 >= 0)
                        newTimeLines[j - 1] += oldTimeLines[j];
                    if (j + 1 < cols)
                        newTimeLines[j + 1] += oldTimeLines[j];
                }
                else
                {
                    newTimeLines[j] += oldTimeLines[j];
                }
            }
        }
        (oldTimeLines,newTimeLines) = (newTimeLines,oldTimeLines);
        Array.Fill(newTimeLines, 0);
    }

    return oldTimeLines.Sum();
}