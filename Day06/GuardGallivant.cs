using AdventOfCode2024.Utilities;

namespace AdventOfCode2024.Day06;

internal class GuardGallivant : SolutionBase
{
    public override string Name => "Day 06: Guard Gallivant";

    public override int Day => 6;

    public override string Solution(Part part, string input)
    {
        CoordinateArray array = new(input);
        Console.WriteLine(array.Print(200, 200));

        Coordinate start = array.Array
            .ToEnumerable<Coordinate>()
            .First(c => c.C == '^');
        Console.WriteLine("Start: " + start);

        Guard guard = new(array.Clone(), start);

        while (guard.CurrentPosition is not null)
            guard.Traverse();

        var result = guard.VisitedCount;

        Console.WriteLine(guard.Map.Print(200,200));
        
        if (part is Part.One)
            return result.ToString();

        var loopFinder = new ObstacleLoopFinder(array, start, guard.Visited);

        result = loopFinder.FindLoops();

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

    public Coordinate? CurrentPosition { get; set; }

    public Direction Facing { get; set; }

    public HashSet<Coordinate> Visited { get; set; } = [];

    public Dictionary<Direction, (int row, int col)> TravelVectors = new()
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

    public bool WithinMap => Map.WithinArray(CurrentPosition);

    public void Traverse()
    {
        var (newX, newY) = (CurrentPosition.X + TravelVectors[Facing].col, CurrentPosition.Y + TravelVectors[Facing].row);
        var newC = Map.GetPoint(newX, newY)?.C;
        if (newC is '#')
        {
            Facing = Facing is Direction.Left
                ? Direction.Up
                : Facing + 1;

            //Console.WriteLine($"Obstacle encountered ({newX}, {newY}), turning {Facing}");
        }
        else if (Map.WithinArray(newX, newY))
        {
            CurrentPosition = Map.GetPoint(newX, newY)!;    // not null because we just checked
            //Console.WriteLine($"Moving to new position, ({newX}, {newY})");
            if (newC is not '#' or 'X')
            {
                var newPos = CurrentPosition with { C = 'X' };
                Map.Array[newX, newY] = newPos;
                Visited.Add(newPos);
            }
        }
        else
        {
            Console.WriteLine($"Exited map! Last position was {CurrentPosition}");
            CurrentPosition = null;
        }
    }

    public int VisitedCount => Visited.Count;
}

public class ObstacleLoopFinder : Guard
{
    public HashSet<(Coordinate, Direction)> PreviousStates { get; set; } = [];

    public Coordinate StartPoint { get; init; }
    
    public ObstacleLoopFinder(CoordinateArray map, Coordinate start, HashSet<Coordinate> visited) 
        : base(map, start)
    {
        Visited = visited;
        StartPoint = start;
    }

    public int FindLoops()
    {
        
        HashSet<CoordinateArray> parallelUniverses = [];
        HashSet<(int, int)> addedObstacles = [];
        foreach (var coordinate in Visited)
        {
            var (x, y) = (coordinate.X, coordinate.Y);
            if (Map.WithinArray(x, y) == false 
            || (coordinate.X == StartPoint.X && coordinate.Y == StartPoint.Y))
            {
                continue;
            }

            var newMap = Map.Clone();
            newMap.Array[x, y] = new Coordinate(x, y, '#');
            if (addedObstacles.Add((x, y)))
                parallelUniverses.Add(newMap);
        }

        int totalLoops = 0;
        int currentPU = 1;
        foreach (var map in parallelUniverses)
        {
            Console.WriteLine($"PU: {currentPU}\n" + map.Print());
            const int MAX_STEPS = 10000;
            int steps = 0;
            bool isLoop = false;

            var newGuard = new Guard(map, StartPoint);
            PreviousStates = [(StartPoint with { C = 'X' }, newGuard.Facing)];
            while (CurrentPosition is not null 
                && steps < MAX_STEPS 
                && isLoop is false)
            {
                newGuard.Traverse();

                if (newGuard.CurrentPosition is null) break;

                var state = (newGuard.CurrentPosition, newGuard.Facing);
                if (PreviousStates.Contains(state))
                {
                    Console.WriteLine($"Loop found! (PU: {currentPU})\n{map.Print()}");
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

        // 1509 too low!
        return totalLoops;
    }
}
