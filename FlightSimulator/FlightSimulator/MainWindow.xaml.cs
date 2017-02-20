using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace FlightSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double[] m_y0Initials = new double[]
                                  {
                                      /*Well duh
                                      -.903,
                                       -.633,
                                       -.913,
                                      13.4,
                                      -4.11,
                                       0.00112,
                                      -0.0711,
                                       0.211,
                                     -14.9,
                                      -1.48,
                                      54.3,
                                       5.03,*/
                                      -9.03E-01,
                                      -6.33E-01,
                                      -9.13E-01,
                                      1.34E+01,
                                      -4.11E-01,
                                      1.12E+01,
                                      -7.11E-02,
                                      2.11E-01,
                                      -1.49E+01,
                                      -1.48E+00,
                                      5.43E+01,
                                      5.03E+00
                                  };

        public double[] m_y0;

        private readonly Frisbee m_frisbee = new Frisbee();

        public int view = 1;

        public MainWindow()
        {
            InitializeComponent();

            m_y0 = new double[m_y0Initials.Length];

            Reset();
        }

        private void Reset()
        {
            for (int i = 0; i < m_y0Initials.Length; i++)
            {
                m_y0[i] = m_y0Initials[i];
            }
            m_graph.Reset();
            ExecuteSimulation(m_y0);
            m_graph.Refresh();

        }

        private void OnGraphLoaded(object sender, RoutedEventArgs e)
        {

            ExecuteSimulation(m_y0);

            /* List<Point> simulate = m_frisbee.Simulate(1, 14, 0, 90, 0.001);
            m_graph.AddPoints(simulate);
            m_graph.Refresh();*/
        }

        private void ExecuteSimulation(double[] y0)
        {
            m_graph.Reset();
            try
            {
                //double tfinal = 1.46; //% length of flight
                double tfinal = 5; //% length of flight
                double nsteps = 292;// % number of time steps for data

                double span = tfinal/nsteps;

                double[] x = new double[(int)nsteps];
                for (int i = 0; i < nsteps; i++)
                {
                    x[i] = span*i;
                }

                double[,] y = m_frisbee.Ode(y0, x);
                double z0 = 1; //Initial Height
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"D:\temp\test.txt"))
                {
                    for (int i = 0; i < y.GetLength(0); i++)
                    {
                        //Y axis on horizontal, X axis on vertical
                        if (y[i,3] + z0 >= 0)
                        {
                            switch (view)
                            {
                                case 1:
                                    m_graph.AddPoint(y[i, 1], -y[i, 2], Colors.Blue);
                                    break;

                                case 2:
                                    m_graph.AddPoint(y[i, 1], y[i, 3], Colors.Red);
                                    break;

                                case 3:
                                    m_graph.AddPoint(y[i, 2], y[i, 3], Colors.Green);
                                    break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                Dispatcher.BeginInvoke(new Action(() => m_graph.Refresh()));
                writeJSON(y);
                m_graph.GetArray(y);
                //m_frisbee.Simulate(new Frisbee.SimulationState
                //{
                //    VX = 1.34E+01,
                //    VY = -4.11E-01,
                //    VZ = 1.12E-03,
                //    Phi = -7.11E-02,
                //    Theta = 2.11E-01,
                //    PhiDot = -1.49E+01,
                //    ThetaDot = -1.48E+00,
                //    GammaDot = 5.43E+01,
                //});
            }
            catch (Exception ex)
            {

                //MessageBox.Show(ex.ToString());
            }
        }

        private void writeJSON(double[,] y)
        {
            string output = "";
            string output_raw = "";
            double[,] pos = new double[y.GetLength(0), 3];
            string[] line = new string[y.GetLength(0)];

            for (int a = 0; a < y.GetLength(0); a++)
            {
                for (int b = 0; b < 3; b++)
                {
                    pos[a, b] = y[a, b + 1];
                    line[a] += pos[a, b] + "%";
                }
                output_raw = output_raw + line[a] + System.Environment.NewLine;
            }
            output = JsonConvert.SerializeObject(output_raw);
            output = output.Replace(@"\r\n", System.Environment.NewLine).Replace(@"''", System.Environment.NewLine);
            File.WriteAllText(@"...\frisbeeValues.json", output);
        }

        private void OnGraphSizeChanged(object sender, SizeChangedEventArgs e)
        {
            m_graph.Refresh();
        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            m_y0[3] = sliderVx.Value;
            m_y0[4] = -sliderVy.Value;
            m_y0[5] = sliderVz.Value;
            m_y0[6] = sliderPhi.Value;
            m_y0[7] = sliderTheta.Value;
            ExecuteSimulation(m_y0);
            m_graph.Refresh();
        }

        private bool dragStarted = false;

        private void sliderDragChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStarted)
            {
                sliderUpdate(sender, e.NewValue);
            }
        }

        private void sliderDragStart(object sender, DragStartedEventArgs e)
        {
            this.dragStarted = true;
        }

        private void sliderDragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.dragStarted = false;
            Slider slider = sender as Slider;
            sliderUpdate(sender, slider.Value);
        }

        private void sliderUpdate (object sender, double val)
        {
            Slider slider = sender as Slider;
            try
            {
                if ((slider != null) && (!dragStarted))
                {
                    double value = slider.Value;

                    if (slider.Name == "sliderVx")
                    {
                        m_y0[3] = slider.Value;
                    }
                    else if (slider.Name == "sliderVy")
                    {
                        m_y0[4] = -slider.Value;
                    }
                    else if (slider.Name == "sliderVz")
                    {
                        m_y0[5] = slider.Value;
                    }
                    else if (slider.Name == "sliderPhi")
                    {
                        m_y0[6] = -slider.Value;
                    }
                    else if (slider.Name == "sliderTheta")
                    {
                        m_y0[7] = slider.Value;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            ExecuteSimulation(m_y0);
            m_graph.Refresh();
        }

        private void buttonReset_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void viewTop(object sender, RoutedEventArgs e)
        {
            view = 1;
            ExecuteSimulation(m_y0);
            m_graph.Refresh();
        }
        private void viewSide(object sender, RoutedEventArgs e)
        {
            view = 2;
            ExecuteSimulation(m_y0);
            m_graph.Refresh();
        }
        private void viewFront(object sender, RoutedEventArgs e)
        {
            view = 3;
            ExecuteSimulation(m_y0);
            m_graph.Refresh();
        }
    }
}