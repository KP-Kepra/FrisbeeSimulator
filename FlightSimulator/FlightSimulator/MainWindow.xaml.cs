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
                                       /* Default */
                                       0.00E+00,        //X Position
                                       0.00E+00,        //Y Position
                                       0.40E+00,        //Z Position
                                       1.00E+01,        //X Velocity
                                      -3.00E+00,        //Y Velocity, Put negative because inversed
                                       1.00E+00,        //Z Velocity
                                       5.11E-01,        //Phi Velocity
                                      -5.00E-01,        //Theta Velocity
                                      1.49E+01,        //Phi Acc
                                      -0.48E+00,        //Theta Acc
                                      5.43E+01,         //Gamma Acc
                                      5.03E+00          //Gamma
                                  };

        public double[] m_y0;

        private readonly Frisbee m_frisbee = new Frisbee();

        public int view = 1;

        public MainWindow()
        {
            InitializeComponent();

            m_y0 = new double[12];

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
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"D:\temp\test.txt"))
                {
                    for (int i = 0; i < y.GetLength(0); i++)
                    {
                        //Y axis on horizontal, X axis on vertical
                        if (y[i, 3] >= 0)
                        {
                            switch (view)
                            {
                                case 1:
                                    m_graph.AddPoint(-y[i, 2], y[i, 1], Colors.Blue);
                                    break;

                                case 2:
                                    m_graph.AddPoint(y[i, 1], y[i, 3], Colors.Red);
                                    break;

                                case 3:
                                    m_graph.AddPoint(-y[i, 2], y[i, 3], Colors.Green);
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
            }
            catch (Exception ex)
            {

                //MessageBox.Show(ex.ToString());
            }
        }

        public class tickJSON
        {
            public string Tick { get; set; }
            public PointJSON Points { get; set; }
        }

        public class PointJSON
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
        }
        
        private void writeJSON(double[,] y)
        {
            string output;
            List<tickJSON> List = new List<tickJSON>();

            for (int a = 0; a < y.GetLength(0); a++)
            {
                if (y[a, 3] >= 0)
                {
                    tickJSON tick = new tickJSON();
                    tick.Tick = a.ToString();

                    PointJSON Point = new PointJSON();
                    Point.X = y[a, 1];
                    Point.Y = -y[a, 2];
                    Point.Z = y[a, 3];

                    tick.Points = Point;
                    List.Add(tick);
                }
                else break;
            }

            output = JsonConvert.SerializeObject(List, Formatting.Indented);
            string dir = Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName).Parent.FullName, @"js\coor-map\CoordinateMapper\private\frisbeeValues.json");
            File.WriteAllText(dir, output);
        }

        private void OnGraphSizeChanged(object sender, SizeChangedEventArgs e)
        {
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
                        m_y0[6] = slider.Value;
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