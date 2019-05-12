using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionLib
{
    /// <summary>
    /// Solves systems of linear equations (A*x = b, where A is a matrix and b is a vector)
    /// </summary>
    [Serializable]
    public class SystemOfLinearEquations
    {
        /// <summary>
        /// Given matrix
        /// </summary>
        private double[,] matrix;
        /// <summary>
        /// Given vector
        /// </summary>
        private double[] vector;
        /// <summary>
        /// Initializes fields matrix and vector
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vector"></param>
        public SystemOfLinearEquations(double[,] matrix, double[] vector)
        {
            this.matrix = matrix;
            this.vector = vector;
        }
        /// <summary>
        /// Solves the system by using Gauss method
        /// </summary>
        /// <returns></returns>
        public List<double> Solve()
        {
            double[,] matrix = this.matrix;
            double[] vector = this.vector;
            for (int i = 0; i < vector.Length; i++)
            {
                double diagVal = matrix[i, i];
                for (int j = i + 1; j < vector.Length; j++)
                {
                    double columnVal = matrix[j, i];
                    for (int k = 0; k < vector.Length; k++)
                    {
                        matrix[j, k] = matrix[j, k] * diagVal - matrix[i, k] * columnVal;
                    }
                    vector[j] = vector[j] * diagVal - vector[i] * columnVal;
                }
            }
            List<double> solution = new List<double>();
            for (int i = vector.Length - 1; i >= 0; i--)
            {
                double x = vector[i];
                for (int j = vector.Length - 1; j > vector.Length - solution.Count - 1; j--)
                {
                    x -= matrix[i, j] * solution[vector.Length - 1 - j];
                }
                solution.Add(x / matrix[i, vector.Length - solution.Count - 1]);
            }
            solution.Reverse();
            return solution;
        }

    }
}
