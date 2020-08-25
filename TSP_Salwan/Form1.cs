using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Windows;
using System.Media;



namespace TSP_Salwan
{
    public partial class Form1 : Form
    {
        List<Point> m_Points = new List<Point>();
        List<Point> HPts = null;
        Graphics g; Pen p; Point cursor; SolidBrush b, b2;
        int k = 0; Point[] points = new Point[1000];
        Bitmap img;
        int Maincounter = 0;
        Bitmap newimg;
        int[,] DistanceArrayOfCitys;
        string Time_MyAlgorthem, Time_NeighborAlgorithm, Dist_MyAlgorthem, Dist__NeighborAlgorithm;
        bool firstCity = true;

        public Form1()
        {
            InitializeComponent();
            g = pictureBox1.CreateGraphics();
            p = new Pen(Color.Red, 3);
            b = new SolidBrush(Color.Green);
            b2 = new SolidBrush(Color.Blue);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "jpg (*.jpg)|*.jpg|bmp (*.bmp)|*.bmp|png (*.png)|*.png";

                if (ofd.ShowDialog() == DialogResult.OK && ofd.FileName.Length > 0)
                {
                    pictureBox1.Image = Image.FromFile(ofd.FileName);
                }
                img = new Bitmap(pictureBox1.Image, pictureBox1.Width, pictureBox1.Height);
                pictureBox1.Image = img;
                newimg = new Bitmap(img);
                resite();
            }
            catch { }

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            cursor = pictureBox1.PointToClient(Cursor.Position);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                using (Graphics g = Graphics.FromImage(img))
                {
                    if (!firstCity)
                        b.Color = Color.Red;

                    g.FillEllipse(b2, cursor.X - 12, cursor.Y - 12, 14, 14);
                    g.FillEllipse(b, cursor.X - 10, cursor.Y - 10, 10, 10);
                    //g.DrawString($"({cursor.X},{cursor.Y})", new Font("Arial", 13), b, new Point(cursor.X, cursor.Y));
                }

                m_Points.Add(new Point(e.X, e.Y));

                if (m_Points.Count >= 3)
                    HPts = Geometry.MakeConvexHull(m_Points);

                points[k++] = new Point(cursor.X, cursor.Y);
                Maincounter += 1;
                pictureBox1.Image = img;
                firstCity = false;
            }
            catch
            {
                MessageBox.Show("Plase open map first!");
            }

        }


        const int TURN_LEFT = 1;
        const int TURN_RIGHT = -1;
        const int TURN_NONE = 0;

        public int turn(Point p, Point q, Point r)
        {
            return ((q.X - p.X) * (r.Y - p.Y) - (r.X - p.X) * (q.Y - p.Y)).CompareTo(0);
        }

        public int dist(Point p, Point q)
        {
            int dx = q.X - p.X;
            int dy = q.Y - p.Y;
            return Convert.ToInt32(Math.Sqrt(dx * dx + dy * dy));
        }

        public Point nextHullPoint(List<Point> points, Point p)
        {
            Point q = p;
            int t;
            foreach (Point r in points)
            {
                t = turn(p, q, r);
                if (t == TURN_RIGHT || t == TURN_NONE && dist(p, r) > dist(p, q))
                    q = r;
            }
            return q;
        }




        public double getAngle(Point p1, Point p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            return Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
        }




        List<Point> FindThePath(List<Point> Citylist)
        {
            int TheCounter = 0;
            List<Point> CityListNewPath = new List<Point>(Citylist);
            List<Point> Skiplist = new List<Point>(Citylist);





            for (int i = 0; i < Citylist.Count; i++)
            {
                points = new Point[CityListNewPath.Count];
                CityListNewPath.CopyTo(points);

                Pen p = new Pen(Color.Red, 2);
                Bitmap pic = new Bitmap(img);
                using (Graphics g = Graphics.FromImage(pic))
                {
                    g.DrawPolygon(p, points);
                }
                pictureBox1.Image = new Bitmap(pic);
                pictureBox1.Refresh();
                Thread.Sleep(100);


                bool IsFirstPoint = true;
                int d1, d2, d3;


                if (i == Citylist.Count - 1)
                {
                    d1 = dist(Citylist[i], Citylist[0]);
                }
                else
                {
                    d1 = dist(Citylist[i], Citylist[i + 1]);
                }

                Point NextPoint = new Point();

                double TrangleHeghit = 0;

                foreach (Point NewNextPoint in m_Points)
                {

                    double NewTrangleHeghit = 0;

                    bool skip = false;


                    for (int c = 0; c < Skiplist.Count; c++)
                    {

                        if (NewNextPoint == Skiplist[c])
                        {

                            skip = true;
                            break;
                        }
                    }

                    if (skip == true)
                        continue;

                    if (IsFirstPoint)
                    {
                        if (i == Citylist.Count - 1)
                            d3 = dist(Citylist[0], NewNextPoint);
                        else
                            d3 = dist(Citylist[i + 1], NewNextPoint);

                        d2 = dist(Citylist[i], NewNextPoint);

                        TrangleHeghit = (d2 + d3) - d1;

                        IsFirstPoint = false;
                        NextPoint = NewNextPoint;
                    }
                    else
                    {
                        if (i == Citylist.Count - 1)
                            d3 = dist(Citylist[0], NewNextPoint);
                        else
                            d3 = dist(Citylist[i + 1], NewNextPoint);

                        d2 = dist(Citylist[i], NewNextPoint);

                        NewTrangleHeghit = (d2 + d3) - d1;
                        if (NewTrangleHeghit < TrangleHeghit)
                        {
                            TrangleHeghit = NewTrangleHeghit;
                            NextPoint = NewNextPoint;
                        }

                    }
                }

                for (int j = 0; j < CityListNewPath.Count; j++)
                {
                    double TrangleHeghit2 = 0;


                    if (j == CityListNewPath.Count - 1)
                    {
                        TrangleHeghit2 = (dist(NextPoint, CityListNewPath[j]) + dist(NextPoint, CityListNewPath[0])) - dist(CityListNewPath[j], CityListNewPath[0]);
                    }
                    else
                    {
                        TrangleHeghit2 = (dist(NextPoint, CityListNewPath[j]) + dist(NextPoint, CityListNewPath[j + 1])) - dist(CityListNewPath[j], CityListNewPath[j + 1]);
                    }

                    if (TrangleHeghit2 < TrangleHeghit)
                    {
                        NextPoint = new Point(0, 0);
                        break;
                    }
                }

                if (NextPoint.X != 0 && NextPoint.Y != 0)
                {
                    CityListNewPath.Insert(i + 1 + TheCounter, NextPoint);
                    TheCounter++;
                    Skiplist.Add(NextPoint);
                }



            }

            return CityListNewPath;

        }
        private void button2_Click(object sender, EventArgs e)
        {
            Point[] points = new Point[HPts.Count];
            HPts.CopyTo(points);
            List<Point> PathPoint = points.ToList<Point>();

            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                
                while (PathPoint.Count < Maincounter)
                    PathPoint = FindThePath(PathPoint);
                watch.Stop();
                points = new Point[PathPoint.Count];
                PathPoint.CopyTo(points);

                Pen p = new Pen(Color.Red, 2);
                using (Graphics g = Graphics.FromImage(img))
                {
                    g.DrawPolygon(p, points);
                }

                Bitmap pic = new Bitmap(pictureBox1.Image);
                pictureBox1.Image = img;

            }
            catch
            {
                MessageBox.Show("Plase make sure you select point on map first!");
            }
            MessageBox.Show("Done!");
            
            int FainlDist = 0;
            for (int i = 0; i < PathPoint.Count; i++)
            {
                if (i == PathPoint.Count - 1)
                    FainlDist += dist(PathPoint[i], PathPoint[0]);
                else
                    FainlDist += dist(PathPoint[i], PathPoint[i + 1]);
            }
            Time_MyAlgorthem = watch.ElapsedMilliseconds.ToString();
            Dist_MyAlgorthem = FainlDist.ToString();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        void resite()
        {
            m_Points = new List<Point> { };
            points = new Point[1000];
            img = new Bitmap(newimg);
            pictureBox1.Image = img;
            Maincounter = 0;
            HPts = null;
            k = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            DistanceArrayOfCitys = new int[m_Points.Count, m_Points.Count];
            for (int i = 0; i < m_Points.Count; i++)
            {

                for (int j = 0; j < m_Points.Count; j++)
                {
                    if (i == j)
                    {
                        DistanceArrayOfCitys[i, j] = 0;
                        continue;
                    }
                    DistanceArrayOfCitys[i, j] = dist(m_Points[i], m_Points[j]);
                }

            }

            var watch = System.Diagnostics.Stopwatch.StartNew();
            Dist__NeighborAlgorithm = FindPathNeighborAlgorithm(DistanceArrayOfCitys);
            watch.Stop();
            Time_NeighborAlgorithm = watch.ElapsedMilliseconds.ToString();

            MessageBox.Show($@"My Algorthem >> Execution Time: {Time_MyAlgorthem} ms And Distance: {Dist_MyAlgorthem}
Neighbor Algorithm >> Execution Time: {Time_NeighborAlgorithm} ms And {Dist__NeighborAlgorithm}");

        }

        string FindPathNeighborAlgorithm(int[,] A)
        {

            String S = "";
            int Nextraw = 0;
            int ThePotentialNextRaw = 0;
            int CurrnetRaw;
            int min;
            int secmin;
            List<int> Skpied = new List<int> { };
            int distance = 0;
            List<int> Path = new List<int> { };

            for (int i = 0; i < A.GetLength(0); i++)
            {
                Path.Add(Nextraw);
                S = S + Nextraw.ToString() + " -> ";
                min = 0;
                secmin = 0;
                CurrnetRaw = Nextraw;


                for (int j = 0; j < A.GetLength(1); j++)
                {
                    bool NextLoop = false;

                    if (A[CurrnetRaw, j] == 0)
                        continue;

                    if (i == A.GetLength(0) - 1)
                    {
                        min = A[CurrnetRaw, 0];
                        break;
                    }


                    foreach (int item in Skpied)
                    {
                        if (j == item)
                        {
                            NextLoop = true;
                            break;
                        }

                    }

                    if (NextLoop)
                        continue;

                    if (min == 0)
                    {

                        min = A[CurrnetRaw, j];
                        Nextraw = j;

                        continue;
                    }


                    if (min > A[CurrnetRaw, j])
                    {

                        secmin = min;
                        ThePotentialNextRaw = Nextraw;
                        min = A[CurrnetRaw, j];
                        Nextraw = j;
                    }

                    if (A[CurrnetRaw, j] > min && secmin == 0)
                    {
                        secmin = A[CurrnetRaw, j];
                        ThePotentialNextRaw = j;
                    }

                    if (secmin > A[CurrnetRaw, j] && min != A[CurrnetRaw, j])
                    {

                        secmin = A[CurrnetRaw, j];
                        ThePotentialNextRaw = j;
                    }


                }

                Skpied.Add(CurrnetRaw);
                distance += min;
                if (i == A.GetLength(0) - 1)
                {
                    S = S + "0";
                    Path.Add(0);
                }


            }
            return " Distance: " + distance.ToString();
        }

    }
}

