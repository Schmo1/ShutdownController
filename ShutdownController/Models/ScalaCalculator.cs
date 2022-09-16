using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts;
using ShutdownController.Utility;

namespace ShutdownController.Models
{
    internal class ScalaCalculator
    {

        private List<ChartValues<double>> charts;
        private List<double> maxValues = new List<double>();
        private double maxValueOld = 0;
        private double _numberOfSteps = 5;
        private const double _upperHysteresis = 1.1;
        private const double _lowerHysteresis = 0.8;

        public int NumberOfSteps { get { return (int)_numberOfSteps; } }


        public ScalaCalculator(List<ChartValues<double>> charts, int steps)
        {
            this.charts = charts;
            this._numberOfSteps = steps;
        }

        public ScalaCalculator(List<ChartValues<double>> charts)
        {
            this.charts = charts;
        }

        public int GetChartUpperLine(out int steps) //Returns the max upper line
        {
            try
            {
                maxValues.Clear();
                foreach (ChartValues<double> chart in charts)
                {
                    maxValues.Add(chart.Max());
                }

                double chartMax = Math.Ceiling(maxValues.Max()); //Round up

                if (maxValueOld == 0) //first value
                {
                     maxValueOld = chartMax;
                }
                else if (chartMax > maxValueOld) // Values are bigger then chart => adjust Chartvalue
                {
                    maxValueOld = Math.Round(chartMax + _upperHysteresis);
                }
                else if (chartMax < (maxValueOld * _lowerHysteresis)) //Value are lower then current chart max => adjust Chartvalue
                {
                    maxValueOld = Math.Round(chartMax * _upperHysteresis);
                }
            }
            catch (Exception e)
            {
                MyLogger.Instance().Error("Error on returning the upper chart line Exception: " + e.Message);
                
            }

            steps = (int)Math.Ceiling(maxValueOld / _numberOfSteps);

            if(steps == 0)
                steps = 1;

            if ((int)maxValueOld == 0)
                return 1;

            return (int)maxValueOld;

        }

    }
}
