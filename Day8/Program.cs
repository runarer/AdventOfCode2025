using System.Numerics;


int NumbersToGrab = 1000;
string[] lines = [];

try
{
    lines = File.ReadAllLines(args[0]);
} catch(Exception ex)
{
    Console.WriteLine(ex.Message);
    return 1;
}

Vector3[] JunctionBoxes = [..lines.Select(ToVector)];

(int A, int B, float distance)[] Distances = FindAllDistances(JunctionBoxes);
(int A, int B)[] Closest = [..Distances.OrderBy(p => p.distance).Select(p => (p.A,p.B))];


// Get the number of elements in each list, get three largest and multibly them together.
List<List<int>> Circuts = CreateCircuts([.. Closest.Take(NumbersToGrab)]);
int MultiplyLargest = Circuts.Select(c => c.Count)
                             .OrderDescending()
                             .Take(3)                             
                             .Aggregate(1, (x, y) => x * y);

Console.WriteLine($"Part 1: {MultiplyLargest}");

(int A, int B) = ConnectUntil(Closest, JunctionBoxes.Length);


Console.WriteLine($"Part 2: {(int)JunctionBoxes[A].X * (int)JunctionBoxes[B].X}");

return 0;

static Vector3 ToVector(string line)
{
    float[] numbers = [..line.Split(',').Select(float.Parse)];
    return new Vector3(numbers);
}

static (int,int,float)[] FindAllDistances(Vector3[] points)
{
    (int,int,float)[] distances = new (int,int,float)[points.Length * (points.Length-1)/2];
    int index = 0;

    for(int i = 0; i < points.Length; i++)
    {
        for (int j = i+1; j < points.Length; j++)
        {
            float distance = Vector3.Distance(points[i], points[j]);
            distances[index++] = (i,j,distance);
            
        }
    }

    return distances;
}

static List<List<int>> CreateCircuts((int A,int B)[] pairsToConnect)
{
    List<List<int>> circuts = [];

    foreach((int A, int B) in pairsToConnect)
    {
        // 
        int aIsIn = -1;
        int bIsIn = -1;
        for (int i = 0; i < circuts.Count; i++)
        {
            if (circuts[i].Contains(A))
                aIsIn = i;
            if (circuts[i].Contains(B))
                bIsIn = i;

            // Check if we are done early
            if (aIsIn >= 0 && bIsIn >= 0)
                break;
        }

        // Found noone, add new circut
        if (aIsIn == -1 && bIsIn == -1)
        {
            circuts.Add([A, B]);
            continue;
        }

        // Both are in same, ignore
        if (aIsIn == bIsIn)
            continue;

        // One is in a circut, add the other one
        if(aIsIn == -1 && bIsIn >= 0)
        {
            circuts[bIsIn].Add(A);
            continue;
        }
        if (aIsIn >= 0 && bIsIn == -1)
        {
            circuts[aIsIn].Add(B);
            continue;
        }

        // Both are in different circuts, combine to one and delete other
        circuts[aIsIn].AddRange(circuts[bIsIn]);
        circuts.RemoveAt(bIsIn);
    }
    return circuts;
}

static (int, int) ConnectUntil((int, int)[] pairsToConnect, int numbers)
{
    List<List<int>> circuts = [];
    bool[] connected = new bool[numbers];
    

    foreach ((int A, int B) in pairsToConnect)
    {
        // After this block A and B will be connected
        connected[A] = true;
        connected[B] = true;

        int aIsIn = -1;
        int bIsIn = -1;
        for (int i = 0; i < circuts.Count; i++)
        {
            if (circuts[i].Contains(A))
                aIsIn = i;
            if (circuts[i].Contains(B))
                bIsIn = i;

            // Check if we are done early
            if (aIsIn >= 0 && bIsIn >= 0)
                break;
        }

        // Found noone, add new circut
        if (aIsIn == -1 && bIsIn == -1)
        {
            circuts.Add([A, B]);
            continue;
        }

        // Both are in same, ignore
        if (aIsIn == bIsIn)
            continue;

        // One is in a circut, add the other one
        if (aIsIn == -1 && bIsIn >= 0)
        {
            circuts[bIsIn].Add(A);
            //continue;
        }
        else if (aIsIn >= 0 && bIsIn == -1)
        {
            circuts[aIsIn].Add(B);
            //continue;
        }
        else
        {
            // Both are in different circuts, combine to one and delete other
            circuts[aIsIn].AddRange(circuts[bIsIn]);
            circuts.RemoveAt(bIsIn);
        }
        if(connected.All(a => a))
        {
            return (A, B);
        }
    }
    return (-1,-1);
}