using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionLib
{
    /// <summary>
    /// Square regression model
    /// </summary>
    [Serializable]
    public class SquareRegression : AbstractRegression
    {
        /// <summary>
        /// Regression coefficients
        /// </summary>
        private double xSqCoef, xCoef, freeCoef;

        /// <summary>
        /// Calculates regression coefficients, approximation error and correlation coefficient
        /// </summary>
        /// <param name="xVals"></param>
        /// <param name="yVals"></param>
        public SquareRegression(double[] xVals, double[] yVals) : base(xVals, yVals)
        {
            double[,] matrix = new double[3, 3];
            double[] vector = new double[3];
            matrix[0, 0] = GetMultipliedSum(new List<double[]>() { xVals, xVals });
            matrix[0, 1] = xVals.Sum();
            matrix[0, 2] = xVals.Length;
            matrix[1, 0] = GetMultipliedSum(new List<double[]>() { xVals, xVals, xVals });
            matrix[1, 1] = matrix[0, 0];
            matrix[1, 2] = matrix[0, 1];
            matrix[2, 0] = GetMultipliedSum(new List<double[]>() { xVals, xVals, xVals, xVals });
            matrix[2, 1] = matrix[1, 0];
            matrix[2, 2] = matrix[1, 1];
            vector[0] = yVals.Sum();
            vector[1] = GetMultipliedSum(new List<double[]>() { xVals, yVals });
            vector[2] = GetMultipliedSum(new List<double[]>() { xVals, xVals, yVals });
            system = new SystemOfLinearEquations(matrix, vector);
            List<double> solvation = system.Solve();
            xSqCoef = solvation[0];
            xCoef = solvation[1];
            freeCoef = solvation[2];
            correlationCoefficient = GetCorrelationCoefficient();
            approximationError = GetError();
        }
        /// <summary>
        /// A vector of regression coefficient is got by solving this system
        /// </summary>
        public SystemOfLinearEquations system { get; set; }
        /// <summary>
        /// Overrides PredictY method from AbstractRegression with a1*x^2 + a2*x + b
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public override double PredictY(double x)
        {
            return xSqCoef * x * x + xCoef * x + freeCoef;
        }
        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{xSqCoef:f5} * x^2 + {xCoef:f5} * x + {freeCoef:f5}";
        }
    }
}
