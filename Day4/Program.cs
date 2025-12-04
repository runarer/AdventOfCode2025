string[] lines;
try
{
    lines = File.ReadAllLines(args[0]);
} catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    return 1;
}

char[][] map = TransformMap(lines);

Console.WriteLine($"Part 1: {CountAccessablePaperRolls(map)}");
Console.WriteLine($"Part 2: {CountAndRemovePaperRolls(map)}");

return 0;

/* Transform it into a char array and adds a border.
 * The char array is for part to that require change to the map.
 * The border avoids a lot of out of bound checks.
 */
char[][] TransformMap(string[] lines)
{
    char[][] map = new char[lines.Length+2][];
    map[0] = new char[ lines[0].Length +2 ];
    Array.Fill(map[0], '.');

    for(int i = 0; i < lines.Length; i++)
    {
        map[i+1] = new char[lines[0].Length + 2];
        map[i + 1][0] = '.';
        for(int j = 0; j < lines[i].Length; j++)
        {
            map[i+1][j+1] = lines[i][j];
        }
        map[i + 1][^1] = '.';
    }

    map[^1] = new char[lines[0].Length + 2];
    Array.Fill(map[^1], '.');

    return map;
}

int CountAccessablePaperRolls( char[][] lines )
{
    int count = 0;
    for (int i = 1; i < lines.Length-1; i++)
    {
        for (int j = 1; j < lines[i].Length-1; j++)
        {
            if (lines[i][j] == '.')
                continue;

            int rolls = 0;
            // North West
            if (lines[i-1][j-1] == '@')
                    rolls++;

            // North
            if(lines[i - 1][j] == '@')
                rolls++;

            // North East
            if (lines[i - 1][j + 1] == '@')
                rolls++;

            // West
            if (lines[i][j-1] == '@')
                rolls++;

            // East
            if (lines[i][j + 1] == '@')
                rolls++;

            // South West
            if (lines[i+1][j - 1] == '@')
                rolls++;

            // South
            if (lines[i + 1][j] == '@')
                rolls++;

            // South East
            if (lines[i + 1][j + 1] == '@')
                rolls++;

            if (rolls < 4)
                count++;                
        }
    }
    return count;
}

int CountAndRemovePaperRolls(char[][] lines)
{
    int count = 0;
    bool removedRoll = true;

    while (removedRoll) {
        removedRoll = false;
        for (int i = 1; i < lines.Length - 1; i++)
        {
            for (int j = 1; j < lines[i].Length - 1; j++)
            {
                if (lines[i][j] == '.')
                    continue;

                int rolls = 0;
                // North West
                if (lines[i - 1][j - 1] == '@')
                    rolls++;

                // North
                if (lines[i - 1][j] == '@')
                    rolls++;

                // North East
                if (lines[i - 1][j + 1] == '@')
                    rolls++;

                // West
                if (lines[i][j - 1] == '@')
                    rolls++;

                // East
                if (lines[i][j + 1] == '@')
                    rolls++;

                // South West
                if (lines[i + 1][j - 1] == '@')
                    rolls++;

                // South
                if (lines[i + 1][j] == '@')
                    rolls++;

                // South East
                if (lines[i + 1][j + 1] == '@')
                    rolls++;

                if (rolls < 4) 
                {
                    removedRoll = true;
                    count++;
                    lines[i][j] = '.';
                }
                
            }
        }
    }
    return count;
}