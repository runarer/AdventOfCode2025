string[] lines;

try
{
    lines = File.ReadAllLines(args[0]);
} catch (Exception ex )
{
    Console.WriteLine(ex.Message);
    return 1;
}

char[][] batterieBanks = [.. lines.Select( (string line) => line.ToArray() )];

long totalJoltage = batterieBanks.Sum(bank => FindLargestJoltage(bank,2));
Console.WriteLine($"Part 1: {totalJoltage}");

long totalOverrideJoltage = batterieBanks.Sum(bank => FindLargestJoltage(bank,12));
Console.WriteLine($"Part 2: {totalOverrideJoltage}");

return 0;

static long FindLargestJoltage(char[] bank, int numberOfBatteries)
{
    char[] batteries = new char[numberOfBatteries];
    Array.Fill(batteries, '0');

    int next = 0;
    int batteriesLeft = numberOfBatteries - 1;

    for (int cur = 0; cur < batteries.Length; cur++)
    {
        for (int i = next; i < bank.Length - batteriesLeft; i++)
        {
            if (bank[i] > batteries[cur])
            {
                batteries[cur] = bank[i];
                next = i + 1;
            }

            if (bank[i] == '9')
                break;
        }
        batteriesLeft--;
    }

    return long.Parse(new string(batteries));
}
