using System;
using System.Linq;

namespace SimplexAlgoImplementation
{
    public enum SimplexResult { Unbounded, Found, NotYetFound }

    public class SimplexAlgorithm
    {
        // This Simplex Algorithm Implementation is based on the BigM Method to solve minimization problems
        // The Big M method is a version of the Simplex Algorithm that first finds a best feasible solution by adding “artificial” variables to the problem
        // Implemented by following a Step-by-Step Guide from https://cbom.atozmath.com/example/CBOM/Simplex.aspx?he=e&q=sm&ex=1
        // && http://arts.brainkart.com/article/big-m-method--introduction--1123/

        ObjectiveFunction objFunction;
        double[] objFunctionValues; // coefficients of objective function variables
        double[] pivotValues; // initially the constraint values, which are being altered by each iteration
        double[][] matrix; // matrix of constraint values, joined with slack and minimization (artificial) variables (BigM specific)
        bool[] indecesToIgnore; // bool[] which keeps track of which index is a minimization (artificial) variable and is therefore to be ignored
        double[] Zj; // Big M Method - Zj is the sum of all constraint coefficients in each column of the matrix
        double[] Cj; // Big M Method - Cj is initially the objFunctionValues but will be altered with each iteration
        int[] IndecesOfPivot; // saves the index of a pivot element 
        int iterations = 0; // amount of iterations until a result was found

        public SimplexAlgorithm(ObjectiveFunction function, Constraint[] constraints)
        { // initializing all variables in order to be able to calculate a result.
            this.objFunction = ConvertToMaximizationProblem(function);
            getMatrix(constraints);
            createObjFunctionArray();
            getZjAndCj();
        }

        public ObjectiveFunction ConvertToMaximizationProblem(ObjectiveFunction objFunction)
        { //multiplies the objective Function by -1
            double[] flippedFuncVars = new double[objFunction.coefficients.Length];

            for (int i = 0; i < objFunction.coefficients.Length; i++)
            {
                flippedFuncVars[i] = -objFunction.coefficients[i];
            }
            return new ObjectiveFunction(flippedFuncVars);
        }

        public void getMatrix(Constraint[] constraints)
        {
            CreateMatrixFromConstraints(constraints); //Matrix contains only the constraints at the moment
            double[][] slackMatrix = new double[0][];
            double[] coefficients = new double[constraints.Length];
            for (int i = 0; i < constraints.Length; i++)
            { //creates an array with the specified value for each constraint and appends it to the matrix
              //will diagonally be filled with -1's from top left to bottom right
                var currentConstraint = constraints[i];
                coefficients[i] = currentConstraint.rightHandSide;
                slackMatrix = addColumn(slackMatrix, createArrayWithSpecifiedValue(-1, i, constraints.Length));
            }

            var joinedMatrix = JoinMatrices(slackMatrix, constraints);
            IndecesOfPivot = new int[constraints.Length];

            for (int i = 0; i < constraints.Length; i++)
            {// adds arrays to the joinedmatrix filled with 1's diagonally from top left to bottom right
                joinedMatrix = addColumn(joinedMatrix, createArrayWithSpecifiedValue(1, i, constraints.Length));
                IndecesOfPivot[i] = joinedMatrix.Length - 1;
                //saves the position of the pivot elements inside the array
            }

            indecesToIgnore = new bool[joinedMatrix.Length];
            for (int i = 0; i < constraints.Length; i++)
            {//saves which index is an artificial variable
                indecesToIgnore[indecesToIgnore.Length - i - 1] = true;
            }

            this.pivotValues = coefficients;
            this.matrix = joinedMatrix;
        }
        public void CreateMatrixFromConstraints(Constraint[] constraints)
        { // creates a matrix from the parsed constraints
            matrix = new double[constraints.First().coefficients.Length][];

            for (int a = 0; a < constraints.First().coefficients.Length; a++)
            {
                matrix[a] = new double[constraints.Length];
                for (int b = 0; b < constraints.Length; b++)
                {
                    matrix[a][b] = constraints[b].coefficients[a];
                }
            }
        }
        double[][] addColumn(double[][] matrix, double[] columnToAdd)
        {// adds a given column to a given matrix
            double[][] newMatrix = new double[matrix.Length + 1][]; // increase amount of columns of the matrix by 1
            for (int i = 0; i < matrix.Length; i++)
            {//copies the matrix
                newMatrix[i] = matrix[i];
            }
            newMatrix[matrix.Length] = columnToAdd; // appends the column to the matrix
            return newMatrix;
        }
        double[] createArrayWithSpecifiedValue(double value, int indexToPlace, int length)
        { //Creates an array with one element having the specified value at the specified place and the rest filled with 0's
            //used for creating diagonal slack & artificial matrices
            double[] newColumn = new double[length];

            for (int i = 0; i < length; i++)
            {
                if (i == indexToPlace)
                    newColumn[i] = value;
                else
                    newColumn[i] = 0;
            }
            return newColumn;
        }
        private double[][] JoinMatrices(double[][] slackMatrix, Constraint[] constraints)
        { //creates a matrix by joining both the slack variable Matrix & the constraints Matrix
            var joinedMatrix = new double[constraints.First().coefficients.Length + slackMatrix.Length][];

            for (int i = 0; i < constraints.First().coefficients.Length; i++)
            {
                joinedMatrix[i] = matrix[i];
            }

            for (int i = constraints.First().coefficients.Length; i < constraints.First().coefficients.Length + slackMatrix.Length; i++)
            {
                joinedMatrix[i] = slackMatrix[i - constraints.First().coefficients.Length];
            }
            return joinedMatrix;
        }
        public void createObjFunctionArray()
        { // fills an array with length of the matrix with Obj Function values and the rest with 0
            double[] filledObjFuncArray = new double[matrix.Length];
            for (int i = 0; i < matrix.Length; i++)
            {
                if (i < objFunction.coefficients.Length)
                    filledObjFuncArray[i] = objFunction.coefficients[i];
                else
                    filledObjFuncArray[i] = 0;
            }
            this.objFunctionValues = filledObjFuncArray;
        }
        void getZjAndCj()
        {
            Zj = new double[matrix.Length];
            Cj = new double[matrix.Length];

            for (int a = 0; a < matrix.Length; a++)
            {
                double sum = 0;
                double sumOfConstraintCoefficientsInColumn = 0;
                for (int b = 0; b < matrix.First().Length; b++)
                {
                    if (indecesToIgnore[IndecesOfPivot[b]])
                    {
                        sumOfConstraintCoefficientsInColumn -= matrix[a][b];
                    }
                    else
                    {
                        sum += objFunctionValues[IndecesOfPivot[b]] * matrix[a][b];
                    }
                }
                Zj[a] = indecesToIgnore[a] ? 1 : sumOfConstraintCoefficientsInColumn; //Zj filled with sum of Constraint coefficients and filled with 1's for surplus vars.
                Cj[a] = sum - objFunctionValues[a];
            }
        }
        //
        //
        //
        //
        //
        // incomming functions in order to calculate the result
        public Tuple<FinalSolutionPresenter, SimplexResult> calcResult()
        {
            SimplexIterationResult result = iterationResult();
            while (result.result != SimplexResult.Found && iterations < 300)
            {//search for a result until its found, with a maximum of 300 iterations
                recalculateSimplexTable(result.index);
                getZjAndCj(); //getting new Cj and Zj after the simplex table has been recalculated
                result = iterationResult();
                iterations++;
            }
            var solution = new FinalSolutionPresenter(objFunctionValues, IndecesOfPivot, pivotValues, iterations);
            return new Tuple<FinalSolutionPresenter, SimplexResult>(solution, result.result);
        }
        SimplexIterationResult iterationResult() //if there is a negative element in Cj or Zj, the tableau will be calculated again with the new pivot
        {
            int PivotIndexOfZj = IndexMostNegativeValue(Zj);
            if (PivotIndexOfZj == -1)//will be -1 only if Zj is >= 0
            {// there is no negative element in Zj
                int indexPivotCj = IndexMostNegativeValue(Cj);
                if (indexPivotCj != -1) //Cj has 1 or more negative values which are not artificial vars
                {
                    int indexPivotRow = IndexOfMinimalRatio(matrix[indexPivotCj], pivotValues);
                    return new SimplexIterationResult(new Tuple<int, int>(indexPivotCj, indexPivotRow), SimplexResult.NotYetFound);
                }
                else
                {//Cj & Zj have no negative values - cancel condition - Solution has been found
                    return new SimplexIterationResult(null, SimplexResult.Found);
                }
            }
            else
            { //there is a negative elemnt in Zj
                int indexOfRowWithMinimalRatio = IndexOfMinimalRatio(matrix[PivotIndexOfZj], pivotValues);
                if (indexOfRowWithMinimalRatio != -1)
                {
                    return new SimplexIterationResult(new Tuple<int, int>(PivotIndexOfZj, indexOfRowWithMinimalRatio), SimplexResult.NotYetFound);
                }
                else
                {
                    return new SimplexIterationResult(null, SimplexResult.Unbounded);
                }
            }
        }
        int IndexMostNegativeValue(double[] array)
        { // calcs the index of the pivot element (most negative value)
            int index = -1;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < 0)
                {
                    if (!indecesToIgnore[i]) //only respect indices which are not artificial variables
                    {
                        if (index == -1)
                        {
                            index = i;
                        }
                        else if (Math.Abs(array[i]) > Math.Abs(array[index]))
                        {
                            index = i;
                        }
                    }
                }
            }
            return index;
        }
        int IndexOfMinimalRatio(double[] column, double[] constraintValues)
        {//compares the ratios between constraint value and identified pivot value and returns the index of the row with the smallest ratio
            int index = -1;
            for (int i = 0; i < column.Length; i++)
            {
                if (column[i] > 0 && constraintValues[i] > 0)
                {
                    if (index == -1)
                    {
                        index = i;
                    }
                    else if (constraintValues[i] / column[i] < constraintValues[index] / column[index])
                    {
                        index = i;
                    }
                }
            }
            return index;
        }
        void recalculateSimplexTable(Tuple<int, int> pivotColumnMinIndex)
        {//Step 5 of the BigM Method, Determining the new solution
            IndecesOfPivot[pivotColumnMinIndex.Item2] = pivotColumnMinIndex.Item1;

            //Step 5 BigM Method section A - normalizing pivot Column
            double[] normalizedPivotColumn = new double[matrix.Length];
            double pivotElement = matrix[pivotColumnMinIndex.Item1][pivotColumnMinIndex.Item2];
            for (int i = 0; i < matrix.Length; i++)
            {
                var keyRowElement = matrix[i][pivotColumnMinIndex.Item2];
                normalizedPivotColumn[i] = keyRowElement / pivotElement; //normalized pivot by division through its value
            }

            calcNewConstraintValues(pivotColumnMinIndex);

            //Step 5 section B BigM - create new matrix with the new values
            double[][] newMatrix = new double[matrix.Length][];
            for (int a = 0; a < matrix.Length; a++)
            {
                newMatrix[a] = new double[IndecesOfPivot.Length];
                for (int b = 0; b < IndecesOfPivot.Length; b++)
                {
                    if (b == pivotColumnMinIndex.Item2) //checks if the current matrix value is that of the pivot row
                    {
                        newMatrix[a][b] = normalizedPivotColumn[a]; // Section A - sets the new matrix value to the normalized pivot value
                    }
                    else
                    {
                        //Section B - Row(new) = Row(old) - (value of key column and Row(old)) × KeyRow(new)
                        newMatrix[a][b] = matrix[a][b] - normalizedPivotColumn[a] * matrix[pivotColumnMinIndex.Item1][b];
                    }
                }
            }
            matrix = newMatrix;
        }
        private void calcNewConstraintValues(Tuple<int, int> pivotColumnMinIndex)
        {
            double[] newConstraintValues = new double[pivotValues.Length];
            for (int i = 0; i < pivotValues.Length; i++)
            {
                double pivot = matrix[pivotColumnMinIndex.Item1][pivotColumnMinIndex.Item2];
                if (i == pivotColumnMinIndex.Item2)
                {//constraint value is that of pivot row -> normalize
                    newConstraintValues[i] = pivotValues[i] / pivot;
                }
                else
                {//constraint is not that of pivot row -> calc value
                    newConstraintValues[i] = pivotValues[i] - pivotValues[pivotColumnMinIndex.Item2] / pivot * matrix[pivotColumnMinIndex.Item1][i];
                }
            }
            pivotValues = newConstraintValues;
        }
    }
}