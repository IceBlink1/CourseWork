using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionLib
{
    /// <summary>
    /// Hyperbolic regression model
    /// </summary>
    [Serializable]
    public sealed class HyperbolicRegression : LinearRegression
    {
        /// <summary>
        /// Inherits LinearRegression constructor
        /// </summary>
        /// <param name="xVals"></param>
        /// <param name="yVals"></param>
        public HyperbolicRegression(double[] xVals, double[] yVals) : base(xVals, yVals)
        {}

        /// <summary>
        /// Overrides PredictY from base class with b + a/x;
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public override double PredictY(double x)
        {
            return freeCoefficient + xCoefficient / x;
        }

        /// <summary>
        /// Replaces xVals[i] with 1/xVals[i]
        /// </summary>
        /// <param name="yVals"></param>
        /// <returns></returns>
        protected override double[] GetConvertedX(double[] xVals)
        {
            return Array.ConvertAll(xVals, x => 1 / x);
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{freeCoefficient:f5} + {xCoefficient:f5}/x";
        }
    }
}
