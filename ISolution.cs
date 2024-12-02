using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024
{
    public interface ISolution
    {
        public string Name { get; }
        public string PuzzleInput { get; }
        public string TestInput { get; }
        public string Solve(Part part);
        public string Test(Part part);
    }

    public enum Part
    {
        One = 1,
        Two = 2,
    }
}
