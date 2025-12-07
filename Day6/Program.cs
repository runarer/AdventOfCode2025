using System.Text;

string[] lines;

try
{
    lines = File.ReadAllLines(args[0]); 
} catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    return 1;
}

string[][] numbers = [..lines[..^1].Select(s => s.Trim().Split(' ',StringSplitOptions.RemoveEmptyEntries))];
char[] operators = operators = [.. lines[^1].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => s[0])];


Console.WriteLine($"Part 1: {CalculateAll(operators,numbers)}");
Console.WriteLine($"Part 2: {CalcutaleRightToLeft(operators, lines[..^1])}");

return 0;


static long CalculateAll(char[] operators, string[][] numbers)
{
    long total = 0;

    for (int i = 0; i < operators.Length; i++)
    {
        long sum = (operators[i] == '*') ? 1 : 0;

        for (int j = 0; j < numbers.Length; j++)
        {
            if (operators[i] == '*')            
                sum *= int.Parse(numbers[j][i]);
            else
                sum += int.Parse(numbers[j][i]);
        }
        total += sum;
    }

    return total;
}

static long CalcutaleRightToLeft(char[] operators, string[] numbers)
{
    long total = 0;
    int operand = 0;
    List<long> partNumbers = [];

    for (int i = 0; i < numbers[0].Length; i++) {        
        bool numberStarted = false;
        StringBuilder numberAsString = new StringBuilder(12);
        for(int line = 0; line < numbers.Length; line++)
        {
            // Skip starting whitespaces
            if (!numberStarted && Char.IsWhiteSpace(numbers[line][i]))
                continue;

            // Done
            if (numberStarted && Char.IsWhiteSpace(numbers[line][i]))
                break;

            numberStarted = true;
            numberAsString.Append(numbers[line][i]);            
        }
        string s  = numberAsString.ToString();
        bool gotNumber = long.TryParse(s, out long n);
        if (gotNumber)
        {
            partNumbers.Add(n);
        } 
        else
        {
            total += Calculate(partNumbers, operators[operand]);

            partNumbers.Clear();
            operand++;
        }
    }
    total += Calculate(partNumbers, operators[operand]);

    return total;
}

static long Calculate(ICollection<long> numbers, char op)
{
    long sum = (op == '*') ? 1 : 0;

    foreach (var number in numbers)
    {
        if (op == '*')
            sum *= number;
        else
            sum += number;
    }
    return sum;
}