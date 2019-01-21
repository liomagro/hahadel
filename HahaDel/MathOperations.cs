using InteractiveDataDisplay.WPF;
using MathNet.Numerics.IntegralTransforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HahaDel
{
    class MathOperations
    {
        /// <summary>
        /// Playing with fourier transformation
        /// </summary>
        public void DoSomeFourier(List<float[]> inSoundData, List<float[]> outSoundData)
        {          
            var data = inSoundData[1];
            var complex = new Complex[data.Length];
            for (int i = 0; i < data.Length; i++)
                complex[i] = new Complex(data[i], 0);


            Fourier.Inverse(complex);

            var app = new Application();
            var window = new Window();
            var grid = new Grid();
            RowDefinition rowDef1 = new RowDefinition();
            RowDefinition rowDef2 = new RowDefinition();
            RowDefinition rowDef3 = new RowDefinition();
            RowDefinition rowDef4 = new RowDefinition();
            grid.RowDefinitions.Add(rowDef1);
            grid.RowDefinitions.Add(rowDef2);
            grid.RowDefinitions.Add(rowDef3);
            grid.RowDefinitions.Add(rowDef4);



            var chart1 = new Chart();
            Grid.SetRow(chart1, 0);
            var barChart1 = new BarGraph();
            chart1.Content = barChart1;

            double[] y1 = new double[complex.Length];          

            for (int i = 0; i < complex.Length; i++)
            {
                y1[i] = data[i];
                //y1[i] = complex[i].Magnitude;
                //y1[i] = Math.Sin(((double)i) / complex.Length * 2 * Math.PI);
            }
            barChart1.PlotBars(y1);
            grid.Children.Add(chart1);


            var chart2 = new Chart();
            Grid.SetRow(chart2, 1);
            var barChart2 = new BarGraph();
            chart2.Content = barChart2;

            double[] y2 = new double[complex.Length];

            for (int i = 0; i < complex.Length; i++)
            {
                //y1[i] = complex[i].Magnitude;
                y2[i] = 1.0;
            }
            barChart2.PlotBars(y2);
            grid.Children.Add(chart2);
                        
            window.Content = grid;
            app.Run(window);
        }


        /// <summary>
        /// Make fourier forward and backward - result must be same
        /// </summary>
        /// <param name="inSoundData"></param>
        /// <returns></returns>
        public List<float[]> DoFourierForthAndBack(List<float[]> inSoundData)
        {

            var res = new List<float[]>();

            for(int j = 0; j < inSoundData.Count; j++)
            {
                var data = inSoundData[j];
                var complex = new Complex[data.Length];
                for (int i = 0; i < data.Length; i++)
                    complex[i] = new Complex(data[i], 0);
                Fourier.Forward(complex);
                Fourier.Inverse(complex);

                res.Add(complex.Select(t => (float)t.Real).ToArray());
            }
            return res;
        }
    }
}
