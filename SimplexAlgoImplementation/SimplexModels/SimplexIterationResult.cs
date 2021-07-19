using System;

namespace SimplexAlgoImplementation
{
    public class SimplexIterationResult
    {
        public Tuple<int, int> index;
        public SimplexResult result;

        public SimplexIterationResult(Tuple<int, int> index, SimplexResult result)
        {
            this.index = index;
            this.result = result;
        }
    }
}