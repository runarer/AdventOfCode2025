/*
 This was way simpler than expected.
 */

using System.Text.RegularExpressions;

string[] lines = [];

try
{
    lines = File.ReadAllLines(args[0]);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    return 1;
}

List<Puzzle> unsolved = [];

foreach(string line in lines.Skip(30))
{
    unsolved.Add(ParseLine(line));
}


List<Puzzle> solved = [];
List<Puzzle> unsolvable = [];

(unsolved, solved,unsolvable) = FindObviousSolutions(unsolved, solved);
//(unsolved, solved, unsolvable) = FindSimpleSolutions(unsolved, solved, unsolvable);

Console.WriteLine($"Part 1: {unsolved.Count}");


return 0;


static (List<Puzzle>, List<Puzzle>, List<Puzzle>) FindObviousSolutions(List<Puzzle> unsolved, List<Puzzle> solved)
{
    List<Puzzle> notSolved = [];
    List<Puzzle> unsolvable = [];

    // If all pieces can be placed in a 3x3 cell
    foreach(Puzzle p in unsolved)
    {
        // find grid of 3x3 cells
        int grids = ( (p.Height / 3) * (p.Width/3) ) / 9;
        int pieces = p.PresentZero + p.PresentOne + p.PresentTwo + p.PresentThree + p.PresentFour +p.PresentFive;

        int gridTiles = p.Height * p.Width;
        int piecesTiles = 7*p.PresentZero + 5*p.PresentOne + 7*p.PresentTwo + 7*p.PresentThree + 7*p.PresentFour + 6*p.PresentFive;

        // If all pieces can be placed in its own 3x3 cell
        if (grids >= pieces) 
            solved.Add(p);
        
        // If there is not enough cells in the grid to fit all the tiles, there can not be any solutions
        else if(gridTiles < piecesTiles)
            unsolvable.Add(p);

        // Need another method
        else
            notSolved.Add(p);
    }

    return (notSolved,solved,unsolvable);
}

// Turns out this was not needed!
static (List<Puzzle>, List<Puzzle>, List<Puzzle>) FindSimpleSolutions(List<Puzzle> unsolved, List<Puzzle> solved, List<Puzzle> unsolvable)
{
    List<Puzzle> notSolved = [];

    // If all pieces can be placed in a 3x3 cell
    foreach (Puzzle p in unsolved)
    {
        int sixHeight = p.Height / 6;
        int sixWidth = p.Width / 6;
        int sixCells = sixHeight * sixWidth;

        int leftOnTop = p.Height - (sixHeight*6);
        int leftOnRight = p.Width - (sixWidth*6);

        int oneTwoFiveSquares = Math.Min(p.PresentOne / 2, p.PresentTwo / 2);
        oneTwoFiveSquares = Math.Min(oneTwoFiveSquares, p.PresentFive / 2);

        int oneTwoFiveSquaresPlaced = Math.Min(oneTwoFiveSquares,sixCells);
        sixCells = sixCells-oneTwoFiveSquaresPlaced;

        Console.WriteLine(sixCells);

        // 3x3 cells left 
        int threeByThreeCells = sixCells * 4 + ((leftOnTop / 3) * (p.Width/3) ) + ((leftOnRight/3) * (p.Height-leftOnTop));

        int presentsLeft =     p.PresentZero 
                            + (p.PresentOne - 2*oneTwoFiveSquaresPlaced) 
                            + (p.PresentTwo - 2 * oneTwoFiveSquaresPlaced)
                            +  p.PresentThree + p.PresentFour
                            + (p.PresentFive - 2 *oneTwoFiveSquaresPlaced);
        if (presentsLeft <= threeByThreeCells)
            solved.Add(p);
        else
            notSolved.Add(p);
    }

    return (notSolved, solved, unsolvable);
}



static Puzzle ParseLine(string line)
{
    string pattern = @"(\d+)x(\d+): (\d+) (\d+) (\d+) (\d+) (\d+) (\d+)";
    var result = Regex.Match(line, pattern);
    if (result.Success)
    {
        var values = result.Groups[0];

        return new Puzzle(
            int.Parse(result.Groups[1].Value),
            int.Parse(result.Groups[2].Value),
            int.Parse(result.Groups[3].Value),
            int.Parse(result.Groups[4].Value),
            int.Parse(result.Groups[5].Value),
            int.Parse(result.Groups[6].Value),
            int.Parse(result.Groups[7].Value),
            int.Parse(result.Groups[8].Value)
            );

    }
    
    throw new Exception("A line did not match regex.");
}

record Puzzle(int Height, int Width, int PresentZero, int PresentOne, int PresentTwo, int PresentThree, int PresentFour, int PresentFive);