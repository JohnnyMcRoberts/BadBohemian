using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;

using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

using MongoDbBooks.Models;
using MongoDbBooks.ViewModels.Utilities;
using OxyPlot.Annotations;
using System.IO;
using System.Windows.Media.Imaging;
using System.Net;

namespace MongoDbBooks.ViewModels.PlotGenerators
{
    public class WorldCountriesMapPlotGenerator : IPlotGenerator
    {
        public OxyPlot.PlotModel SetupPlot(Models.MainBooksModel mainModel)
        {
            _mainModel = mainModel;
            return SetupWorldCountriesMapPlot();
        }
        private Models.MainBooksModel _mainModel;

        private PlotModel SetupWorldCountriesMapPlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Countries of the World" };

            SetupLatitudeAndLongitudeAxes(newPlot);

            int flagCount = 0;
            foreach (Models.Database.Nation nation in _mainModel.Nations)
            {
                Models.Geography.CountryGeography country = nation.Geography;
                if (country != null)
                {
                    OxyColor colour = OxyColors.LightGreen;
                    string title = country.Name;
                    string tag = "";
                    string trackerFormat = "{0}";
                    OxyPlotUtilities.AddCountryGeographyAreaSeriesToPlot(newPlot, country, colour, title, tag, trackerFormat);
                }

                if (!string.IsNullOrEmpty(nation.ImageUri) && flagCount < 10)
                {
                    Models.Geography.PolygonPoint capitalCity =
                        new Models.Geography.PolygonPoint(nation.Longitude, nation.Latitude);
                    double x, y;
                    capitalCity.GetCoordinates(out x, out y);
                    
                    WebRequest req = HttpWebRequest.Create(nation.ImageUri);
                    Stream stream = req.GetResponse().GetResponseStream();
                    var bitmap = new System.Drawing.Bitmap(stream);

                    MemoryStream memoryStream = new MemoryStream();
                    bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                    var asBytes = memoryStream.ToArray();

                    var typeOfImage = GetImageFormat(asBytes);

                    if (typeOfImage == ImageFormat.Unknown)
                        continue;

                    OxyImage image = new OxyImage(asBytes);
                    newPlot.Annotations.Add(
                        new ImageAnnotation
                        {
                            ImageSource = image,
                            Opacity = 0.5,

                            X = new PlotLength(x, PlotLengthUnit.Data),
                            Y = new PlotLength(y, PlotLengthUnit.Data),
                            Width = new PlotLength(30, PlotLengthUnit.ScreenUnits),
                            Height = new PlotLength(20, PlotLengthUnit.ScreenUnits),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Middle
                        });


                    //newPlot.Annotations.Add(new TextAnnotation { TextPosition = new DataPoint(x, y), Text = nation.Capital });

                    flagCount++;
                }

            }

            // finally update the model with the new plot
            return newPlot;
        }

        /// <summary>
        /// Gets the image format.
        /// </summary>
        /// <param name="bytes">The image bytes.</param>
        /// <returns>The <see cref="ImageFormat" /></returns>
        private static ImageFormat GetImageFormat(byte[] bytes)
        {
            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xD8)
            {
                return ImageFormat.Jpeg;
            }

            if (bytes.Length >= 2 && bytes[0] == 0x42 && bytes[1] == 0x4D)
            {
                return ImageFormat.Bmp;
            }

            if (bytes.Length >= 4 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
            {
                return ImageFormat.Png;
            }

            return ImageFormat.Unknown;
        }

        private System.Drawing.Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new System.Drawing.Bitmap(bitmap);
            }
        }

        public byte[] ImageToByte(System.Windows.Media.Imaging.BitmapImage imageSource)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageSource));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }


        private void SetupLatitudeAndLongitudeAxes(PlotModel newPlot)
        {
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Longitude",
                Key = ChartAxisKeys.LongitudeKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
                Maximum = 200,
                Minimum = -200
            };
            newPlot.Axes.Add(xAxis);

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Latitude",
                Key = ChartAxisKeys.LatitudeKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
                Maximum = 100,
                Minimum = -100
            };
            newPlot.Axes.Add(yAxis);
        }
        
    }
}
