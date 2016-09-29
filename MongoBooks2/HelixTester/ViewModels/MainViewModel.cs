using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using _3DTools;
//using Petzold.Media3D;

namespace HelixTester.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        void OnPropertyChanged<T>(Expression<Func<T>> sExpression)
        {
            if (sExpression == null) throw new ArgumentNullException("sExpression");

            MemberExpression body = sExpression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Body must be a member expression");
            }
            OnPropertyChanged(body.Member.Name);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members

        private MainWindow mainWindow;

        public MainViewModel(MainWindow mainWindow)
        {
            // TODO: Complete member initialization
            this.mainWindow = mainWindow;

            Model3DGroup modelGroup = new Model3DGroup();

            TubeVisual3D tube1 = GenerateTube();
            RectangleVisual3D rectangle = GenerateRectangle();
            SphereVisual3D sphere = new SphereVisual3D() { Center = new Point3D(1.5, 2.5, -0.7), Radius = 2 };

            //modelGroup.Children.Add(tube1.Model);
            //modelGroup.Children.Add(rectangle.Model);
            //modelGroup.Children.Add(sphere.Model);

            /*

            Point3D point;
            Point3D point2;
            Point3D point3;
            Point3D point4;
            AddElementsPart2(modelGroup, out point, out point2, out point3, out point4);


            TubeVisual3D path = new TubeVisual3D()
            {
                Path =  new Point3DCollection(new List<Point3D>(){
                point + new Vector3D(0, 0, 20),
                point2 + new Vector3D(0, 0, 19),
                point3 + new Vector3D(0, 0, 18),
                point4 + new Vector3D(0, 0, 17)
                }),
                Diameter = 0.5,
                ThetaDiv = 20,
                IsPathClosed = false,
                Fill = Brushes.Green
                //Fill = new Brush() { Colors.Blue}
            };

            modelGroup.Children.Add(path.Content);

            */
            //AddSolidBox(modelGroup);

            AddRevolved(modelGroup);


            
            HelixVisual3D helixItem = new HelixVisual3D();
            helixItem.Fill = new SolidColorBrush(Colors.Violet);
            helixItem.Origin = new Point3D(0,0,-0.45);
            helixItem.Diameter = 0.1;
            helixItem.Turns = 2;
            helixItem.Length = 0.9;
            helixItem.Radius = 0.35;
            
            modelGroup.Children.Add(helixItem.Content);
            //modelGroup.Children.Add(box.Content);


                       //<helix:HelixVisual3D 
                           //Origin="0 0 -0.45" 
            //Diameter="0.1" 
            //Turns="2" 
            //Length="0.9" 
            //Radius="0.35" 
            //Fill="Violet" 
            //Visible="{Binding IsChecked, ElementName=HelixVisible}"/>


            numberOfPoints = 100;

           // wireLines = new WireLines { Color = Colors.Black, Thickness = 3,  };
            Points = new Point3DCollection(GeneratePoints(numberOfPoints, 7000 * 0.001));
            //wireLines.Lines = Points;


            //modelGroup.Children.Add(wireLines.Content);


            var y0 = 0d;
            var theta = Math.PI / 180 * 30;
            var roofBuilder = new MeshBuilder(false, false);
            var y1 = y0 + Math.Tan(theta) * 5 / 2;
            var p0 = new Point(0, y1);
            var p1 = new Point(5 / 2 + 0.2 * Math.Cos(theta), y0 - 0.2 * Math.Sin(theta));
            var p2 = new Point(p1.X + 0.1 * Math.Sin(theta), p1.Y + 0.1 * Math.Cos(theta));
            var p3 = new Point(0, y1 + 0.1 / Math.Cos(theta));
            var p4 = new Point(-p2.X, p2.Y);
            var p5 = new Point(-p1.X, p1.Y);


            IList<Point> roofSection = new List<Point>() { p0, p1, p1, p2, p2, p3, p3, p4, p4, p5, p5, p0 };
            var roofAxisX = new Vector3D(0, -1, 0);
            var roofAxisY = new Vector3D(0, 0, 1);
            var roofOrigin = new Point3D(1, 1, 1);
            //roofBuilder.AddPolygon(roofSection, roofAxisX, roofAxisY, roofOrigin);


            Point3DCollection simplePoly = new System.Windows.Media.Media3D.Point3DCollection(
            new List<Point3D>(){
                new Point3D(1,0, 1),
                new Point3D(1,1, -1),
                new Point3D(0,1, -1),
                new Point3D(0,0, 1),
                //new Point3D(1,0, 1),
            }
            );


            roofBuilder.AddPolygon(simplePoly);

            GeometryModel3D roofGeometry = new GeometryModel3D();

            //var material = MaterialHelper.CreateMaterial(Brushes.LightBlue, ambient: 77, freeze: false);
            var material = MaterialHelper.CreateMaterial(Colors.LightBlue, 0.25);


            roofGeometry.Material = material;

            roofGeometry.Geometry = roofBuilder.ToMesh(true);
            modelGroup.Children.Add(roofGeometry);
          


            //ScreenSpaceLines3D Wireframe = new ScreenSpaceLines3D();

            //Wireframe.Thickness = 2;
            //Wireframe.Color = Colors.Black;
            //Wireframe.Points = simplePoly;
            ////Wireframe.MakeWireframe(roofGeometry);
            //modelGroup.Children.Add(Wireframe.Content);

            this.Model = modelGroup;
            


        }
        public Point3DCollection Points;

        public static IEnumerable<Point3D> GeneratePoints(int n, double time)
        {
            const double R = 2;
            const double Q = 0.5;
            for (int i = 0; i < n; i++)
            {
                double t = Math.PI * 2 * i / (n - 1);
                double u = (t * 24) + (time * 5);
                var pt = new Point3D(Math.Cos(t) * (R + (Q * Math.Cos(u))), Math.Sin(t) * (R + (Q * Math.Cos(u))), Q * Math.Sin(u));
                yield return pt;
                if (i > 0 && i < n - 1)
                {
                    yield return pt;
                }
            }
        }

        private int numberOfPoints;

        private LinesVisual3D linesVisual;
        private PointsVisual3D pointsVisual;
        //private ScreenSpaceLines3D screenSpaceLines;
        //private WireLines wireLines;

        private Point3DCollection points;


        private static void AddSolidBox(Model3DGroup modelGroup)
        {


            GeometryBoxVisual3D box = new GeometryBoxVisual3D();
            box.Fill = new SolidColorBrush(Colors.Red);
            modelGroup.Children.Add(box.Content);
            MeshGeometry3D geometry3 = box.Geometry();
            LinesVisual3D lines = new LinesVisual3D();
            lines.Thickness = 3;
            lines.Points = geometry3.Positions;
            lines.Color = Colors.Black;
            lines.Transform = new TranslateTransform3D(3.5, 1.5, 2.5);
            modelGroup.Children.Add(lines.Content);
        }

        private static void AddRevolved(Model3DGroup modelGroup)
        {


            GeometryModel3D railingRevolvedGeometry = new GeometryModel3D();

            railingRevolvedGeometry.Material = MaterialHelper.CreateMaterial(Brushes.LightBlue, ambient: 77);

            var builder = new MeshBuilder(true, true);
            var profile = new[] { new Point(0, 0.4), new Point(0.06, 0.36), new Point(0.1, 0.1), new Point(0.34, 0.1), new Point(0.4, 0.14), new Point(0.5, 0.5), new Point(0.7, 0.56), new Point(1, 0.46) };
            builder.AddRevolvedGeometry(profile, null, new Point3D(0, 0, 0), new Vector3D(0, 0, 1), 100);
            railingRevolvedGeometry.Geometry = builder.ToMesh(true);
            modelGroup.Children.Add(railingRevolvedGeometry);

        }

        private static void AddElementsPart2(Model3DGroup modelGroup, 
            out Point3D point, out Point3D point2, out Point3D point3, out Point3D point4)
        {


            GeometryModel3D railing = new GeometryModel3D();

            railing.Material = MaterialHelper.CreateMaterial(Brushes.LightPink, ambient: 55);
            var railingBuilder = new MeshBuilder(false, false);

            double height = 5;
            double diameter = 3;
            point = new Point3D(3, 3, 3);
            railingBuilder.AddCylinder(point, point + new Vector3D(0, 0, height), diameter, 10);
            railingBuilder.AddSphere(point, diameter * 0.75);

            railing.Geometry = railingBuilder.ToMesh();
            modelGroup.Children.Add(railing);


            GeometryModel3D railing2 = new GeometryModel3D();

            railing2.Material = MaterialHelper.CreateMaterial(Brushes.LightBlue, ambient: 77);
            var railingBuilder2 = new MeshBuilder(false, false);

            double height2 = 12;
            double diameter2 = 4;
            point2 = new Point3D(-4, -4, -4);
            railingBuilder2.AddCylinder(point2, point2 + new Vector3D(0, 0, height2), diameter2, 10);
            railingBuilder2.AddSphere(point2, 2 * diameter2);

            railing2.Geometry = railingBuilder2.ToMesh();
            modelGroup.Children.Add(railing2);



            GeometryModel3D railing3 = new GeometryModel3D();

            railing3.Material = MaterialHelper.CreateMaterial(Brushes.LightGoldenrodYellow, ambient: 177);
            var railingBuilder3 = new MeshBuilder(false, false);

            double height3 = 12;
            double diameter3 = 4;
            point3 = new Point3D(-7, 5, -3);
            //railingBuilder3.AddCylinder(point3, point3 + new Vector3D(0, 0, height3), diameter2, 10);
            //railingBuilder3.AddSphere(point3, 2 * diameter3);
            railingBuilder3.AddCone(point3, point3 + new Vector3D(0, 0, height3), diameter3, false, 15);

            railing3.Geometry = railingBuilder3.ToMesh();
            modelGroup.Children.Add(railing3);

            GeometryModel3D railing4 = new GeometryModel3D();

            railing4.Material = MaterialHelper.CreateMaterial(Brushes.LightSkyBlue, ambient: 177);
            var railingBuilder4 = new MeshBuilder(false, false);

            double height4 = 10;
            double diameter4 = 4;
            point4 = new Point3D(-14, 4, -4);
            //railingBuilder3.AddCylinder(point3, point3 + new Vector3D(0, 0, height3), diameter2, 10);
            //railingBuilder3.AddSphere(point3, 2 * diameter3);

            railingBuilder4.AddEllipsoid(point4, diameter4 / 2.0, diameter4 / 2.0, height4, 15);

            railing4.Geometry = railingBuilder4.ToMesh();
            modelGroup.Children.Add(railing4);



            TextVisual3D text = new TextVisual3D()
            {
                Foreground = Brushes.Black,
                Background = Brushes.LightBlue,
                BorderBrush = Brushes.DarkBlue,
                //BorderThickness = System.Windows.Thickness,
                Height = 2,
                //Padding  = System.Windows.Thickness,
                FontWeight = System.Windows.FontWeights.Normal,
                IsDoubleSided = true,
                Position = point3 + new Vector3D(0, 0, height3 + 1),
                UpDirection = new Vector3D(0, 0, 1),
                TextDirection = new Vector3D(0, 1, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Text = "test Text for £3 ono"
            };


            modelGroup.Children.Add(text.Content);

        }

        public class GeometryBoxVisual3D : BoxVisual3D
        {
            public MeshGeometry3D Geometry()
            {
                return Tessellate();
            }
        }

        private static RectangleVisual3D GenerateRectangle()
        {

            RectangleVisual3D rectangle = new RectangleVisual3D()
            {
                Width = 6,
                Length = 7,

                Origin = new Point3D(0.5, -0.5, 0.7),
                Normal = new Vector3D(0.5, 1, 0.7),
                LengthDirection = new Vector3D(3, 4, 5),
                Fill = Brushes.Yellow


            };
            return rectangle;
        }

        private TubeVisual3D GenerateTube()
        {


            int n = 180;
            double r = Math.Sqrt(3) / 3;
            Ring1 = this.CreatePath(0, Math.PI * 2, n, u => Math.Cos(u), u => Math.Sin(u) + r, u => Math.Cos(3 * u) / 3);

            TubeVisual3D tube1 = new TubeVisual3D()
            {
                Path = Ring1,
                Diameter = 0.5,
                ThetaDiv = 20,
                IsPathClosed = true,
                Fill = Brushes.Green
                //Fill = new Brush() { Colors.Blue}
            };

            return tube1;
        }

        /*
         * 
         * 
            <ht:TubeVisual3D Path="{Binding Ring3}" Diameter="{Binding Value, ElementName=diameterSlider}" ThetaDiv="{Binding Value, ElementName=thetaDivSlider}" IsPathClosed="True" Fill="Blue"/>
        </ht:HelixViewport3D>
        <StackPanel Orientation="Horizontal" HorizontalAlignment="Left" VerticalAlignment="Bottom" Opacity="0.5" >
            <Slider x:Name="diameterSlider" Value="0.4" Minimum="0.1" Maximum="1" Width="150" Margin="10"/>
            <Slider x:Name="thetaDivSlider" Value="20" Minimum="3" Maximum="100" Width="150" Margin="10"/>
        </StackPanel>
         * 
         
            <!-- Road -->
            <ht:RectangleVisual3D Width="10" Length="60" Origin="0,30,0" LengthDirection="0,1,0" Normal="0,0,1" Material="{ht:Material Gray}"/>
            
         * 
         * */

        private Point3DCollection CreatePath(double min, double max, int n, Func<double, double> fx, Func<double, double> fy, Func<double, double> fz)
        {
            var list = new Point3DCollection(n);
            for (int i = 0; i < n; i++)
            {
                double u = min + (max - min) * i / n;
                list.Add(new Point3D(fx(u), fy(u), fz(u)));
            }
            return list;
        }

        public Point3DCollection Ring1 { get; set; }
        public Point3DCollection Ring2 { get; set; }
        public Point3DCollection Ring3 { get; set; }


        //Property for the binding with the Skeleton
        public Model3D Model { get; set; }
    }
}
