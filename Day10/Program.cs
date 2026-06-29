/*
    Den mest optimale måten å løse del 2 på er med LP, dette kan jeg ikke så en bruteforce metode er brukt.
    
    Bruker Gauss Elimination for å finne løsninger. Dersom systemet ikke går opp så permuteres det rundt et begrenset
    antall ukjente, for så å regne ut svaret.
 */

using Day10;

int fewestPresses = 0;
int fewestPressesForJoltage = 0;

// Read all lines first so we can show a progress indicator with a known total.
var allLines = await File.ReadAllLinesAsync(args[0]);
int total = allLines.Length;
var processingTasks = new List<Task<(int presses, int joltagePresses)>>();


// Solve the puzzles, async since each line is its own puzzle. 
for (int i = 0; i < total; i++)
{
    var line = allLines[i];
    var indicatorLights = Parser.CreateIndicatorLights(line);

    int index = i;
    var localIndicatorLights = indicatorLights;


        processingTasks.Add(Task.Run(async () =>
        {
            var p1 = await FindFewestPresses(localIndicatorLights);
            var p2 = await FindFewestPressesGauss(localIndicatorLights);
            return (p1, p2);
        }));
    
}

if (processingTasks.Count > 0)
{
    var results = await Task.WhenAll(processingTasks);
    fewestPresses = results.Sum(r => r.presses);
    fewestPressesForJoltage = results.Sum(r => r.joltagePresses);
}

Console.WriteLine($"Part 1: {fewestPresses}");
Console.WriteLine($"Part 2: {fewestPressesForJoltage}");

return 0;


static async Task<int> FindFewestPressesGauss(IndicatorLights lights)
{
    Fraction[,] matrix = CreateMatrix(lights);
    GaussElimination(lights, matrix);

    int[,] intMatrix = ConvertToIntegerMatrix(matrix);
    int presses = ReduceKnownValues(intMatrix);

    bool allZero = true;
    foreach(var number in intMatrix)
    {

        if(number != 0)
        {
            allZero = false;
            break;
        }
    }
    
    if(allZero)
        return presses;
    return presses +  PermuteOnUnknowns(lights,intMatrix);
}
 
static List<int> FindUnknowns(int[,] matrix)
{
    bool[] unknowns = new bool[matrix.GetLength(1) - 1];
    Array.Fill(unknowns, true);

    // Find the unknowns
    for (int row = 0; row < matrix.GetLength(0) && row < matrix.GetLength(1) - 1; row++)
    {
        if (matrix[row, row] > 0)
            unknowns[row] = false;
    }

    for (int idx = 0; idx < unknowns.Length; idx++)
    {
        bool allZeroes = true;
        for (int row = 0; row < matrix.GetLength(0); row++)
            if (matrix[row, idx] != 0)
            {
                allZeroes = false;
                break;
            }
        if (allZeroes)
        {
            unknowns[idx] = false;
        }
    }

    List<int> unknownIndexs = [];
    for (int i = 0; i < unknowns.Length; i++)
        if (unknowns[i])
            unknownIndexs.Add(i);
    return unknownIndexs;
}

static int PermuteOnUnknowns(IndicatorLights lights, int[,] matrix)
{

    List<int> unknownIndexs = FindUnknowns(matrix);

    // Find max values for unknowns
    int[] maxValues = new int[unknownIndexs.Count];

    // Det er en feil her ved max verdier, fungerer på de fleste, men det er flere som ikke
    // finner løsninger innenfor begrensningene.
    //for(int i = 0; i < unknownIndexs.Count;i++)
    //{
    //    int col = unknownIndexs[i];
    //    int maxValue = 300;
    //    for(int row = 0;row < matrix.GetLength(0); row++)
    //    {
    //        if (matrix[row, col] != 0)
    //            maxValue = Math.Min(maxValue, matrix[row, matrix.GetLength(1) - 1] < 0 ? maxValue : matrix[row, matrix.GetLength(1) - 1]); // Dette kan være feil! 
    //    }
    //    maxValues[i] = maxValue;
    //}

    Array.Fill(maxValues, 200);

    // Find min values for unknowns
    int[] minValues = new int[unknownIndexs.Count];


    int minSolution = int.MaxValue;

    int[] presses = new int[matrix.GetLength(1)-1];

    int[] currentUnknowns = new int[unknownIndexs.Count];
    int permutations = maxValues.Aggregate(1, (acc, next) => acc * (next+1));
    for (int i = 0; i < permutations; i++)
    {
        // Do calc
        Array.Fill(presses, 0);
        // Insert guest unknowns
        for(int uIdx = 0; uIdx < unknownIndexs.Count; uIdx++)
        {
            presses[unknownIndexs[uIdx]] = currentUnknowns[uIdx];
        }

        bool validSolution = true;
        int rows = Math.Min(matrix.GetLength(0), matrix.GetLength(1));
        for (int row = rows - 1; row >= 0; row--)
        {
            if (matrix[row, row] == 0)
                continue;
            // Calculate new value, trekker fra kjente verdier
            int currPress = matrix[row, matrix.GetLength(1) - 1];
            for(int col = matrix.GetLength(1) - 2; col > row; col--)
                currPress -= presses[col] * matrix[row, col];

            // resultatet må være delig
            if (currPress % matrix[row, row] != 0)
            { 
                validSolution = false;
                break;
            }

            presses[row] = currPress / matrix[row, row];
            
            // Må ha et positivt antall press.
            if(presses[row] < 0)
            {
                validSolution = false;
                break;
            }
        }


        // next permutation
        currentUnknowns[0]++;
        for (int j = 0; j < currentUnknowns.Length; j++)
        {
            if (currentUnknowns[j] > maxValues[j])
            {
                currentUnknowns[j] = 0;
                if (j < currentUnknowns.Length - 1)
                    currentUnknowns[j + 1]++;
            }
        }
        ;
        if (!validSolution)
            continue;

        // For return value
        int sum = presses.Sum();
        minSolution = Math.Min(sum, minSolution);
    }

    return minSolution;
}

static async Task<int> FindFewestPresses(IndicatorLights lights)
{
    // do a breath first search, need to keep track of wich values reached
    var reachedLights = new List<int>();
    Queue<(int, int)> toVisit = new();    
    // We start with 0,
    toVisit.Enqueue((0, 0));

    while(toVisit.Count > 0)
    {
        var (currentLights, presses) = toVisit.Dequeue();
        if(currentLights == lights.TargetLights)
            return presses;

        

        foreach(var button in lights.Buttons)
        {
            int newLights = currentLights ^ button;
            if(!reachedLights.Contains(newLights))
            {
                reachedLights.Add(newLights);
                toVisit.Enqueue((newLights, presses + 1));
            }
        }
    }
    // Should never happen and returning negative value could go unnoticed, so throw an exception instead
    throw new InvalidOperationException("No path found to target lights");
}

static int Presses(int[] joltage, int[] targetJoltage, int[][] buttons, int button)
{
    if (joltage.SequenceEqual(targetJoltage))
        return 0;
    if (button == buttons.Length)
        return -1;

    // Find how many presses we can do
    int maxPresses = int.MaxValue;
    foreach (int light in buttons[button])
        maxPresses = Math.Min(maxPresses, targetJoltage[light] - joltage[light]);

    // Add joltage
    foreach (int light in buttons[button])
    {
        joltage[light] += maxPresses;
    }

    if (joltage.SequenceEqual(targetJoltage))
    {
        foreach (int light in buttons[button])
        {
            joltage[light] -= maxPresses;
        }
        return maxPresses;
    }

    // Figure out minPresses
    int minPresses = 0;
    // For any of the lights the button toggles, if there are no buttons below that toggles
    // the same light, we need to press the button at least targetJoltage[light] - joltage[light] times,
    // otherwise we will never reach the target joltage for that light.
    foreach (var light in buttons[button])
    {
        bool notFound = false;
        for (int i = button + 1; i < buttons.Length; i++)
        {
            if (buttons[i].Contains(light))
            {
                notFound = false;
                break;
            }
            notFound = true;
        }
        if (notFound)
            minPresses = Math.Max(minPresses, targetJoltage[light] - joltage[light]);
    }
    if (minPresses > maxPresses || minPresses < 0)
    {
        foreach (int light in buttons[button])
        {
            joltage[light] -= maxPresses;
        }
        return -1;
    }


    // For each presss, we want to check when the button is not pressed.
    int minimumPresses = int.MaxValue;
    for (int press = maxPresses; press >= minPresses; press--)
    {
        int totalPresses = Presses(joltage, targetJoltage, buttons, button + 1);
        // if totalPresses is 1, cant be better -> subtract joltage and return 1 + presses
        if (totalPresses == 1)
        {
            foreach (int light in buttons[button])
                joltage[light] -= press;
            return 1 + press;
        }
        if(totalPresses >= 0 && totalPresses + press < minimumPresses)
        {
            minimumPresses = totalPresses + press;
        }
        if (press > minPresses)
            foreach (int light in buttons[button])
                joltage[light]--;
    }
    foreach (int light in buttons[button])
        joltage[light] -= minPresses;

    if(minimumPresses == int.MaxValue)
        return -1;
    return minimumPresses;
}

static void AddRowInt(int[,] matrix, int row, int targetRow, int multiplier = 1)
{
    if (row >= matrix.GetLength(0) || row < 0)
        throw new ArgumentOutOfRangeException(nameof(row));
    if (targetRow >= matrix.GetLength(0) || targetRow < 0)
        throw new ArgumentOutOfRangeException(nameof(targetRow));

    for (int i = 0; i < matrix.GetLength(1); i++)
        matrix[targetRow, i] += multiplier * matrix[row, i];
}

static void DivideRowInt(int[,] matrix, int row, int multiplier)
{
    if (row >= matrix.GetLength(0) || row < 0)
        throw new ArgumentOutOfRangeException(nameof(row));

    for (int i = 0; i < matrix.GetLength(1); i++)
        matrix[row, i] /= multiplier;
}


static Fraction[,] CreateMatrix(IndicatorLights indicators)
{
    Fraction[,] matrix = new Fraction[indicators.TargetJoltage.Length, indicators.ButtonsArray.Length + 1];

    for (int i = 0; i < indicators.TargetJoltage.Length; i++)
    {
        matrix[i, matrix.GetLength(1) - 1] = new(indicators.TargetJoltage[i],1);
    }

    for (int button = 0; button < indicators.ButtonsArray.Length; button++)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
            matrix[i, button] = new(0, 1);
        foreach (var light in indicators.ButtonsArray[button])
            matrix[light, button] = new Fraction(1,1);
    }

    return matrix;
}

static void SwapRows(Fraction[,] matrix, int row1, int row2)
{
    Fraction[] temp = new Fraction[matrix.GetLength(1)];

    if (row1 >= matrix.GetLength(0) || row1 < 0)
        throw new ArgumentOutOfRangeException(nameof(row1));
    if (row2 >= matrix.GetLength(0) || row2 < 0)
        throw new ArgumentOutOfRangeException(nameof(row2));

    for (int i = 0; i < temp.Length; i++)
        temp[i] = matrix[row1, i];
    for (int i = 0; i < temp.Length; i++)
        matrix[row1, i] = matrix[row2, i];
    for (int i = 0; i < temp.Length; i++)
        matrix[row2, i] = temp[i];
}

static void AddRow(Fraction[,] matrix, int row, int targetRow, Fraction multiplier)
{
    if (row >= matrix.GetLength(0) || row < 0)
        throw new ArgumentOutOfRangeException(nameof(row));
    if (targetRow >= matrix.GetLength(0) || targetRow < 0)
        throw new ArgumentOutOfRangeException(nameof(targetRow));

    for (int i = 0; i < matrix.GetLength(1); i++)
        matrix[targetRow, i] += multiplier * matrix[row, i];
}

static void MultiplyRow(Fraction[,] matrix, int row, Fraction multiplier)
{
    if (row >= matrix.GetLength(0) || row < 0)
        throw new ArgumentOutOfRangeException(nameof(row));

    for (int i = 0; i < matrix.GetLength(1); i++)
        matrix[row, i] *= multiplier;
}

static void DivideRow(Fraction[,] matrix, int row, Fraction multiplier)
{
    if (row >= matrix.GetLength(0) || row < 0)
        throw new ArgumentOutOfRangeException(nameof(row));

    for (int i = 0; i < matrix.GetLength(1); i++)
        matrix[row, i] /= multiplier;
}


static void SwapColumns(Fraction[,] matrix, int col1, int col2)
{
    Fraction[] temp = new Fraction[matrix.GetLength(0)];

    if (col1 >= matrix.GetLength(1) - 1 || col1 < 0)
        throw new ArgumentOutOfRangeException(nameof(col1));
    if (col2 >= matrix.GetLength(1) - 1 || col2 < 0)
        throw new ArgumentOutOfRangeException(nameof(col2));

    for (int i = 0; i < temp.Length; i++)
        temp[i] = matrix[i, col2];
    for (int i = 0; i < temp.Length; i++)
        matrix[i, col2] = matrix[i, col1];
    for (int i = 0; i < temp.Length; i++)
        matrix[i, col1] = temp[i];
}

static int FindLargestDeterminator(Fraction[,] matrix, int row)
{
    int largest = 0;

    if (row >= matrix.GetLength(0) || row < 0)
        throw new ArgumentOutOfRangeException(nameof(row));
    for (int i = 0; i < matrix.GetLength(1); i++)
        largest = Math.Max(largest, matrix[row, i].Denominator);
    return largest;
}

static int[,] ConvertToIntegerMatrix(Fraction[,] matrix)
{
    // For each row
    for(int row = 0; row < matrix.GetLength(0); row++)
    {
        int largest = 0;
        do
        {
            largest = FindLargestDeterminator(matrix, row);
            MultiplyRow(matrix, row, new(largest, 1));
        } while (largest != 1);
    }

    int[,] intMatrix = new int[matrix.GetLength(0),matrix.GetLength(1)];
    for (int i = 0; i < matrix.GetLength(0); i++)
        for (int j = 0; j < matrix.GetLength(1); j++)
            intMatrix[i, j] = matrix[i, j].Numerator;

    return intMatrix;
}

static void SwapButtons(IndicatorLights indicators, int buttonA, int buttonB )
{
    int[] temp = indicators.ButtonsArray[buttonA];
    indicators.ButtonsArray[buttonA] = indicators.ButtonsArray[buttonB];
    indicators.ButtonsArray[buttonB] = temp;
}

static void GaussElimination(IndicatorLights inducators,Fraction[,] matrix)
{
    int rows = matrix.GetLength(0);
    int cols = matrix.GetLength(1);
    for (int row = 0; row < rows && row < cols - 1; row++)
    {
        // Gets rows that got value in 
        List<int> currentRows = [];
        for (int i = row; i < rows; i++)
            if (matrix[i, row].Numerator != 0)
                currentRows.Add(i);

        if (currentRows.Count == 0)
        {
            // We need to swap a columns
            int swapTo = row + 1;
            while (swapTo < cols - 1 && matrix[row, swapTo].Numerator == 0)
                swapTo++;
            if (swapTo == cols - 1)
                continue;
            SwapColumns(matrix, row, swapTo);
            SwapButtons(inducators, row, swapTo);
            for (int i = row; i < rows; i++)
                if (matrix[i, row].Numerator != 0)
                    currentRows.Add(i);
        }

        // Here we can check for conditions for selecting the row to move
        int keepRow = 0;

        // We want a row with 1 in the col, then prioritize based on number of lights affected, as few as possible
        int maxPresses = int.MaxValue;
        foreach (int r in currentRows)
        {
            int sumPresses = 0;
            for (int lights = 0; lights < cols - 1; lights++)
                if (matrix[r, lights].Numerator != 0)
                    sumPresses++;
            if (sumPresses < maxPresses)
            {
                keepRow = r;
                maxPresses = sumPresses;
            }
        }
        if (row != keepRow)
        {
            SwapRows(matrix, row, keepRow);
            keepRow = row;
        }

        // Make matrix[row,row] == 1
        DivideRow(matrix, row, matrix[row, row]);


        // top is discovered and moved
        // Make all other rows 0 in col.
        currentRows.Remove(keepRow);

        Fraction negative = new(-1, 1);

        foreach (int r in currentRows)
            AddRow(matrix, row, r, negative * matrix[r, row]);
    }
}

static int ReduceKnownValues(int[,] matrix)
{
    int presses = 0;
    bool reduce = true;
    while(reduce)
    {
        reduce = false;
        for(int row = 0; row < matrix.GetLength(0); row++)
        {
            bool justOne = false;
            int index = 0;
            for(int col = 0; col < matrix.GetLength(1) - 1; col++)
            {
                if (matrix[row,col] != 0)
                {
                    if (justOne)
                    {
                        justOne = false;
                        break;
                    }
                    justOne = true;
                    index = col;
                }
            }
            if(justOne)
            {// We have a known value of button 'index'
                reduce = true; // this will require a rerun to discover changes done to the matrix

                // Make it 1 = 
                DivideRowInt(matrix, row, matrix[row, index]);
                
                presses += matrix[row,matrix.GetLength(1)-1] / matrix[row, index];

                // Remove from matrix
                // For every non zero in matrix[i,index] -> add line with right multiplier.
                for(int r = 0; r < matrix.GetLength(0); r++)
                {
                    if (matrix[r,index] != 0)
                    {
                        AddRowInt(matrix, row, r, -1 * matrix[r,index]);
                    }
                }
            }
        }
    }

    return presses;
}

public readonly struct Fraction
{
    public int Numerator { get; }
    public int Denominator { get; }

    public Fraction(int numerator, int denominator)
    {
        if (denominator == 0)
        {
            throw new ArgumentException("Denominator can't be zero.");
        }

        // Håndter negative fortegn slik at minusen alltid havner i telleren
        if (denominator < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }

        // Finn største felles divisor for å forkorte brøken
        int gcd = FindGcd(Math.Abs(numerator), denominator);

        Numerator = numerator / gcd;
        Denominator = denominator / gcd;
    }

    // Euklids algoritme for å finne største felles divisor
    private static int FindGcd(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
    public readonly override string ToString() => $"{Numerator}/{Denominator}";

    // Overload addisjonsoperatøren (+) for å plusse sammen to brøker
    public static Fraction operator +(Fraction b1, Fraction b2)
    {
        // Formel: (t1 * n2 + t2 * n1) / (n1 * n2)
        int nyTeller = (b1.Numerator * b2.Denominator) + (b2.Numerator * b1.Denominator);
        int nyNevner = b1.Denominator * b2.Denominator;
        return new Fraction(nyTeller, nyNevner);
    }

    // Overload multiplikasjonsoperatøren (*) for å gange sammen to brøker
    public static Fraction operator *(Fraction b1, Fraction b2)
    {
        // Formel: (t1 * t2) / (n1 * n2)
        return new Fraction(b1.Numerator * b2.Numerator, b1.Denominator * b2.Denominator);
    }

    public static Fraction operator /(Fraction b1, Fraction b2)
    {
        // Formel: (t1 * t2) / (n1 * n2)
        return new Fraction(b1.Numerator * b2.Denominator, b1.Denominator * b2.Numerator);
    }
}




/*
 static async Task<int> FindFewestPressesForJoltageLP(IndicatorLights lights) 
{
    // Two-phase simplex implementation to minimize sum of button presses
    int n = lights.ButtonsArray.Length; // number of original variables (buttons)
    int m = lights.TargetJoltage.Length; // number of constraints (lights)

    // Tableau dimensions: rows = m + 1 (objective + constraints)
    // cols = n (original vars) + m (artificials) + 1 (RHS)
    int cols = n + m + 1;
    double[,] tab = new double[m + 1, cols];

    // Fill constraint rows (rows 1..m)
    for (int j = 0; j < n; j++)
    {
        foreach (var light in lights.ButtonsArray[j])
        {
            // light is 0-based index into constraints; constraint rows start at 1
            tab[1 + light, j] = 1.0;
        }
    }

    // Add artificial variables (one per constraint)
    for (int i = 0; i < m; i++)
        tab[1 + i, n + i] = 1.0;

    // RHS (target joltage)
    for (int i = 0; i < m; i++)
        tab[1 + i, cols - 1] = lights.TargetJoltage[i];

    // Phase 1 objective: minimize sum of artificials.
    // When artificials are basic, the initial objective row (reduced costs)
    // is computed as: row0[j] = objCoeff[j] - sum_{basic rows r} objCoeff[basic_r] * row_r[j]
    // For phase1 objCoeff = 0 for original vars and 1 for artificials. With artificials in basis
    // this reduces to row0[j] = - sum_{i=1..m} row_i[j] for all columns except RHS which becomes 0.
    for (int j = 0; j < cols - 1; j++)
    {
        double sum = 0.0;
        for (int i = 1; i <= m; i++) sum += tab[i, j];
        tab[0, j] = -sum;
    }
    tab[0, cols - 1] = 0.0;

    // Basis: initially the artificial variables are basic
    int[] basis = new int[m];
    for (int i = 0; i < m; i++) basis[i] = n + i;

    const double eps = 1e-9;

    // Phase 1 simplex loop (minimize sum of artificials)
    while (true)
    {
        // pick entering column (most negative reduced cost)
        int enter = -1;
        double mostNeg = -eps;
        for (int j = 0; j < cols - 1; j++)
        {
            if (tab[0, j] < mostNeg)
            {
                mostNeg = tab[0, j];
                enter = j;
            }
        }
        if (enter == -1) break; // optimal for phase 1

        // pick leaving row by minimum positive ratio
        int leaveRow = -1;
        double minRatio = double.PositiveInfinity;
        for (int i = 1; i <= m; i++)
        {
            double a = tab[i, enter];
            if (a > eps)
            {
                double ratio = tab[i, cols - 1] / a;
                if (ratio < minRatio - eps)
                {
                    minRatio = ratio;
                    leaveRow = i;
                }
            }
        }
        if (leaveRow == -1) throw new InvalidOperationException("LP is unbounded during Phase 1");

        // pivot
        double pivot = tab[leaveRow, enter];
        for (int j = 0; j < cols; j++) tab[leaveRow, j] /= pivot;
        for (int i = 0; i <= m; i++)
        {
            if (i == leaveRow) continue;
            double factor = tab[i, enter];
            if (Math.Abs(factor) <= eps) continue;
            for (int j = 0; j < cols; j++) tab[i, j] -= factor * tab[leaveRow, j];
        }
        basis[leaveRow - 1] = enter;
    }

    // Check feasibility: objective value should be zero (all artificials driven out)
    if (tab[0, cols - 1] > eps) throw new InvalidOperationException("No feasible solution for joltage constraints");

    // Phase 2: minimize sum of original variables x_j (j=0..n-1)
    double[] objCoeff = new double[cols - 1];
    for (int j = 0; j < n; j++) objCoeff[j] = 1.0; // minimize sum x_j
    for (int j = n; j < cols - 1; j++) objCoeff[j] = 0.0; // artificials have zero cost in phase2

    // Recompute reduced costs for phase 2: row0 = objCoeff - sum_{basis} objCoeff[basisVar] * row
    for (int j = 0; j < cols; j++)
    {
        double sum = 0.0;
        for (int i = 0; i < m; i++)
        {
            int bv = basis[i];
            double bc = (bv < objCoeff.Length) ? objCoeff[bv] : 0.0;
            sum += bc * tab[i + 1, j];
        }
        double oc = (j < objCoeff.Length) ? objCoeff[j] : 0.0;
        tab[0, j] = oc - sum;
    }

    // Phase 2 simplex loop
    while (true)
    {
        int enter = -1;
        double mostNeg = -eps;
        // only original variables may enter
        for (int j = 0; j < n; j++)
        {
            if (tab[0, j] < mostNeg)
            {
                mostNeg = tab[0, j];
                enter = j;
            }
        }
        if (enter == -1) break; // optimal

        int leaveRow = -1;
        double minRatio = double.PositiveInfinity;
        for (int i = 1; i <= m; i++)
        {
            double a = tab[i, enter];
            if (a > eps)
            {
                double ratio = tab[i, cols - 1] / a;
                if (ratio < minRatio - eps)
                {
                    minRatio = ratio;
                    leaveRow = i;
                }
            }
        }
        if (leaveRow == -1) throw new InvalidOperationException("LP is unbounded during Phase 2");

        double pivot = tab[leaveRow, enter];
        for (int j = 0; j < cols; j++) tab[leaveRow, j] /= pivot;
        for (int i = 0; i <= m; i++)
        {
            if (i == leaveRow) continue;
            double factor = tab[i, enter];
            if (Math.Abs(factor) <= eps) continue;
            for (int j = 0; j < cols; j++) tab[i, j] -= factor * tab[leaveRow, j];
        }
        basis[leaveRow - 1] = enter;
    }

    // Extract solution for original variables and return sum (rounded to integer)
    double sumX = 0.0;
    for (int j = 0; j < n; j++)
    {
        int bi = Array.IndexOf(basis, j);
        double val = 0.0;
        if (bi >= 0) val = tab[bi + 1, cols - 1];
        sumX += val;
    }

    int result = (int)Math.Round(sumX);
    return await Task.FromResult(result);
}
 
 
 */


//static async Task<int> FindFewestPressesForJoltageLP(IndicatorLights lights) 
//{
//    int buttonsNr = lights.ButtonsArray.Length;
//    int lightsNr = lights.TargetJoltage.Length;
//    int rows = lightsNr + 1;
//    int cols = buttonsNr + lightsNr + 1;

//    int[,] lpMatrix = new int[rows,cols];

//    // Insert buttons
//    for(int i = 0; i < lights.ButtonsArray.Length; i++)
//    {
//        foreach (var light in lights.ButtonsArray[i])
//            lpMatrix[light + 1, i] = 1;
//    }

//    // Add target joltage
//    for(int i=1; i < rows; i++)
//    {
//        lpMatrix[i, cols - 1] = lights.TargetJoltage[i-1];
//    }

//    // create W
//    for(int i = 0; i< cols; i++)
//    {
//        for (int j = 1; j < rows; j++)
//            lpMatrix[0, i] += lpMatrix[j, i];
//    }

//    // Add artificial variables
//    for(int i = 0; i < lightsNr; i++)
//        lpMatrix[i+1,buttonsNr+i] = 1;


//    // Print matrix
//    //for(int i = 0; i < rows; i++)
//    //{
//    //    for (int j = 0; j < cols; j++)
//    //    {
//    //        Console.Write(lpMatrix[i, j] + " ");
//    //    }
//    //    Console.WriteLine();
//    //}

//    // Solve LP using simplex algorithm
//    // 
//    while (lpMatrix[0, cols - 1] != 0)
//    {
//        // Find lowes index of max coefficient in W
//        int maxIndex = 0;
//        int maxValue = int.MinValue;
//        for (int i = 0; i < buttonsNr; i++) // Finds max
//            maxValue = Math.Max(maxValue, lpMatrix[0, i]);
//        for (; lpMatrix[0,maxIndex] != maxValue; maxIndex++) ; // Finds index

//        // We have the column, lets find the row with the smallest positive RHS to the coefficient in the column
//        int minRHS = int.MaxValue;
//        int minRHSIndex = 0;
//        for (int i = 1; i < rows; i++)
//        {
//            if (lpMatrix[i, maxIndex] == 0)
//                continue;
//            //if (lpMatrix[i, cols - 1] >= 0 && lpMatrix[i, cols - 1] < minRHS)
//            //{
//                minRHS = lpMatrix[i, cols - 1];
//                minRHSIndex = i;
//            //}
//            break;
//        }

//        int pivotCol = maxIndex;
//        int rowToKeep = minRHSIndex;

//        // Get a list of all rows that need to be made 0 in the pivot column.
//        List<int> rowsToSubtract = [];
//        for(int i = 0; i < rows; i++)
//        {
//            if(i == rowToKeep)
//                continue;
//            if (lpMatrix[i,pivotCol] != 0)
//                rowsToSubtract.Add(i);
//        }

//        // For each row in list we need to add values such that lpMatrix[row,pivotCol] becomes 0.
//        // The same value is added to each item in the row multiplied by lpMatrix[rowToKeep,item_col] > 0
//        // multiplied by
//        foreach(var row in rowsToSubtract)
//        {
//            int valueToAdd = lpMatrix[row, pivotCol] * lpMatrix[rowToKeep, pivotCol];

//            // We need to add valueToAdd * lpMatrix[rowToKeep, item_col] to each item in the row.
//            for(int i = 0;i < cols; i++)
//            {
//                lpMatrix[row, i] -= valueToAdd * lpMatrix[rowToKeep, i];
//            }
//        }

//        //Console.WriteLine(lpMatrix[0, cols - 1]);
//    }

//    int sum = 0;
//    for(int i = 1; i < rows; i++)
//    {
//        sum += lpMatrix[i, cols - 1];
//    }
//    return sum;
//}

//static async Task<int> FindFewestPressesForJoltage(IndicatorLights lights)
//{
//    int[] joltage = new int[lights.TargetJoltage.Length];
//    var buttonsSorted = lights.ButtonsArray.OrderByDescending(lightList => lightList.Length).ToArray();
//    return Presses(joltage, lights.TargetJoltage, buttonsSorted, 0);
//}
//static async Task<int> FindFewestPressesForJoltage(IndicatorLights lights)
//{
//    // this is matrix with each row representing a button and each column representing a light, the value is 1
//    // if the button toggles the light, 0 otherwise
//    int[,] buttons = new int[lights.ButtonsArray.Length, lights.TargetJoltage.Length];

//    // populate buttons matrix, we sort based on how many lights each button toggles, so we can try the buttons that
//    // toggle more lights first, to hopefully reach the target faster
//    var buttonsSorted = lights.ButtonsArray.OrderByDescending(lightList => lightList.Length).ToArray();
//    for (int i = 0; i < buttonsSorted.Length; i++)
//        for (int j = 0; j < buttonsSorted[i].Length; j++)
//            buttons[i, buttonsSorted[i][j]] = 1;


//    int[] target = lights.TargetJoltage;
//    int[] current = new int[target.Length];
//    int[] presses = new int[buttons.GetLength(0)];

//    // set max presses
//    int[] maxPresses = new int[buttons.GetLength(0)];
//    for(int i = 0; i < maxPresses.Length; i++)
//    {
//        // For each button, the max presses is the minimum of the target joltage of the lights it toggles,
//        // because pressing it more than that would add to much joltage.
//        maxPresses[i] = target.Max();
//        for(int j = 0; j < buttons.GetLength(1); j++)
//        {
//            if (buttons[i, j] == 1)
//                maxPresses[i] = Math.Min(maxPresses[i], target[j]);
//        }
//    }


//    // Finding the fewest presses is a combinatorial problem, we can solve it with a
//    // backtracking algorithm, we try all combinations of button presses and keep
//    // track of the minimum number of presses that reaches the target joltage.

//    // We can optimize the backtracking by using the maxPresses array to limit the
//    // number of presses for each button, and by trying the buttons that toggle more lights first,
//    // to hopefully reach the target faster. This is why we sorted the buttons array.

//    // We start by pressing buttons[0] max times. Then next button max times, and so on, until we reach
//    // the target joltage or exceed the max presses for all buttons.


//    // Iterate all combinations of presses using a mixed-radix odometer.
//    // Start from all zeros and increment until all combinations are exhausted.
//    Array.Clear(presses, 0, presses.Length);
//    int currentButton = 0;
//    int[] currentJoltage = new int[buttons.GetLength(1)];
//    int[] currentMaxPresses = new int[maxPresses.Length];
//    currentMaxPresses[currentButton] = maxPresses[currentButton];

//    while (true)
//    {
//        // Calculate current joltage based on presses and buttons matrix.
//        for (int i = 0; i < presses.Length; i++)
//        {
//            for (int j = 0; j < buttons.GetLength(1); j++)
//            {
//                if (buttons[i, j] == 1)
//                {
//                    current[j] += presses[i];
//                }
//            }
//        }

//        if (current.SequenceEqual(target))
//            return presses.Sum();

//        // We are at the end of the tree but no match, reduce max presses for the largest one, we have exhasted that search tree.
//        if(currentButton == (maxPresses.Length -1))
//        {
//            currentButton = 0;
//            while (maxPresses[currentButton] == 0) currentButton++;
//            maxPresses[currentButton]--;
//        }

//        currentButton++;
//        // We cant use 0 as a starter when comparing for min as 0 is a possible value
//        // It can never be higher than this.
//        currentMaxPresses[currentButton] = maxPresses[currentButton];
//        // For each button above
//        for(int buttonAbove = currentButton - 1; buttonAbove >= 0; buttonAbove--)
//        {
//            // For each light, check if both buttons affect same light and for all find the smallest number we can press the button.
//            for(int light = 0;  light < buttons.GetLength(1); light++)
//            {
//                // Both buttons affect same light, we need to calculate the max number the current button can be pressed.
//                if (buttons[currentButton, light] == 1 && buttons[buttonAbove, light] == 1)
//                    currentMaxPresses[currentButton] = Math.Min(currentMaxPresses[currentButton], target[light] - current[light]);
//            }
//        }

//    }

//    throw new InvalidOperationException("No combination of presses reaches the target joltage");
//    // AI is usefull for commenting code.
//}