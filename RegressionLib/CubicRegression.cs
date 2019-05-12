using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionLib
{
    /// <summary>
    /// Cubic regression model
    /// </summary>
    [Serializable]
    public class CubicRegression : AbstractRegression
    {
        /// <summary>
        /// Regression coefficients
        /// </summary>
        private double xCubicCoef, xSqCoef, xCoef, freeCoef;
        /// <summary>
        /// Calculates regression coefficients, approximation error and correlation coefficient
        /// </summary>
        /// <param name="xVals"></param>
        /// <param name="yVals"></param>
        public CubicRegression(double[] xVals, double[] yVals) : base(xVals, yVals)
        {
            double[,] matrix = new double[4, 4];
            double[] vector = new double[4];
            matrix[0, 0] = GetMultipliedSum(new List<double[]>() { xVals, xVals, xVals });
            matrix[0, 1] = GetMultipliedSum(new List<double[]>() { xVals, xVals });
            matrix[0, 2] = xVals.Sum();
            matrix[0, 3] = xVals.Length;
            matrix[1, 0] = GetMultipliedSum(new List<double[]>() { xVals, xVals, xVals, xVals });
            matrix[1, 1] = matrix[0, 0];
            matrix[1, 2] = matrix[0, 1];
            matrix[1, 3] = matrix[0, 2];
            matrix[2, 0] = GetMultipliedSum(new List<double[]>() { xVals, xVals, xVals, xVals, xVals });
            matrix[2, 1] = matrix[1, 0];
            matrix[2, 2] = matrix[1, 1];
            matrix[2, 3] = matrix[1, 2];
            matrix[3, 0] = GetMultipliedSum(new List<double[]>() { xVals, xVals, xVals, xVals, xVals, xVals });
            matrix[3, 1] = matrix[2, 0];
            matrix[3, 2] = matrix[2, 1];
            matrix[3, 3] = matrix[2, 2];
            vector[0] = yVals.Sum();
            vector[1] = GetMultipliedSum(new List<double[]>() { xVals, yVals });
            vector[2] = GetMultipliedSum(new List<double[]>() { xVals, xVals, yVals });
            vector[3] = GetMultipliedSum(new List<double[]>() { xVals, xVals, xVals, yVals });
            System = new SystemOfLinearEquations(matrix, vector);
            List<double> solvation = System.Solve();
            xCubicCoef = solvation[0];
            xSqCoef = solvation[1];
            xCoef = solvation[2];
            freeCoef = solvation[3];
            correlationCoefficient = GetCorrelationCoefficient();
            approximationError = GetError();
        }
        /// <summary>
        /// A vector of regression coefficient is got by solving this system
        /// </summary>
        public SystemOfLinearEquations System { get; set; }
        /// <summary>
        /// Overrides PredictY method from AbstractRegression with a1*x^3 + a2*x^2 + a3*x + b
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public override double PredictY(double x)
        {
            return xCubicCoef * x * x * x + xSqCoef * x * x + xCoef * x + freeCoef;
        }
        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{xCubicCoef:f5} * x^3 + {xSqCoef:f5} * x^2 + {xCoef:f5} * x + {freeCoef:f5}";
        }
    }
}
