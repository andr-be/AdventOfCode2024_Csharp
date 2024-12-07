using AdventOfCode2024.Utilities;

namespace AdventOfCode2024.Day06;

internal class GuardGallivant : SolutionBase
{
    public override string Name => "Day 06: Guard Gallivant";

    public override int Day => 6;

    public override string Solution(Part part, string input)
    {
        CoordinateArray array = new(input);

        //Console.WriteLine(array.Print(200, 200));

        Coordinate start = array.Array
            .ToEnumerable<Coordinate>()
            .First(c => c.C == '^');

        //Console.WriteLine("Start: " + start);

        Guard guard = new(array.CloneParallel(), start);

        while (guard.CurrentPosition is not null)
            guard.Traverse();

        if (part is Part.One)
        {
            //Console.WriteLine(guard.Map.Print());
            return guard.VisitedCount.ToString();
        }

        var loopFinder = new ObstacleLoopFinder(start, guard);
        var result = loopFinder.FindLoops();

        return result.ToString();
    }
}

public enum Direction
{
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3,
}

public class Guard
{
    public CoordinateArray Map { get; init; }

    public Direction Facing { get; set; }

    public Coordinate? CurrentPosition { get; private set; }

    public HashSet<Coordinate> Visited { get; private set; } = [];

    private readonly static Dictionary<Direction, (int row, int col)> TravelVectors = new()
    {
        { Direction.Up,    (-1, +0) },
        { Direction.Right, (+0, +1) },
        { Direction.Down,  (+1, +0) },
        { Direction.Left,  (+0, -1) },
    };

    public Guard(CoordinateArray map, Coordinate start)
    {
        Map = map;
        CurrentPosition = start;
        Facing = Direction.Up;
        Map.Array[start.X, start.Y] = start with { C = 'X' };
        Visited.Add(CurrentPosition with { C = 'X' });
    }

    public void Traverse()
    {
        var newX = CurrentPosition.X + TravelVectors[Facing].col;
        var newY = CurrentPosition.Y + TravelVectors[Facing].row;
        var targetPosition = Map.GetPoint(newX, newY)?.C;

        if (targetPosition is '#')
        {
            ChangeDirection();
        }

        else if (Map.WithinArray(newX, newY))
        {
            MoveToNewPosition(newX, newY, targetPosition);
        }

        else
        {
            CurrentPosition = null;
        }
    }

    private void MoveToNewPosition(int newX, int newY, char? newC)
    {
        CurrentPosition = Map.GetPoint(newX, newY)!;    // not null because we just checked
        if (newC is not '#' or 'X')
        {
            var newPos = CurrentPosition with { C = 'X' };
            Map.Array[newX, newY] = newPos;
            Visited.Add(newPos);
        }
    }

    private void ChangeDirection() => 
        Facing = Facing is Direction.Left ? Direction.Up : Facing + 1;

    public int VisitedCount => 
        Visited.Count;
}

public class ObstacleLoopFinder(Coordinate start, Guard guard)
{
    public Coordinate StartPoint { get; init; } = start;

    public Guard Guard { get; init; } = guard;
 
    private HashSet<(Coordinate, Direction)> PreviousStates { get; set; } = [];

    public int FindLoops()
    {
        HashSet<CoordinateArray> parallelUniverses = [];
        HashSet<(int, int)> addedObstacles = [];
        foreach (var coordinate in Guard.Visited)
        {
            var (x, y) = (coordinate.X, coordinate.Y);

            if (BadObstaclePlacement(coordinate, x, y))
                continue;

            CreateNewParallelUniverse(parallelUniverses, addedObstacles, x, y);
        }

        int totalLoops = FindAllLoopWorlds(parallelUniverses);

        return totalLoops;
    }

    private int FindAllLoopWorlds(HashSet<CoordinateArray> parallelUniverses)
    {
        int totalLoops = 0;
        int currentPU = 1;
        foreach (var map in parallelUniverses)
        {
            const int MAX_STEPS = 10000;
            int steps = 0;
            bool isLoop = false;

            var newGuard = new Guard(map, StartPoint);
            PreviousStates = [(StartPoint with { C = 'X' }, newGuard.Facing)];
            while (steps < MAX_STEPS && isLoop is false)
            {
                newGuard.Traverse();

                if (newGuard.CurrentPosition is null) break;

                var state = (newGuard.CurrentPosition, newGuard.Facing);
                if (PreviousStates.Contains(state))
                {
                    //Console.WriteLine($"PU: {currentPU} is a loop! {steps} steps.");
                    isLoop = true;
                    break;
                }
                PreviousStates.Add(state);

                steps++;
            }

            if (isLoop)
                totalLoops++;

            currentPU++;
        }

        return totalLoops;
    }

    private void CreateNewParallelUniverse(HashSet<CoordinateArray> parallelUniverses,
                                           HashSet<(int, int)> addedObstacles,
                                           int x, int y)
    {
        if (!addedObstacles.Add((x, y)))
            return;
        
        var newMap = Guard.Map.CloneParallel();

        newMap.Array[x, y] = new Coordinate(x, y, '#');

        parallelUniverses.Add(newMap);
    }

    private bool BadObstaclePlacement(Coordinate coordinate, int x, int y) =>
        Guard.Map.WithinArray(x, y) is false || IsStartPoint(coordinate);

    private bool IsStartPoint(Coordinate coordinate) => 
        coordinate.X == StartPoint.X && coordinate.Y == StartPoint.Y;
}
