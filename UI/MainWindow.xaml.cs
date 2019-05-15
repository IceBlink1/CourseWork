using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.IO;
using RegressionLib;
using CsvFileLibrary;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;

namespace UI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Dictionary for serialization as Brush is Nonserializable 
        /// </summary>
        private Dictionary<int, Brush> colorValuePairs = new Dictionary<int, Brush>()
        {
            {0, Brushes.Gold},
            {1, Brushes.Coral},
            {2, Brushes.DeepPink },
            {3, Brushes.ForestGreen },
            {4, Brushes.MidnightBlue }
        };
        /// <summary>
        /// 
        /// </summary>
        private List<CheckBox> checkBoxes = new List<CheckBox>();
        /// <summary>
        /// true if selection changed, false otherwise
        /// </summary>
        private bool companyChanged = true;
        /// <summary>
        /// request to the alphavantage api
        /// </summary>
        private readonly string request = @"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&outputsize=full&symbol=";
        /// <summary>
        /// alphavantage api token
        /// </summary>
        private readonly string token = "YGB65RZ0SIRT8U5R";
        /// <summary>
        /// shown regressions list
        /// </summary>
        private List<RegressionContainer> regressions = new List<RegressionContainer>();

        /// <summary>
        /// reading listbox content from companies.txt
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            using (StreamReader s = new StreamReader(@"..\..\..\companies.txt"))
            {
                while (!s.EndOfStream)
                {
                    CompanyListBox.Items.Add(Regex.Match(s.ReadLine(), "[A-Z]+").ToString());
                }
            }
            List<string> RegressionList = new List<string>() { "Exponential", "Logarifmic", "Hyperbolic", "Square", "Cubic" };
            checkBoxes.Add(ExponentialCheck);
            checkBoxes.Add(LogarifmicCheck);
            checkBoxes.Add(HyperbolicCheck);
            checkBoxes.Add(SquareCheck);
            checkBoxes.Add(CubicCheck);
        }

        public CsvFile CsvFile
        {
            get => default(CsvFile);
            set
            {
            }
        }

        public RegressionContainer RegressionContainer
        {
            get => default(RegressionContainer);
            set
            {
            }
        }
        /// <summary>
        /// Downloading data, making regression list, drawing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            double[] xVals;
            double[] yVals;
            CsvFile file;
            string name;
            try
            {
                name = CompanyListBox.SelectedItem.ToString();
                GetVals(out xVals, out yVals, out file, name);
            }
            catch (Exception)
            {
                return;
            }
            int baseDays = 0, predictDays = 0;
            if (!TryParseTextBoxes(ref baseDays, ref predictDays))
            {
                MessageBox.Show("Неверный ввод данных для предсказания");
                return;
            }
            baseDays = Math.Min(baseDays, file.Fields[0].Count);
            Array.Resize(ref xVals, baseDays);

            double[] yValsConv = new double[baseDays];
            for (int i = Math.Max(yVals.Length - baseDays, 0); i < yVals.Length; i++)
            {
                yValsConv[i - yVals.Length + baseDays] = yVals[i];
            }

            try
            {
                GetRegressionsList(xVals, yValsConv, file, predictDays, name);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Не удалось предсказать котировки по скачанным данным, попробуйте другую компанию");
                ClearButton_Click(this, new RoutedEventArgs());
                return;
            }

            if (OutputCheck.IsChecked == true)
            {
                SaveFileDialog fileDialog = new SaveFileDialog
                {
                    Filter = "Comma separated file (*.csv)|*.csv"
                };
                if (fileDialog.ShowDialog() == true)
                {
                    file.Save(fileDialog.FileName, regressions);
                }
            }

            TopXLabel.Content = file.Fields[0][0];
            BottomXLabel.Content = file.Fields[0][baseDays - 1];
            MiddleXLabel.Content = file.Fields[0][baseDays / 2];

            if (regressions.Count != 0)
            {
                DrawPoints(regressions[regressions.Count - 1]);
                double bestCorreliation = regressions.Max(rc => rc.Regression.correlationCoefficient);
                AbstractRegression bestRegression =
                    regressions.Where(rc => Math.Abs(rc.Regression.correlationCoefficient - bestCorreliation)
                    < double.Epsilon).ToList()[0].Regression;
                MessageBox.Show($"Основываясь на лучшей модели предсказания: {bestRegression.ToString()} с коэффициентом корреляции {bestCorreliation:f5}, " +
                    $"вы можете заработать ${-bestRegression.yVals[bestRegression.yVals.Length - 1] + bestRegression.PredictY(xVals.Length + 2):f5} за каждую акцию, если вложитесь сейчас и продадите акции через" +
                    $" {predictDays} дней");
            }
        }

        /// <summary>
        /// Downloading and parsing data from API
        /// </summary>
        /// <param name="xVals"></param>
        /// <param name="yVals"></param>
        /// <param name="file"></param>
        /// <param name="name"></param>
        private void GetVals(out double[] xVals, out double[] yVals, out CsvFile file, string name)
        {
            try
            {
                string path = $"..\\..\\..\\data\\{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}-{name}.csv";
                if (companyChanged)
                {
                    if (!File.Exists(path))
                    {
                        WebClient webClient = new WebClient();
                        webClient.DownloadFile(request + name
                            + $"&apikey={token}&datatype=csv", path);
                    }
                    ClearButton_Click(this, new RoutedEventArgs());
                }
                file = new CsvFile(path);
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }

            try
            {
                xVals = new double[file.Fields[0].Count];
                yVals = file.Fields[1].Select(s => double.Parse(s.Replace('.', ','))).Reverse().ToArray();
                for (int i = 0; i < xVals.Length; i++)
                {
                    xVals[i] = i + 2;
                }
            }
            catch (IndexOutOfRangeException)
            {
                MessageBox.Show("Неверный ввод данных");
                throw;
            }
        }

        /// <summary>
        /// Parsing textboxes' values
        /// </summary>
        /// <param name="baseDays"></param>
        /// <param name="predictDays"></param>
        /// <returns></returns>
        private bool TryParseTextBoxes(ref int baseDays, ref int predictDays)
        {
            return int.TryParse(BaseTextBox.Text, out baseDays) && int.TryParse(PredictTextBox.Text, out predictDays);
        }

        /// <summary>
        /// Initializing regression list
        /// </summary>
        /// <param name="xVals"></param>
        /// <param name="yValsConv"></param>
        /// <param name="file"></param>
        /// <param name="predictDays"></param>
        /// <param name="name"></param>
        private void GetRegressionsList(double[] xVals, double[] yValsConv, CsvFile file, int predictDays, string name)
        {
            regressions.Clear();
            PlotCanvas.Children.Clear();
            AbstractRegression regression;
            Brush brush = null;
            if (ExponentialCheck.IsChecked == true)
            {
                regression = new ExponentialRegression(xVals, yValsConv);
                brush = Brushes.Gold;
                regressions.Add(new RegressionContainer(regression, predictDays, 0, file.Fields[0], name));//TODO: make enum for brush keys
                DrawPlot(regressions[regressions.Count - 1]);
            }
            if (LogarifmicCheck.IsChecked == true)
            {
                regression = new LogarifmicRegression(xVals, yValsConv);
                brush = Brushes.Coral;
                regressions.Add(new RegressionContainer(regression, predictDays, 1, file.Fields[0], name));
                DrawPlot(regressions[regressions.Count - 1]);
            }

            if (HyperbolicCheck.IsChecked == true)
            {
                brush = Brushes.DeepPink;
                regression = new HyperbolicRegression(xVals, yValsConv);
                regressions.Add(new RegressionContainer(regression, predictDays, 2, file.Fields[0], name));
                DrawPlot(regressions[regressions.Count - 1]);
            }
            if (SquareCheck.IsChecked == true)
            {
                brush = Brushes.ForestGreen;
                regression = new SquareRegression(xVals, yValsConv);

                regressions.Add(new RegressionContainer(regression, predictDays, 3, file.Fields[0], name));
                DrawPlot(regressions[regressions.Count - 1]);
            }
            if (CubicCheck.IsChecked == true)
            {
                brush = Brushes.MidnightBlue;
                regression = new CubicRegression(xVals, yValsConv);
                regressions.Add(new RegressionContainer(regression, predictDays, 4, file.Fields[0], name));
                DrawPlot(regressions[regressions.Count - 1]);
            }
        }

        /// <summary>
        /// Drawing plot of a given regression
        /// </summary>
        /// <param name="container"></param>
        private void DrawPlot(RegressionContainer container)
        {
            AbstractRegression regression = container.Regression;
            Brush brush = colorValuePairs[container.BrushKeyValue];
            int predictDays = container.PredictDays;
            double min = regression.yVals.Min();
            double max = regression.yVals.Max();
            TopYLabel.Content = $"{max:f2}";
            BottomYLabel.Content = $"{min:f2}";
            double height = max - min;
            MiddleYLabel.Content = $"{(min + max) / 2.0:f2}";
            double predictCoefficient = 1 + (double)predictDays / regression.xVals.Length;
            double width = regression.xVals.Length;
            double heightCoefficient = PlotCanvas.ActualHeight / (height * predictCoefficient);
            double widthCoefficient = PlotCanvas.ActualWidth / (width * predictCoefficient);
            Line line = new Line
            {
                Y2 = Math.Min(PlotCanvas.ActualHeight,
                    PlotCanvas.ActualHeight - heightCoefficient * (regression.PredictY(0.1) - min)),
                X2 = 0.1 * widthCoefficient,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                StrokeThickness = 1.25,
                Stroke = brush
            };


            for (double i = 0.1; i < regression.xVals.Length * predictCoefficient; i += 0.5)
            {
                double lastY = line.Y2;
                double lastX = line.X2;
                line = new Line
                {
                    Y1 = lastY,
                    X1 = lastX,
                    Y2 = Math.Min(PlotCanvas.ActualHeight,
                    PlotCanvas.ActualHeight - heightCoefficient * (regression.PredictY(i) - min)),
                    X2 = i * widthCoefficient,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    StrokeThickness = 1.25,
                    Stroke = brush
                };

                PlotCanvas.Children.Add(line);
            }
        }

        /// <summary>
        /// Drawing source values
        /// </summary>
        /// <param name="container"></param>
        private void DrawPoints(RegressionContainer container)
        {
            AbstractRegression regression = container.Regression;
            int predictDays = container.PredictDays;
            double min = regression.yVals.Min();
            double max = regression.yVals.Max();
            TopYLabel.Content = $"{max:f2}";
            BottomYLabel.Content = $"{min:f2}";
            double height = max - min;
            MiddleYLabel.Content = $"{(min + max) / 2.0:f2}";
            double predictCoefficient = 1 + (double)predictDays / regression.xVals.Length;
            double width = regression.xVals.Length;
            double heightCoefficient = PlotCanvas.ActualHeight / (height * predictCoefficient);
            double widthCoefficient = PlotCanvas.ActualWidth / (width * predictCoefficient);
            for (int i = 0; i < regression.xVals.Length; i++)
            {
                Rectangle point = new Rectangle
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Stroke = Brushes.Red,
                    Width = 2,
                    Height = 2,
                    Margin = new Thickness(regression.xVals[i] * widthCoefficient,
                    PlotCanvas.ActualHeight - heightCoefficient * (regression.yVals[i] - min),
                    regression.xVals[i] * widthCoefficient,
                    PlotCanvas.ActualHeight - heightCoefficient * (regression.yVals[i] - min))
                };

                PlotCanvas.Children.Add(point);

            }
            companyChanged = false;
        }

        /// <summary>
        /// Clearing canvas 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            MiddleXLabel.Content = "";
            MiddleYLabel.Content = "";
            TopXLabel.Content = "";
            TopYLabel.Content = "";
            BottomYLabel.Content = "";
            BottomXLabel.Content = "";
            PlotCanvas.Children.Clear();
            regressions.Clear();
            GC.Collect();
        }

        /// <summary>
        /// Changing companyChanged to true when listbox selection changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompanyListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            companyChanged = true;
        }

        /// <summary>
        /// Resizing canvas elements when program is resized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double oldWidth = e.PreviousSize.Width;
            double oldHeight = e.PreviousSize.Height;
            double newHeight = e.NewSize.Height;
            double newWidth = e.NewSize.Width;
            double widthCoef = newWidth / oldWidth;
            double heightCoef = newHeight / oldHeight;
            for (int i = 0; i < PlotCanvas.Children.Count; i++)
            {
                if (PlotCanvas.Children[i] is Line)
                {
                    Line line = PlotCanvas.Children[i] as Line;
                    line.Y1 *= heightCoef;
                    line.Y2 *= heightCoef;
                    line.X1 *= widthCoef;
                    line.X2 *= widthCoef;
                    PlotCanvas.Children[i] = line;
                }
                if (PlotCanvas.Children[i] is Rectangle)
                {
                    Rectangle rectangle = PlotCanvas.Children[i] as Rectangle;
                    rectangle.Margin = new Thickness(rectangle.Margin.Left * widthCoef, rectangle.Margin.Top * heightCoef,
                        rectangle.Margin.Right * widthCoef, rectangle.Margin.Bottom * heightCoef);
                    PlotCanvas.Children[i] = rectangle;
                }

            }
            XAxis.X2 += newWidth - oldWidth;
            YAxis.Y1 += newHeight - oldHeight;
            YAxis.Height += newHeight - oldHeight;
            XAxis.Width += newWidth - oldWidth;
        }

        /// <summary>
        /// Opening a *.rgrs file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = "Regression file (*.rgrs)|*.rgrs"
            };
            if (fileDialog.ShowDialog() == true)
            {
                try
                {
                    using (FileStream fs = new FileStream(fileDialog.FileName, FileMode.Open))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        regressions = (List<RegressionContainer>)bf.Deserialize(fs);
                    }
                }
                catch (SerializationException)
                {
                    MessageBox.Show("Сериалиазция не удалась");
                    ClearButton_Click(this, new RoutedEventArgs());
                    return;
                }
                PlotCanvas.Children.Clear();
                if (regressions.Count == 0)
                {
                    MessageBox.Show("Выходной файл пуст");
                    return;
                }
                foreach (RegressionContainer item in regressions)
                {
                    DrawPlot(item);
                }
                RegressionContainer rc = regressions[0];
                DrawPoints(rc);
                int baseDays = rc.Regression.yVals.Length;
                TopXLabel.Content = rc.Dates[0];
                BottomXLabel.Content = rc.Dates[baseDays - 1];
                MiddleXLabel.Content = rc.Dates[baseDays / 2];
                BaseTextBox.Text = rc.Regression.xVals.Length.ToString();
                PredictTextBox.Text = rc.PredictDays.ToString();
                CompanyListBox.SelectedItem = CompanyListBox.Items[CompanyListBox.Items.IndexOf(rc.CompanyName)];
                UncheckAll();
                for (int i = 0; i < regressions.Count; i++)
                {
                    if (regressions[i].Regression is ExponentialRegression)
                    {
                        ExponentialCheck.IsChecked = true;
                        continue;
                    }
                    if (regressions[i].Regression is LogarifmicRegression)
                    {
                        LogarifmicCheck.IsChecked = true;
                        continue;
                    }
                    if (regressions[i].Regression is HyperbolicRegression)
                    {
                        HyperbolicCheck.IsChecked = true;
                        continue;
                    }
                    if (regressions[i].Regression is SquareRegression)
                    {
                        SquareCheck.IsChecked = true;
                        continue;
                    }
                    if (regressions[i].Regression is CubicRegression)
                    {
                        CubicCheck.IsChecked = true;
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Saving a *.rgrs file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog
            {
                Filter = "Regression file (*.rgrs)|*.rgrs"
            };
            if (fileDialog.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(fileDialog.FileName, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, regressions);
                }
            }
        }

        /// <summary>
        /// Unchecking all checks in the checkbox
        /// </summary>
        private void UncheckAll()
        {
            for (int i = 0; i < checkBoxes.Count; i++)
            {
                checkBoxes[i].IsChecked = false;
            }
        }

        /// <summary>
        /// Showing info about creator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Разработчик: Лютиков Александр Сергеевич\n" +
                "Email: aslyutikov@edu.hse.ru");
        }
    }
}
