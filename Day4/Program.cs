string[] lines;
try
{
    lines = File.ReadAllLines(args[0]);
} catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    return 1;
}



Console.WriteLine($"Part 1: {CountAccessablePaperRolls(lines)}");
//Console.WriteLine($"Part 2: {}");

return 0;

int CountAccessablePaperRolls( string[] lines )
{
    int count = 0;
    for (int i = 0; i < lines.Length; i++)
    {
        for (int j = 0; j < lines[i].Length; j++)
        {
            if (lines[i][j] == '.')
                continue;

            int rolls = 0;
            // North West
            if (i > 0 && j > 0 && lines[i-1][j-1] == '@')
                    rolls++;

            // North
            if(i > 0 && lines[i - 1][j] == '@')
                rolls++;

            // North East
            if (i > 0 && j < lines[i].Length-1 && lines[i - 1][j + 1] == '@')
                rolls++;

            // West
            if (j > 0 && lines[i][j-1] == '@')
                rolls++;

            // East
            if (j < lines[i].Length-1 && lines[i][j + 1] == '@')
                rolls++;

            // South West
            if ( i < lines.Length-1 && j > 0 && lines[i+1][j - 1] == '@')
                rolls++;

            // South
            if (i < lines.Length-1 && lines[i + 1][j] == '@')
                rolls++;

            // South East
            if (i < lines.Length-1 && j < lines[i].Length - 1 && lines[i + 1][j + 1] == '@')
                rolls++;

            if (rolls < 4)
                count++;
        }
    }
    return count;
}