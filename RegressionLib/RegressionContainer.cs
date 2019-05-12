using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RegressionLib
{
    /// <summary>
    /// Used as a container for serialization in MainWindow
    /// </summary>
    [Serializable]
    public class RegressionContainer
    {
        /// <summary>
        /// Empty constructor for serialization purposes
        /// </summary>
        public RegressionContainer()
        {

        }

        /// <summary>
        /// Initializing properties
        /// </summary>
        /// <param name="regression"></param>
        /// <param name="predictDays"></param>
        /// <param name="brushKeyValue"></param>
        /// <param name="dates"></param>
        /// <param name="CompanyName"></param>
        public RegressionContainer(AbstractRegression regression, int predictDays, int brushKeyValue, List<string> dates, string CompanyName)
        {
            Regression = regression;
            PredictDays = predictDays;
            BrushKeyValue = brushKeyValue;
            Dates = dates;
            this.CompanyName = CompanyName; 
        }

        /// <summary>
        /// Amount of days for which prediction is made
        /// </summary>
        public int PredictDays { get; set; }
        /// <summary>
        /// Key value of regression brush in Dictionary in MainWindow
        /// </summary>
        public int BrushKeyValue { get; set; }
        /// <summary>
        /// Regression object
        /// </summary>
        public AbstractRegression Regression { get; set; }
        /// <summary>
        /// Dates, based on which prediction of made
        /// </summary>
        public List<string> Dates { get; set; }
        /// <summary>
        /// Token of a company on NASDAQ
        /// </summary>
        public string CompanyName { get; set; }
    }
}
