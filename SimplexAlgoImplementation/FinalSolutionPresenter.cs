using System;

namespace SimplexAlgoImplementation
{
    public class FinalSolutionPresenter
    {
        public double[] objFunctionValues;
        public int[] indecesOfPivot;
        public double[] pivotValues;
        public double optimalSolution;
        public int iterations;

        public FinalSolutionPresenter(double[] objFunctionValues, int[] indecesOfPivot, double[] pivotValues, int iterations)
        {
            this.objFunctionValues = objFunctionValues;
            this.indecesOfPivot = indecesOfPivot;
            this.pivotValues = pivotValues;
            this.iterations = iterations;
            optimalSolution = calcOptimalSolution();
        }

        private double calcOptimalSolution()
        {
            double objectiveFunctionValue = 0;
            for (int i = 0; i < indecesOfPivot.Length; i++)
            {
                objectiveFunctionValue += -objFunctionValues[indecesOfPivot[i]] * pivotValues[i];
            }
            return objectiveFunctionValue;
        }

        public void presentResult(int amountOfConstraintVariables)
        {
            Console.WriteLine("The optimal solution for this minimization problem has been found: " + $"{optimalSolution}" + " after " + $"{iterations}" + " iterations.");
            Console.WriteLine();
            Console.WriteLine("All x Values: ");
            Console.WriteLine();
            var xValuesArray = createXValuesArray(indecesOfPivot, pivotValues, amountOfConstraintVariables);
            for (int i = 0; i < xValuesArray.Length; i++)
            {
                Console.WriteLine("x" + $"{i}" + ": " + xValuesArray[i]);
            }
            Console.ReadLine();
        }
        private double[] createXValuesArray(int[] IndecesOfPivot, double[] pivotValues, int amountOfConstraintVariables)
        {
            double[] pivotElementArray = new double[amountOfConstraintVariables];

            for (int i = 0; i < pivotValues.Length; i++)
            {
                if (IndecesOfPivot[i] < amountOfConstraintVariables)
                {
                    pivotElementArray[indecesOfPivot[i]] = pivotValues[i];
                }
            }
            for (int i = 0; i < amountOfConstraintVariables; i++)
            {
                if (pivotElementArray[i] == null)
                {
                    pivotElementArray[i] = 0;
                }
            }
            return pivotElementArray;
        }
    }
}
