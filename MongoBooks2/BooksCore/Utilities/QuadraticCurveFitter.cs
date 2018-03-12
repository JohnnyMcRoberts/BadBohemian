// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuadraticCurveFitter.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The quadratic curve fitter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksCore.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using BooksOxyCharts.Utilities;

    public class QuadraticCurveFitter : ICurveFitter
    {
        #region Constructor

        public QuadraticCurveFitter(List<double> xVals, List<double> yVals)
        {
            _numOfEntries = 0;
            _pointPair = new double[2];
            _a = _b = _c = 0.0;

            for (int i = 0; i < xVals.Count && i < yVals.Count; i++)
                AddPoints(xVals[i], yVals[i]);

            _a = aTerm();
            _b = bTerm();
            _c = cTerm();
        }

        #endregion

        #region Private data

        private double _a;
        private double _b;
        private double _c;

        /* instance variables */
        readonly ArrayList _pointArray = new ArrayList();
        private int _numOfEntries;
        private double[] _pointPair;

        #endregion

        #region Utility Methods

        /*instance methods */
        /// <summary>
        /// add point pairs
        /// </summary>
        /// <param name="x">x value</param>
        /// <param name="y">y value</param>
        private void AddPoints(double x, double y)
        {
            _pointPair = new double[2];
            _numOfEntries += 1;
            _pointPair[0] = x;
            _pointPair[1] = y;
            _pointArray.Add(_pointPair);
        }

        /// <summary>
        /// returns the a term of the equation ax^2 + bx + c
        /// </summary>
        /// <returns>a term</returns>
        private double aTerm()
        {
            if (_numOfEntries < 3)
            {
                throw new InvalidOperationException(
                   "Insufficient pairs of co-ordinates");
            }
            //notation sjk to mean the sum of x_i^j*y_i^k. 
            double s40 = GetSx4(); //sum of x^4
            double s30 = GetSx3(); //sum of x^3
            double s20 = GetSx2(); //sum of x^2
            double s10 = GetSx();  //sum of x
            double s00 = _numOfEntries;
            //sum of x^0 * y^0  ie 1 * number of entries

            double s21 = GetSx2y(); //sum of x^2*y
            double s11 = GetSxy();  //sum of x*y
            double s01 = GetSy();   //sum of y

            //a = Da/D
            return (s21 * (s20 * s00 - s10 * s10) -
                    s11 * (s30 * s00 - s10 * s20) +
                    s01 * (s30 * s10 - s20 * s20))
                    /
                    (s40 * (s20 * s00 - s10 * s10) -
                     s30 * (s30 * s00 - s10 * s20) +
                     s20 * (s30 * s10 - s20 * s20));
        }

        /// <summary>
        /// returns the b term of the equation ax^2 + bx + c
        /// </summary>
        /// <returns>b term</returns>
        private double bTerm()
        {
            if (_numOfEntries < 3)
            {
                throw new InvalidOperationException(
                   "Insufficient pairs of co-ordinates");
            }
            //notation sjk to mean the sum of x_i^j*y_i^k.
            double s40 = GetSx4(); //sum of x^4
            double s30 = GetSx3(); //sum of x^3
            double s20 = GetSx2(); //sum of x^2
            double s10 = GetSx();  //sum of x
            double s00 = _numOfEntries;
            //sum of x^0 * y^0  ie 1 * number of entries

            double s21 = GetSx2y(); //sum of x^2*y
            double s11 = GetSxy();  //sum of x*y
            double s01 = GetSy();   //sum of y

            //b = Db/D
            return (s40 * (s11 * s00 - s01 * s10) -
                    s30 * (s21 * s00 - s01 * s20) +
                    s20 * (s21 * s10 - s11 * s20))
                    /
                    (s40 * (s20 * s00 - s10 * s10) -
                     s30 * (s30 * s00 - s10 * s20) +
                     s20 * (s30 * s10 - s20 * s20));
        }

        /// <summary>
        /// returns the c term of the equation ax^2 + bx + c
        /// </summary>
        /// <returns>c term</returns>
        private double cTerm()
        {
            if (_numOfEntries < 3)
            {
                throw new InvalidOperationException(
                           "Insufficient pairs of co-ordinates");
            }
            //notation sjk to mean the sum of x_i^j*y_i^k.
            double s40 = GetSx4(); //sum of x^4
            double s30 = GetSx3(); //sum of x^3
            double s20 = GetSx2(); //sum of x^2
            double s10 = GetSx();  //sum of x
            double s00 = _numOfEntries;
            //sum of x^0 * y^0  ie 1 * number of entries

            double s21 = GetSx2y(); //sum of x^2*y
            double s11 = GetSxy();  //sum of x*y
            double s01 = GetSy();   //sum of y

            //c = Dc/D
            return (s40 * (s20 * s01 - s10 * s11) -
                    s30 * (s30 * s01 - s10 * s21) +
                    s20 * (s30 * s11 - s20 * s21))
                    /
                    (s40 * (s20 * s00 - s10 * s10) -
                     s30 * (s30 * s00 - s10 * s20) +
                     s20 * (s30 * s10 - s20 * s20));
        }

        private double rSquare() // get r-squared
        {
            if (_numOfEntries < 3)
            {
                throw new InvalidOperationException(
                   "Insufficient pairs of co-ordinates");
            }
            // 1 - (residual sum of squares / total sum of squares)
            return 1 - GetSSerr() / GetSStot();
        }


        /*helper methods*/
        private double GetSx() // get sum of x
        {
            double Sx = 0;
            foreach (double[] ppair in _pointArray)
            {
                Sx += ppair[0];
            }
            return Sx;
        }

        private double GetSy() // get sum of y
        {
            double Sy = 0;
            foreach (double[] ppair in _pointArray)
            {
                Sy += ppair[1];
            }
            return Sy;
        }

        private double GetSx2() // get sum of x^2
        {
            double Sx2 = 0;
            foreach (double[] ppair in _pointArray)
            {
                Sx2 += Math.Pow(ppair[0], 2); // sum of x^2
            }
            return Sx2;
        }

        private double GetSx3() // get sum of x^3
        {
            double Sx3 = 0;
            foreach (double[] ppair in _pointArray)
            {
                Sx3 += Math.Pow(ppair[0], 3); // sum of x^3
            }
            return Sx3;
        }

        private double GetSx4() // get sum of x^4
        {
            double Sx4 = 0;
            foreach (double[] ppair in _pointArray)
            {
                Sx4 += Math.Pow(ppair[0], 4); // sum of x^4
            }
            return Sx4;
        }

        private double GetSxy() // get sum of x*y
        {
            double Sxy = 0;
            foreach (double[] ppair in _pointArray)
            {
                Sxy += ppair[0] * ppair[1]; // sum of x*y
            }
            return Sxy;
        }

        private double GetSx2y() // get sum of x^2*y
        {
            double Sx2y = 0;
            foreach (double[] ppair in _pointArray)
            {
                Sx2y += Math.Pow(ppair[0], 2) * ppair[1]; // sum of x^2*y
            }
            return Sx2y;
        }

        private double GetYMean() // mean value of y
        {
            double y_tot = 0;
            foreach (double[] ppair in _pointArray)
            {
                y_tot += ppair[1];
            }
            return y_tot / _numOfEntries;
        }

        private double GetSStot() // total sum of squares
        {
            //the sum of the squares of the differences between 
            //the measured y values and the mean y value
            double ss_tot = 0;
            foreach (double[] ppair in _pointArray)
            {
                ss_tot += Math.Pow(ppair[1] - GetYMean(), 2);
            }
            return ss_tot;
        }

        private double GetSSerr() // residual sum of squares
        {
            //the sum of the squares of te difference between 
            //the measured y values and the values of y predicted by the equation
            double ss_err = 0;
            foreach (double[] ppair in _pointArray)
            {
                ss_err += Math.Pow(ppair[1] - GetPredictedY(ppair[0]), 2);
            }
            return ss_err;
        }

        private double GetPredictedY(double x)
        {
            //returns value of y predicted by the equation for a given value of x
            return aTerm() * Math.Pow(x, 2) + bTerm() * x + cTerm();
        }

        #endregion

        #region ICurveFitter

        public double EvaluateYValueAtPoint(double xVal)
        {
            return (xVal * xVal * _a) + (xVal * _b) + _c;
        }

        #endregion
    }
}
