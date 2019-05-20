using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RegressionLib;

namespace CsvFileLibrary
{
    /// <summary>
    /// Working with *.csv files
    /// </summary>
    public class CsvFile
    {
        /// <summary>
        /// Empty contructor for saving *.csv file
        /// </summary>
        public CsvFile() { }
        /// <summary>
        /// Reads fields of the input file
        /// </summary>
        /// <param name="path"></param>
        public CsvFile(string path)
        {
            if (!Regex.IsMatch(path, @".+\.csv"))
            {
                throw new ArgumentException("Данный путь не указывает на файл *.csv");
            }

            using (StreamReader s = new StreamReader(path, Encoding.UTF8))
            {
                Header = s.ReadLine().Split(new[] { ',' },
                        StringSplitOptions.RemoveEmptyEntries);
                string line;
                Fields = new List<string>[Header.Length];
                for (int i = 0; i < Header.Length; i++)
                {
                    Fields[i] = new List<string>();
                }

                while ((line = s.ReadLine()) != null)
                {
                    List<string> splittedLine = Regex.Split(line, ",", RegexOptions.ExplicitCapture).ToList();
                    for (int i = 0; i < splittedLine.Count; i++)
                    {
                        Fields[i].Add(splittedLine[i]);
                    }
                }
                Length = Fields[0].Count;
            }
        }
        /// <summary>
        ///  Saves the regression output file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="regressions"></param>
        public void Save(string path, List<RegressionContainer> regressions)
        {
            using (StreamWriter s = new StreamWriter(path))
            {
                s.WriteLine(regressions[0].CompanyName+";");
                s.WriteLine("Dates;" + string.Join(";", regressions.ConvertAll(rc => rc.Regression.ToString())));
                s.WriteLine("Approximation Error;" + string.Join(";", regressions.ConvertAll(rc => rc.Regression.approximationError.ToString("f4"))));
                s.WriteLine("Correlation Coefficient;" + string.Join(";", regressions.ConvertAll(rc => rc.Regression.correlationCoefficient.ToString("f4"))));
                DateTime date = GetDateTime(regressions[0].Dates[0]);
                for (int i = 0; i < regressions[0].PredictDays; i++)
                {
                    s.WriteLine(date.Date + ";" +
                        string.Join(";", regressions.ConvertAll(rc => rc.Regression.PredictY(i + regressions[0].Regression.yVals.Length).ToString("f4"))));
                    date = date.AddDays(1);
                }
            }
        }
        /// <summary>
        ///  Fields of the *.csv file
        /// </summary>
        public List<string>[] Fields { get; set; }
        /// <summary>
        /// Header of the *.csv file
        /// </summary>
        public string[] Header { get; set; }
        /// <summary>
        /// Amount of lines in the *.csv file
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// Used for getting DateTime object from usual API response
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private DateTime GetDateTime(string date)
        {
            int[] ymd = Array.ConvertAll(date.Split('-'), s => int.Parse(s));
            return new DateTime(ymd[0], ymd[1], ymd[2]);
        }
    }
}