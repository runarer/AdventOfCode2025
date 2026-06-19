// The solution uses brute force, and takes about 30sec.
// 
// For my input we have svr -> fft -> dac -> out

string[] lines;
try
{
    lines = File.ReadAllLines(args[0]);
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
    return 1;
}

var parsedLines = lines.Select(ParseLine).ToArray();
Dictionary<string, string[]> graph = parsedLines.ToDictionary(node => node.Label, node => node.List);
Console.WriteLine($"Part 1: {PathsFromAToB(graph,"you","out").Item2}");
var totalPaths = PathsFromSvrToOutPassFftAndDac(graph);
Console.WriteLine($"Part 2: {totalPaths}");


return 0;


static (string Label, string[] List) ParseLine(string line)
{
    string[] labelAndList = line.Split(':');
    return (Label: labelAndList[0], List: labelAndList[1].Trim().Split());
}

static long PathsFromSvrToOutPassFftAndDac(Dictionary<string, string[]> graph)
{
    var (visited, paths) = PathsFromAToB(graph, "dac", "out");
    foreach(var visit in visited)
        graph.Remove(visit);
    visited.Remove("dac");
    foreach (var node in graph)
        foreach (var visit in visited)
            node.Value.Replace(visit, "out");
    long totalPaths = paths;

    var (visited2, paths2) = PathsFromAToB(graph, "fft", "dac");
    foreach (var visit in visited2)
        graph.Remove(visit);
    visited.Remove("fft");
    foreach (var node in graph)
        foreach (var visit in visited2)
            node.Value.Replace(visit, "out");
    totalPaths *= paths2;

    var (visited3, paths3) = PathsFromAToB(graph, "svr", "fft");
    totalPaths *= paths3;

    return totalPaths;
}

static (HashSet<string>,long) PathsFromAToB(Dictionary<string, string[]> graph,string a, string b)
{
    long outs = 0;

    Queue<string> queue = new(graph[a]);
    HashSet<string> visited = [];
    while(queue.Count > 0)
    {
        string current = queue.Dequeue();
        visited.Add(current);
        if(current.Equals(b)) 
            outs++;
        else if (current.Equals("out"))
            continue;
        else
            foreach (var nextNode in graph[current])
                queue.Enqueue(nextNode);
    }

    return (visited,outs);
}

// This is a super fast way to find all paths from a node to "out"
// There should be a way to use this in part 2, reduce the graph and rerun for targets
static int BackwardsGraphReduction(Dictionary<string, string[]> graph,string target = "out")
{
    // Start with grabbing all nodes that just point to "out"
    var outPaths = graph
        //.Where(node => node.Value.Length == 1 && node.Value[0].Equals(target))
        .Where(node => node.Value.Contains(target))
        .ToDictionary(node => node.Key, node => 1L);

    bool added = true;

    //while(outPaths.Count != graph.Count)
    while(added)
    {
        added = false;
        foreach(var(label, list) in graph)
        {
            // allready handled
            if (outPaths.ContainsKey(label))
                continue;
            // Check if its ready to add
            if (list.All(item => outPaths.ContainsKey(item)))
            {
                long paths = 0;

                foreach (var item in list)
                {
                    paths += outPaths[item];
                }

                outPaths[label] = paths;
                added = true;
            }
        }
    }

    return 0;
}

static int PathsFromSvrToOutPassingFftAndDac(Dictionary<string, string[]> graph)
{
    static int DFSearchGraph(Dictionary<string, string[]> graph, List<string> deadEnd, string nodeLabel,bool fft, bool dac)
    {
        // At the end, do we have a match
        if(nodeLabel.Equals("out"))
        {
            if (dac && fft)
                return 1;
            else
                return 0;
        }

        if (nodeLabel.Equals("fft"))
            fft = true;
        if (nodeLabel.Equals("dac"))
            dac = true;

        // Search
        int paths = 0;
        foreach (var node in graph[nodeLabel])
            paths += DFSearchGraph(graph, deadEnd, node, fft, dac);

        if(paths == 0)
            deadEnd.Add(nodeLabel);

        // We have no way to out passed both dac and fft
        return paths;
    }

    // Need to do a depth first search and keep a record of deadends
    List<string> deadEnds = [];
    return DFSearchGraph(graph,deadEnds, "svr",false,false);
}