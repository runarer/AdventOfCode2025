/*
    Den mest optimale måten å løse del 2 på er med LP, dette kan jeg ikke så en bruteforce metode er brukt.
    
 */

using Day10;
using System.Data;

int fewestPresses = 0;
int fewestPressesForJoltage = 0;

// Read all lines first so we can show a progress indicator with a known total.
var allLines = await File.ReadAllLinesAsync(args[0]);
int total = allLines.Length;
var processingTasks = new List<Task<(int presses, int joltagePresses)>>();
int completed = 0;

for (int i = 0; i < total; i++)
{
    var line = allLines[i];
    var indicatorLights = Parser.CreateIndicatorLights(line);
    //fewestPressesForJoltage += await FindFewestPressesForJoltage(indicatorLights);
    //start processing this line on the thread-pool and capture the task
    //Console.WriteLine();
    //int ff = FindFewestPressesForJoltageLP(indicatorLights).Result;
    //Console.WriteLine();

    processingTasks.Add(Task.Run(async () =>
   {
       var p1 = await FindFewestPresses(indicatorLights);
       var p2 = await FindFewestPressesForJoltageLP(indicatorLights);
       var done = Interlocked.Increment(ref completed);
       Console.Write($"\rProcessed {done}/{total} ({done * 100 / Math.Max(1, total)}%)");
       return (p1, p2);
   }));
}

if (processingTasks.Count > 0)
{
    var results = await Task.WhenAll(processingTasks);
    // ensure progress line ends and moves to next line
    Console.WriteLine();
    fewestPresses = results.Sum(r => r.presses);
    fewestPressesForJoltage = results.Sum(r => r.joltagePresses);
}

Console.WriteLine($"Part 1: {fewestPresses}");
Console.WriteLine($"Part 2: {fewestPressesForJoltage}");

return 0;


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

static async Task<int> FindFewestPressesForJoltageLP(IndicatorLights lights) 
{
    int buttonsNr = lights.ButtonsArray.Length;
    int lightsNr = lights.TargetJoltage.Length;
    int rows = lightsNr + 1;
    int cols = buttonsNr + lightsNr + 1;

    int[,] lpMatrix = new int[rows,cols];
    
    // Insert buttons
    for(int i = 0; i < lights.ButtonsArray.Length; i++)
    {
        foreach (var light in lights.ButtonsArray[i])
            lpMatrix[light + 1, i] = 1;
    }

    // Add target joltage
    for(int i=1; i < rows; i++)
    {
        lpMatrix[i, cols - 1] = lights.TargetJoltage[i-1];
    }

    // create W
    for(int i = 0; i< cols; i++)
    {
        for (int j = 1; j < rows; j++)
            lpMatrix[0, i] += lpMatrix[j, i];
    }

    // Add artificial variables
    for(int i = 0; i < lightsNr; i++)
        lpMatrix[i+1,buttonsNr+i] = 1;


    // Print matrix
    //for(int i = 0; i < rows; i++)
    //{
    //    for (int j = 0; j < cols; j++)
    //    {
    //        Console.Write(lpMatrix[i, j] + " ");
    //    }
    //    Console.WriteLine();
    //}

    // Solve LP using simplex algorithm
    // 
    while (lpMatrix[0, cols - 1] != 0)
    {
        // Find lowes index of max coefficient in W
        int maxIndex = 0;
        int maxValue = int.MinValue;
        for (int i = 0; i < buttonsNr; i++) // Finds max
            maxValue = Math.Max(maxValue, lpMatrix[0, i]);
        for (; lpMatrix[0,maxIndex] != maxValue; maxIndex++) ; // Finds index

        // We have the column, lets find the row with the smallest positive RHS to the coefficient in the column
        int minRHS = int.MaxValue;
        int minRHSIndex = 0;
        for (int i = 1; i < rows; i++)
        {
            if (lpMatrix[i, maxIndex] == 0)
                continue;
            if (lpMatrix[i, cols - 1] >= 0 && lpMatrix[i, cols - 1] < minRHS)
            {
                minRHS = lpMatrix[i, cols - 1];
                minRHSIndex = i;
            }
        }

        int pivotCol = maxIndex;
        int rowToKeep = minRHSIndex;

        // Get a list of all rows that need to be made 0 in the pivot column.
        List<int> rowsToSubtract = [];
        for(int i = 0; i < rows; i++)
        {
            if(i == rowToKeep)
                continue;
            if (lpMatrix[i,pivotCol] != 0)
                rowsToSubtract.Add(i);
        }

        // For each row in list we need to add values such that lpMatrix[row,pivotCol] becomes 0.
        // The same value is added to each item in the row multiplied by lpMatrix[rowToKeep,item_col] > 0
        // multiplied by
        foreach(var row in rowsToSubtract)
        {
            int valueToAdd = lpMatrix[row, pivotCol] * lpMatrix[rowToKeep, pivotCol];

            // We need to add valueToAdd * lpMatrix[rowToKeep, item_col] to each item in the row.
            for(int i = 0;i < cols; i++)
            {
                lpMatrix[row, i] -= valueToAdd * lpMatrix[rowToKeep, i];
            }
        }

        Console.WriteLine(lpMatrix[0, cols - 1]);
    }

    int sum = 0;
    for(int i = 1; i < rows; i++)
    {
        sum += lpMatrix[i, cols - 1];
    }
    return sum;
}

//static async Task<int> FindFewestPressesForJoltage(IndicatorLights lights)
//{
//    int[] joltage = new int[lights.TargetJoltage.Length];
//    var buttonsSorted = lights.ButtonsArray.OrderByDescending(lightList => lightList.Length).ToArray();
//    return Presses(joltage,lights.TargetJoltage, buttonsSorted, 0);
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

static int Presses(int[] joltage, int[] targetJoltage, int[][] buttons, int button)
{
    if (joltage.SequenceEqual(targetJoltage))
        return 0;
    if (button == buttons.Length)
        return -1;

    // Find how many presses we can do
    int maxPressses = int.MaxValue;
    foreach (int light in buttons[button])
        maxPressses = Math.Min(maxPressses, targetJoltage[light] - joltage[light]);

    // Add joltage
    foreach (int light in buttons[button])
    {
        joltage[light] += maxPressses;
    }

    if (joltage.SequenceEqual(targetJoltage))
    {
        foreach (int light in buttons[button])
        {
            joltage[light] -= maxPressses;
        }
        return maxPressses;
    }

    // Figure out minPresses
    int minPresses = 0;
    // For any of the lights the button toggles, if there are no buttons below that toggles
    // the same light, we need to press the button at least targetJoltage[light] - joltage[light] times,
    // otherwise we will never reach the target joltage for that light.


    // For each presss, we want to check when the button is not pressed.
    List<int> presses = [];
    //int totalPresses = 0;
    for (int press = maxPressses; press >= minPresses; press--)
    {
        int totalPresses = Presses(joltage, targetJoltage, buttons, button + 1);
        // if totalPresses is 1, cant be better -> subtract joltage and return 1 + presses
        if (totalPresses == 1)
        {
            foreach (int light in buttons[button])
                joltage[light] -= press;
            return 1 + press;
        }
        if (totalPresses >= 0)
        {
            presses.Add(totalPresses + press);
        }
        if (press > minPresses)
            foreach (int light in buttons[button])
                joltage[light]--;
    }
    foreach (int light in buttons[button])
        joltage[light] -= minPresses;

    if (presses.Count < 1)
        return -1;
    return presses.Min();
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