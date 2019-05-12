using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionLib
{
    /// <summary>
    /// Exponential regression model
    /// </summary>
    [Serializable]
    public class ExponentialRegression : LinearRegression
    {
        /// <summary>
        /// Inherits LinearRegression constructor
        /// </summary>
        /// <param name="xVals"></param>
        /// <param name="yVals"></param>
        public ExponentialRegression(double[] xVals, double[] yVals) : base(xVals, yVals)
        {}

        /// <summary>
        /// Overrides PredictY from base class with e^(b + a*x)
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public override double PredictY(double x)
        {
            return Math.Exp(freeCoefficient + xCoefficient * x);
        }

        /// <summary>
        /// Replaces yVals[i] with ln(yVals[i])
        /// </summary>
        /// <param name="yVals"></param>
        /// <returns></returns>
        protected override double[] GetConvertedY(double[] yVals)
        {
            return Array.ConvertAll(yVals, d => Math.Log(d)); 
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"e^({freeCoefficient:f5} + {xCoefficient:f5}x)";
        }
    }
}
