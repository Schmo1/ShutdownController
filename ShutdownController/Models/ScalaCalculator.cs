using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts;

namespace ShutdownController.Models
{
    internal class ScalaCalculator
    {

        private List<ChartValues<double>> charts;
        private List<double> maxValues = new List<double>();
        private double maxValueOld = 0;
        private int _numberOfSteps = 5;

        public int NumberOfSteps { get { return _numberOfSteps; } }


        public ScalaCalculator(List<ChartValues<double>> charts, int steps)
        {
            this.charts = charts;
            this._numberOfSteps = steps;
        }

        public ScalaCalculator(List<ChartValues<double>> charts)
        {
            this.charts = charts;
        }

        public int GetChartMax(out int steps)
        {
            maxValues.Clear();
            foreach (ChartValues<double> chart in charts)
            {
                maxValues.Add(chart.Max());
            }

            double chartMax = Math.Ceiling(maxValues.Max());

            if (maxValueOld == 0) //first value
            {
                 maxValueOld = chartMax;
            }
            else if (chartMax > maxValueOld) // Values are bigger then chart
            {
                maxValueOld = Math.Round(chartMax + 1.1);
            }
            else if (chartMax < (maxValueOld * 0.8))
            {
                maxValueOld = Math.Round(chartMax * 1.1);
            }

            steps = (int)maxValueOld / _numberOfSteps;

            if(steps == 0)
                steps = 1;

            if ((int)maxValueOld == 0)
                return 1;

            return (int)maxValueOld;

        }



    }
}
