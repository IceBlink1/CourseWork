using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionLib
{
    /// <summary>
    /// Abstract regression model
    /// </summary>
    [Serializable]
    public abstract class AbstractRegression
    {
        /// <summary>
        /// Given vectors of values
        /// </summary>
        public double[] xVals, yVals;
        public double yAvg, correlationCoefficient, approximationError;

        /// <summary>
        /// Calculates average of y values, gives fields the values
        /// </summary>
        /// <param name="xVals"></param>
        /// <param name="yVals"></param>
        public AbstractRegression(double[] xVals, double[] yVals)
        {
            this.xVals = xVals;
            this.yVals = yVals;
            yAvg = yVals.Average();
        }

        /// <summary>
        /// Abstract prediction method
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public abstract double PredictY(double x);

        /// <summary>
        /// Calculates multiplied sum of n vectors
        /// </summary>
        /// <param name="vals"></param>
        /// <returns></returns>
        protected double GetMultipliedSum(List<double[]> vals)
        {
            double sum = 0;
            for (int i = 0; i < vals[0].Length; i++)
            {
                double k = 1;
                for (int j = 0; j < vals.Count; j++)
                {
                    k *= vals[j][i];
                }
                sum += k;
            }
            return sum;
        }

        /// <summary>
        /// Calculates correliation coefficient for a regression
        /// </summary>
        /// <returns></returns>
        protected virtual double GetCorrelationCoefficient()
        {
            double squareDev = GetSquareDev();
            return Math.Sqrt(1 - (squareDev / yVals.Sum(y => Math.Pow(y - yAvg, 2))));
        }

        /// <summary>
        /// Calculates approximation error for a regression
        /// </summary>
        /// <returns></returns>
        protected virtual double GetError()
        {
            double sum = 0;
            for (int i = 0; i < yVals.Length; i++)
            {
                sum += Math.Abs(yVals[i] - PredictY(xVals[i])) / yVals[i];
            }
            return sum / yVals.Length;
        }

        /// <summary>
        /// Calculates square deviation
        /// </summary>
        /// <returns></returns>
        protected virtual double GetSquareDev()
        {
            double squareDev = 0;
            for (int i = 0; i < yVals.Length; i++)
            {
                squareDev += Math.Pow(yVals[i] - PredictY(xVals[i]), 2);
            }
            return squareDev;
        }
    }
}
