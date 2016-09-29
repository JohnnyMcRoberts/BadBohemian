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
using MongoDbBooks.ViewModels.PlotGenerators;

namespace MongoDbBooks.ViewModels.PlotGenerators
{
    public class OxyPlotPair : INotifyPropertyChanged
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

        #region Public Properties

        public IPlotGenerator PlotGenerator
        { get { return _plotGenerator; } set { _plotGenerator = value; } }
        public IPlotController ViewController
        { get { return _viewController; } set { _viewController = value; OnPropertyChanged(() => ViewController); } }
        public PlotModel Model
        { get { return _model; } set { _model = value; OnPropertyChanged(() => Model); } }

        #endregion

        #region Private Data

        private IPlotGenerator _plotGenerator;
        public IPlotController _viewController;
        public PlotModel _model;

        #endregion

        #region Public Methods

        public void UpdateData(MainBooksModel booksModel)
        {
            Model = _plotGenerator.SetupPlot(booksModel);
        }

        #endregion

        #region Constructor

        public OxyPlotPair(IPlotGenerator plotGenerator, string title, bool hoverOver = false)
        {
            _plotGenerator = plotGenerator;

            // Create the plot model & controller 
            var tmp = new PlotModel { Title = title, Subtitle = "using OxyPlot only" };

            // Set the Model property, the INotifyPropertyChanged event will 
            //  make the WPF Plot control update its content
            Model = tmp;
            var controller = new CustomPlotController();
            if (hoverOver)
            {
                controller.UnbindMouseDown(OxyMouseButton.Left);
                controller.BindMouseEnter(PlotCommands.HoverSnapTrack);
            }
            ViewController = controller;
        }

        #endregion

        #region Nested Classes

        public class CustomPlotController : PlotController
        {
            public CustomPlotController()
            {
                this.BindKeyDown(OxyKey.Left, PlotCommands.PanRight);
                this.BindKeyDown(OxyKey.Right, PlotCommands.PanLeft);
            }
        }

        #endregion
    }
}
