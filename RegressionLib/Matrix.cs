using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionLib
{
    class SystemOfLinearEquations
    {
        double[,] values;
        double[] vector;

        public SystemOfLinearEquations(double[,] values, double[] vector)
        {
            this.values = values;
            this.vector = vector;
        }

        public double GetDeterminant()
        {
            throw new NotImplementedException();
        }

    }
}
