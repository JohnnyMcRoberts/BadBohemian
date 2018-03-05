// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AverageDaysPerBookPlotGenerator.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The main view model for books helix chart test application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksOxyCharts.PlotGenerators
{
    using System.Drawing;
    using System.IO;
    using BooksOxyCharts.Utilities;
    using OxyPlot;
    using OxyPlot.Axes;
    using System.Net;
    using System.Windows.Media.Imaging;
    using BooksCore.Geography;
    using OxyPlot.Annotations;

    public class WorldCountriesMapPlotGenerator : BasePlotGenerator
    {
        protected override PlotModel SetupPlot()
        {
            // Create the plot model
            PlotModel newPlot = new PlotModel { Title = "Countries of the World" };

            SetupLatitudeAndLongitudeAxes(newPlot);

            int flagCount = 0;
            foreach (Nation nation in GeographyProvider.Nations)
            {
                CountryGeography country = nation.Geography;
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
                    PolygonPoint capitalCity =
                        new PolygonPoint(nation.Longitude, nation.Latitude);
                    double x, y;
                    capitalCity.GetCoordinates(out x, out y);
                    
                    WebRequest req = WebRequest.Create(nation.ImageUri);
                    Stream stream = req.GetResponse().GetResponseStream();

                    if (stream == null)
                        continue;

                    Bitmap bitmap = new Bitmap(stream);

                    MemoryStream memoryStream = new MemoryStream();
                    bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                    byte[] asBytes = memoryStream.ToArray();

                    ImageFormat typeOfImage = GetImageFormat(asBytes);

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

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        public byte[] ImageToByte(BitmapImage imageSource)
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
