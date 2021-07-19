using System;

namespace SimplexAlgoImplementation
{
    public class Constraint
    {
        public double[] coefficients;
        public double rightHandSide;
        public string operation = ">=";

        public Constraint(double[] coefficients, double rightHandSide)
        {
            this.coefficients = coefficients;
            this.rightHandSide = rightHandSide;
        }
    }
}
