// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The main view model for books helix chart test application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksOxyCharts.ViewModels
{
    using BooksCore.Interfaces;
    using BooksOxyCharts.Interfaces;
    using BooksUtilities.ViewModels;
    using OxyPlot;

    public class OxyPlotPairViewModel : BaseViewModel
    {
        #region Public Properties

        public IPlotGenerator PlotGenerator
        {
            get
            {
                return _plotGenerator;
            }
            set
            {
                _plotGenerator = value;
            }
        }

        public IPlotController ViewController
        {
            get
            {
                return _viewController;
            }
            set
            {
                _viewController = value;
                OnPropertyChanged(() => ViewController);
            }
        }

        public PlotModel Model
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;
                OnPropertyChanged(() => Model);
            }
        }

        public IGeographyProvider GeographyProvider { get; private set; }

        public IBooksReadProvider BooksReadProvider { get; private set; }

        #endregion

        #region Private Data

        private IPlotGenerator _plotGenerator;

        private IPlotController _viewController;

        private PlotModel _model;

        #endregion

        #region Public Methods

        public void UpdateData(IGeographyProvider geographyProvider, IBooksReadProvider booksReadProvider)
        {
            GeographyProvider = geographyProvider;
            BooksReadProvider = booksReadProvider;
            Model = _plotGenerator.SetupPlot(geographyProvider, booksReadProvider);
        }

        #endregion

        #region Constructor

        public OxyPlotPairViewModel(IPlotGenerator plotGenerator, string title, bool hoverOver = false)
        {
            _plotGenerator = plotGenerator;

            // Create the plot model & controller 
            PlotModel tmp = new PlotModel { Title = title, Subtitle = "using OxyPlot only" };

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
