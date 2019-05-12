using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionLib
{
    /// <summary>
    /// Semi-abstract model used for exponential, hyperbolic and logarifmic models with variable replacement
    /// </summary>
    [Serializable]
    public class LinearRegression : AbstractRegression
    {
        /// <summary>
        /// Replaced values for linearized regression models (exponential, hyperbolic, logarifmic)
        /// </summary>
        protected double[] xValsConverted, yValsConverted;
        protected double xCoefficient, freeCoefficient;
        /// <summary>
        /// replaces input values (if needed), calculates regression coefficients for linear regression with replaced values (which are the same for the linearized regression models)
        /// calculates correlation coefficient and approximation error
        /// </summary>
        /// <param name="xVals"></param>
        /// <param name="yVals"></param>
        public LinearRegression(double[] xVals, double[] yVals) : base(xVals, yVals)
        {
            xValsConverted = GetConvertedX(xVals);
            yValsConverted = GetConvertedY(yVals);
            xCoefficient = GetXCoefficient();
            freeCoefficient = GetFreeCoefficient();
            yAvg = yVals.Average();
            correlationCoefficient = GetCorrelationCoefficient();
            approximationError = GetError();
        }

        /// <summary>
        /// Overrides PredictY method from AbstractRegression with a*x + b
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public override double PredictY(double x)
        {
            return xCoefficient * x + freeCoefficient;
        }

        /// <summary>
        /// calculates x coefficient for replaced values
        /// </summary>
        /// <returns></returns>
        protected virtual double GetXCoefficient()
        {
            return (xValsConverted.Length * GetMultipliedSum(new List<double[]>() { xValsConverted, yValsConverted }) - xValsConverted.Sum() * yValsConverted.Sum()) /
    (xValsConverted.Length * GetMultipliedSum(new List<double[]> { xValsConverted, xValsConverted }) - Math.Pow(xValsConverted.Sum(), 2));
        }

        /// <summary>
        /// calculates free coefficient for replaced values
        /// </summary>
        /// <returns></returns>
        protected virtual double GetFreeCoefficient()
        {
            return (yValsConverted.Sum() - xCoefficient * xValsConverted.Sum()) / xValsConverted.Length;
        }

        /// <summary>
        /// replacing x values (not needed for linear regression, but needed for linearized models)
        /// </summary>
        /// <param name="xVals"></param>
        /// <returns></returns>
        protected virtual double[] GetConvertedX(double[] xVals)
        {
            return xVals;
        }
        /// <summary>
        /// replacing y values (not needed for linear regression, but needed for linearized models)
        /// </summary>
        /// <param name="xVals"></param>
        /// <returns></returns>
        protected virtual double[] GetConvertedY(double[] yVals)
        {
            return yVals;
        }
        /// <summary>
        /// string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{xCoefficient:f5}x + {freeCoefficient:f5}";
        }
    }
}
