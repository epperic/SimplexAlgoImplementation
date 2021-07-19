using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimplexAlgoImplementation
{
    class Program
    {
        static void Main(string[] args)
        {
            string selectedPath = selectFile();
            var benchmarkParser = new BenchmarkParser(selectedPath);

            Constraint[] constraints = new Constraint[benchmarkParser.parsedConstraints.Count];
            for (int i = 0; i < benchmarkParser.parsedConstraints.Count; i++)
            {
                double[] variables = benchmarkParser.parsedConstraints[i];
                double rightHandSide = benchmarkParser.rightHandside[i];
                constraints[i] = new Constraint(variables, rightHandSide);
            }
           
            ObjectiveFunction objectiveFunction = new ObjectiveFunction(benchmarkParser.parsedObjectiveFunction);
            SimplexAlgorithm simplex = new SimplexAlgorithm(objectiveFunction, constraints);
            Tuple<FinalSolutionPresenter, SimplexResult> result = simplex.calcResult();

            switch (result.Item2)
            {
                case SimplexResult.Found:
                    result.Item1.presentResult(constraints.First().coefficients.Length);
                    break;
                case SimplexResult.Unbounded:
                    Console.WriteLine("The domain of admissible solutions is unbounded");
                    break;
                case SimplexResult.NotYetFound:
                    Console.WriteLine("Could not find an optimal solution.");
                    break;
            }
        }
        private static string selectFile()
        {
            string selection = "";
            try
            {
                string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Benchmarks";
                var files = from file in Directory.EnumerateFiles(docPath, "*.txt", SearchOption.AllDirectories)
                            select new { File = file };
                int count = 1;
                Console.WriteLine("Welcome to my Simplex Algorithm implementation.");
                Console.WriteLine($"{files.Count().ToString()} files have been found inside the Benchmark folder:");
                Console.WriteLine();
                foreach (var f in files)
                {
                    Console.WriteLine($"{count}. {f.File}\t");
                    count++;
                }
                Console.WriteLine();
                Console.WriteLine("Please enter the number of the Benchmark you want to run.");
                try
                {
                    int input = Convert.ToInt32(Console.ReadLine());
                    var filesArray = files.ToArray();
                    selection = filesArray[input - 1].File;
                    Console.WriteLine($"Loading in selected File: {selection}");
                    Console.WriteLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            catch (UnauthorizedAccessException uAEx)
            {
                Console.WriteLine(uAEx.Message);
            }
            catch (PathTooLongException pathEx)
            {
                Console.WriteLine(pathEx.Message);
            }
            return selection;
        }
    }
}
