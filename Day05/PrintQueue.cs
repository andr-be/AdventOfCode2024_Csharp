using System.Runtime.CompilerServices;

namespace AdventOfCode2024.Day05
{
    internal class PrintQueue : SolutionBase
    {
        public override string Name => "Day 05: Print Queue";

        public override int Day => 5;

        public override string Solution(Part part, string input)
        {
            var (orderingSection, pageNumbersSection) =
                (input.Split("\r\n\r\n")[0].ToString(), input.Split("\r\n\r\n")[1].ToString());

            // "x | y" == Ordering(Before: x, After: y)
            List<Ordering> pageOrderings = orderingSection
                .Split(Environment.NewLine)
                .Select(l => new Ordering
                (
                    int.Parse(l.Split('|')[0]),
                    int.Parse(l.Split('|')[1])
                ))
                .OrderBy(o => o.Before)
                .ToList();

            // "a,b,c,d" == [a, b, c, d];
            var updates = pageNumbersSection
                .Split(Environment.NewLine)
                .Where(l => !string.IsNullOrEmpty(l))
                .Select(l => l
                    .Split(',')
                    .Select(int.Parse)
                    .ToList())
                .ToList();

            List<List<int>> invalidUpdates = [];
            List<int> midPoints = [];
            foreach (var update in updates)
            {
                if (IsValidUpdate(update, pageOrderings))
                {
                    midPoints.Add(update[update.Count / 2]);
                }
                else
                {
                    invalidUpdates.Add(update);
                }
            }

            if (part is Part.One)
                return midPoints.Sum().ToString();

            List<int> newMidPoints = [];
            foreach (var update in invalidUpdates)
            {
                Console.WriteLine($"Incorrect: [{string.Join(", ", update)}]");
                var fixedUpdate = TopologicalSort(update, pageOrderings);

                newMidPoints.Add(fixedUpdate[fixedUpdate.Count / 2]);
                Console.WriteLine($"Fixed: [{string.Join(", ", fixedUpdate)}] ({newMidPoints.Last()}");
            }

            return newMidPoints.Sum().ToString();
        }

        private static bool IsValidUpdate(List<int> update, List<Ordering> pageOrderings)
        {
            bool isValid = true;
            for (int i = 0; i < update.Count && isValid; i++)
                for (int j = update.Count - 1; j > i && isValid; j--)
                    if (!pageOrderings.ValidPair(update[i], update[j]))
                        isValid = false;

            return isValid;
        }

        private static List<int> TopologicalSort(List<int> update, List<Ordering> orderings)
        {
            // Build the graph of dependencies
            var graph = new Dictionary<int, HashSet<int>>();
            foreach (var num in update)
                graph[num] = [];

            foreach (var order in orderings)
            {
                if (update.Contains(order.Before) && update.Contains(order.After))
                {
                    if (!graph.ContainsKey(order.After))
                        graph[order.After] = [];

                    // if y|x is a rule, add x to y's "must come before" list
                    graph[order.After].Add(order.Before);
                }
            }

            // Use DFS to visit nodes in the right order
            var sorted = new List<int>();
            var visited = new HashSet<int>();
            var temp = new HashSet<int>();

            void Visit(int n)
            {
                if (temp.Contains(n) || visited.Contains(n)) 
                    return;                                   // skip if done

                temp.Add(n);                                  // mark as 'in progress'

                foreach (var dep in graph[n])                 // visit all dependencies first
                    Visit(dep);

                temp.Remove(n);                               // done with this node 
                visited.Add(n);                               // mark as completed
                sorted.Add(n);                                // add to result
            }

            // Visit all nodes if you haven't already
            foreach (var n in update)
                if (!visited.Contains(n))
                    Visit(n);

            // Reverse the sort to get the correct order
            sorted.Reverse();
            return sorted;
        }
    }

    public record Ordering(int Before, int After);

    public static class OrderingExtensions
    {
        public static bool HasPagesAfter(this List<Ordering> list, int i) => 
            list.Any(o => o.Before == i);

        public static List<int>? GetAfters(this List<Ordering> list, int i) => list.HasPagesAfter(i) 
            ? list.Where(o => o.Before == i)
                .Select(o => o.After)
                .ToList() 
            : null;

        public static bool ValidPair(this List<Ordering> list, int i, int j) => 
            !(list.GetAfters(j)?.Contains(i) ?? false);
    }
}
