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

int totalJoltage = batterieBanks.Sum(FindLargestJoltage);
Console.WriteLine($"Part 1: {totalJoltage}");

long totalOverrideJoltage = batterieBanks.Sum(FindLargestOverrideJoltage);
Console.WriteLine($"Part 2: {totalOverrideJoltage}");

return 0;

static int FindLargestJoltage(char[] bank)
{
    int firstBattery = 0;
    char firstLargest = '0';    
    for(int i = 0;i <bank.Length-1;i++)
    {
        if (bank[i] > firstLargest)
        {
            firstLargest = bank[i];
            firstBattery = i;
        }            
        
        if (bank[i] == '9')
            break;
    }
    
    
    int secondBattery = firstBattery+1;
    char secondLargest = bank[secondBattery];
    for (int i = secondBattery; i < bank.Length; i++)
    {
        if (bank[i] > secondLargest)
        {
            secondLargest = bank[i];
            secondBattery = i;
        }

        if (bank[i] == '9')
            break;
    }

    return int.Parse(""+firstLargest+secondLargest);
}

static long FindLargestOverrideJoltage(char[] bank)
{
    char[] batteries = ['0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0'];
    int next = 0;
    int batteriesLeft = 11;

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

