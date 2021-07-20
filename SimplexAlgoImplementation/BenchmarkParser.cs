using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimplexAlgoImplementation
{
    class BenchmarkParser
    {
        public string benchmarkPath { get; set; }
        public List<double> rightHandside = new List<double>();
        public List<double[]> parsedConstraints = new List<double[]>();
        public double[] parsedObjectiveFunction { get; set; }

        public BenchmarkParser(string benchmarkPath)
        {
            this.benchmarkPath = benchmarkPath;
            parse();
        }
        private void parse()
        {
            if (benchmarkPath == null || benchmarkPath == "")
            {

            }
            else // projectpath is set
            {
                String line;
                try
                {
                    StreamReader sr = new StreamReader(this.benchmarkPath);
                    string[] splitter = { " + ", " >= ", ";" };

                    line = sr.ReadLine();
                    //Continue to read until you reach end of file
                    while (line != null)
                    {
                        if (!line.StartsWith("//"))
                        {
                            if (line.StartsWith("min:"))
                            { //line is the obj. Function
                                line = line.Substring(7);
                                string[] objectiveFunction = line.Split(splitter, System.StringSplitOptions.RemoveEmptyEntries);
                                //contains whole block e.g "10*x7"
                                parsedObjectiveFunction = extractCoefficients(objectiveFunction);
                            }
                            else // line is a constraint
                            {
                                line = line.Substring(3);
                                string[] constraintVariables = line.Split(splitter, System.StringSplitOptions.RemoveEmptyEntries);
                                rightHandside.Add(Convert.ToDouble(constraintVariables[constraintVariables.Length - 1]));
                                constraintVariables = constraintVariables.Take(constraintVariables.Length - 1).ToArray();
                                parsedConstraints.Add(extractCoefficients(constraintVariables));
                            }
                        }
                        line = sr.ReadLine();
                    }
                    //close the file
                    sr.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
            }

        }
        private double[] extractCoefficients(string[] parsedVariables)
        {
            double[] coefficients = new double[parsedVariables.Length];
            for (int i = 0; i < parsedVariables.Length; i++)
            {
                var splitIndex = parsedVariables[i].IndexOf('*');
                coefficients[i] = Convert.ToDouble(parsedVariables[i].Remove(splitIndex));
            }
            return coefficients;
        }
    }
}
