// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorUtilities.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The color utility functions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BooksUtilities.Colors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;

    //using OxyPlot;


    public class ColorUtilities
    {
        #region Constants

        // taken from http://dmcritchie.mvps.org/excel/colors.htm
        public static readonly List<Tuple<byte, byte, byte>> StandardColours =
            new List<Tuple<byte, byte, byte>>()
        {
                                    // R    G    B
            new Tuple<byte,byte,byte>(255,  0   ,0),
            new Tuple<byte,byte,byte>(0,    255 ,0),
            new Tuple<byte,byte,byte>(0,    0   ,255),
            new Tuple<byte,byte,byte>(255,  255 ,0),
            new Tuple<byte,byte,byte>(255,  0   ,255),
            new Tuple<byte,byte,byte>(0,    255 ,255),
            new Tuple<byte,byte,byte>(128,  0   ,0),
            new Tuple<byte,byte,byte>(0,    128 ,0),
            new Tuple<byte,byte,byte>(0,    0,  128),
            new Tuple<byte,byte,byte>(128,  128 ,0),
            new Tuple<byte,byte,byte>(128,  0   ,128),
            new Tuple<byte,byte,byte>(0,    128 ,128),
            new Tuple<byte,byte,byte>(153,  153 ,255),
            new Tuple<byte,byte,byte>(153,  51  ,102),
            new Tuple<byte,byte,byte>(255,  255 ,204),
            new Tuple<byte,byte,byte>(204,  255 ,255),
            new Tuple<byte,byte,byte>(102,  0   ,102),
            new Tuple<byte,byte,byte>(255,  128 ,128),
            new Tuple<byte,byte,byte>(0,    102 ,204),
            new Tuple<byte,byte,byte>(204,  204 ,255),
            new Tuple<byte,byte,byte>(0,    0   ,128),
            new Tuple<byte,byte,byte>(255,  0   ,255),
            new Tuple<byte,byte,byte>(255,  255 ,0),
            new Tuple<byte,byte,byte>(0,    255 ,255),
            new Tuple<byte,byte,byte>(128,  0   ,128),
            new Tuple<byte,byte,byte>(128,  0   ,0),
            new Tuple<byte,byte,byte>(0,    128 ,128),
            new Tuple<byte,byte,byte>(0,    0   ,255),
            new Tuple<byte,byte,byte>(0,    204 ,255),
            new Tuple<byte,byte,byte>(204,  255 ,255),
            new Tuple<byte,byte,byte>(204,  255 ,204),
            new Tuple<byte,byte,byte>(255,  255 ,153),
            new Tuple<byte,byte,byte>(153,  204 ,255),
            new Tuple<byte,byte,byte>(255,  153 ,204),
            new Tuple<byte,byte,byte>(204,  153 ,255),
            new Tuple<byte,byte,byte>(255,  204 ,153),
            new Tuple<byte,byte,byte>(51,   102 ,255),
            new Tuple<byte,byte,byte>(51,   204 ,204),
            new Tuple<byte,byte,byte>(153,  204 ,0),
            new Tuple<byte,byte,byte>(255,  204 ,0),
            new Tuple<byte,byte,byte>(255,  153 ,0),
            new Tuple<byte,byte,byte>(255,  102 ,0),
            new Tuple<byte,byte,byte>(102,  102 ,153),
            new Tuple<byte,byte,byte>(150,  150 ,150),
            new Tuple<byte,byte,byte>(0,    51  ,102),
            new Tuple<byte,byte,byte>(51,   153 ,102),
            new Tuple<byte,byte,byte>(0,    51  ,0),
            new Tuple<byte,byte,byte>(51,  51   ,0),
            new Tuple<byte,byte,byte>(153,  51  ,0),
            new Tuple<byte,byte,byte>(153,  51  ,102),
            new Tuple<byte,byte,byte>(51,   51  ,153),
        };

        #endregion

        public static int SetupFaintPaletteForRange(
            int range,
            out List<Color> colors,
            byte aValue = 225)
        {
            // add 20% tolerance to the range
            range *= 12;
            range /= 10;
            if (range < 15)
                range = 5;

            // set up the colours
            colors = Jet(range).Select(color => Color.FromArgb(aValue, color.R, color.G, color.B)).ToList();

            // Put them into the palette
            return range;
        }


        /// <summary>
        /// Interpolates the specified colors.
        /// </summary>
        /// <param name="color1">
        /// The color1.
        /// </param>
        /// <param name="color2">
        /// The color2.
        /// </param>
        /// <param name="t">
        /// The t.
        /// </param>
        /// <returns>
        /// The interpolated color
        /// </returns>
        public static Color Interpolate(Color color1, Color color2, double t)
        {
            double a = (color1.A * (1 - t)) + (color2.A * t);
            double r = (color1.R * (1 - t)) + (color2.R * t);
            double g = (color1.G * (1 - t)) + (color2.G * t);
            double b = (color1.B * (1 - t)) + (color2.B * t);
            return Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
        }

        /// <summary>
        /// Interpolates the specified colors to a palette of the specified size.
        /// </summary>
        /// <param name="paletteSize">
        /// The size of the palette.
        /// </param>
        /// <param name="colors">
        /// The colors.
        /// </param>
        /// <returns>
        /// A palette.
        /// </returns>
        public static List<Color> Interpolate(int paletteSize, params Color[] colors)
        {
            var palette = new Color[paletteSize];
            for (int i = 0; i < paletteSize; i++)
            {
                double y = (double)i / (paletteSize - 1);
                double x = y * (colors.Length - 1);
                int i0 = (int)x;
                int i1 = i0 + 1 < colors.Length ? i0 + 1 : i0;
                palette[i] = Interpolate(colors[i0], colors[i1], x - i0);
            }

            return new List<Color>(palette);
        }

        /// <summary>
        /// Creates a 'jet' palette with the specified number of colors.
        /// </summary>
        /// <param name="numberOfColors">
        /// The number of colors to create for the palette.
        /// </param>
        /// <returns>
        /// A palette.
        /// </returns>
        /// <remarks>
        /// See http://www.mathworks.se/help/techdoc/ref/colormap.html.
        /// </remarks>
        public static List<Color> Jet(int numberOfColors)
        {
            return Interpolate(
                numberOfColors,
                Colors.DarkBlue,
                Colors.Cyan,
                Colors.Yellow,
                Colors.Orange,
                Colors.DarkRed);
        }

        //public static void SetupFaintPaletteForRange(int range, out List<Color> colors, byte aValue = 225)
        //{
        //    // Get the soft OxyPlot colors.
        //    List<OxyColor> oxyColors;
        //    OxyPalette faintPalette;
        //    SetupFaintPaletteForRange(range, out oxyColors, out faintPalette, aValue);

        //    colors = new List<Color>();
        //    foreach (OxyColor color in oxyColors)
        //    {
        //        colors.Add(Color.FromArgb(color.A, color.R, color.G, color.B));
        //    }
        //}

        public static List<Color> SetupStandardColourSet(byte aValue = 225)
        {
            List<Color> standardColours = new List<Color>();

            foreach (var colour in StandardColours)
                standardColours.Add(Color.FromArgb(aValue, colour.Item1, colour.Item2, colour.Item3));
            return standardColours;
        }
    }
}
