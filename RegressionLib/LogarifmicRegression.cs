using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionLib
{
    /// <summary>
    /// Logarifmic regression model
    /// </summary>
    [Serializable]
    public class LogarifmicRegression : LinearRegression
    {
        /// <summary>
        /// Inherits LinearRegression constructor
        /// </summary>
        /// <param name="xVals"></param>
        /// <param name="yVals"></param>
        public LogarifmicRegression(double[] xVals, double[] yVals) : base(xVals, yVals)
        {}

        /// <summary>
        /// Overrides PredictY from base class with a*ln(x) + b;
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public override double PredictY(double x)
        {
            return freeCoefficient + xCoefficient * Math.Log(x);
        }

        /// <summary>
        /// Replaces xVals[i] with ln(xVals[i])
        /// </summary>
        /// <param name="yVals"></param>
        /// <returns></returns>
        protected override double[] GetConvertedX(double[] xVals)
        {
            return Array.ConvertAll(xVals, d => Math.Log(d));
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{freeCoefficient:f5} + {xCoefficient:f5} * ln(x)";
        }
    }
}
