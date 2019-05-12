using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionLib
{
    public class Exponential1Regression : ExponentialRegression
    {
        public Exponential1Regression(double[] xVals, double[] yVals) : base(xVals, yVals)
        {}

        protected override double GetXCoefficient()
        {
            return Math.Exp(base.GetXCoefficient());
        }
        protected override double GetFreeCoefficient()
        {
            return Math.Exp((yValsConverted.Sum() - Math.Log(xCoefficient) * xValsConverted.Sum()) / xValsConverted.Length);
        }
        public override string ToString()
        {
            return $"{a0:f5} * {xCoefficient:f5}^x";
        }
    }
}
