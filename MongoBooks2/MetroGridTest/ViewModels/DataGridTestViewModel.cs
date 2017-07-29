
namespace MetroGridTest.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;


    public class DataGridTestViewModel : Caliburn.Micro.PropertyChangedBase
    {
        ///
        /// nested classes
        ///

        public class RandomRowItem : INotifyPropertyChanged
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

            private static Random _numberGenerator = new Random();

            private static int _itemCount = 0;

            private bool _isSelected = false;

            public DateTime CreatedDate { get; set; }

            public string Name { get; set; }

            public int Score { get; set; }

            public bool IsSelected { get { return _isSelected; }
                set { if (value == _isSelected) return; _isSelected = value; Console.WriteLine("changed selection for " + Name);
                    OnPropertyChanged(() => IsSelected);
                } }

            public static Random Generator { get { return _numberGenerator; } }

            public RandomRowItem()
            {
                _itemCount++;
                Name = "Random item #" + _itemCount;

                CreatedDate = DateTime.Now;

                Score = _numberGenerator.Next(100);
            }

        }


        ///
        /// private data
        ///

        private readonly string _addRowText = "Add Row";

        private readonly string _selectRowsText = "Select some Rows";

        private DataGrid _loadedDataGrid;

        private ObservableCollection<RandomRowItem> _selectedRows;

        ///
        /// Public Data.
        ///

        public string AddRowText => _addRowText;

        public string SelectRowsText => _selectRowsText;

        public ObservableCollection<RandomRowItem> RandomRows { get; set; }

    
        public ObservableCollection<RandomRowItem> SelectedItemsList
        {
            get { return _selectedRows; }
            set { _selectedRows = value; NotifyOfPropertyChange(() => SelectedItemsList); }
        }

        ///
        /// Private functions
        /// 

        private void OtherObservableListChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("OtherObservableListChanged rows = " + SelectedItemsList.Count);

            foreach (var row in RandomRows)
            {
                row.IsSelected = SelectedItemsList.Contains(row);
            }
        }

        /// 
        /// Public functions.
        /// 

        public void AddRow()
        {
            Console.WriteLine("AddRow");
            RandomRows.Add(new RandomRowItem());
            NotifyOfPropertyChange(() => RandomRows);
        }
        
        public void GridLoaded(object source, object view, object self)
        {

            Console.WriteLine("Selected source = " + source?.GetType().ToString());
            Console.WriteLine("Selected view = " + view?.GetType().ToString());
            Console.WriteLine("Selected self = " + self?.GetType().ToString());

            _loadedDataGrid = source as DataGrid;

            //_loadedDataGrid.
        }

        public void SelectionChanged(object sender)
        {
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid == null)
                return;
        }

        public void SelectRows()
        {
            Console.WriteLine("SelectRows");

            _loadedDataGrid?.UnselectAll();
            
            for (int i = 0; i< RandomRows.Count; i++)
            {
                if (RandomRowItem.Generator.NextDouble() > 0.5)
                    RandomRows[i].IsSelected = true;
                else
                    RandomRows[i].IsSelected = false;

            }
            NotifyOfPropertyChange(() => RandomRows);
            NotifyOfPropertyChange(() => SelectedItemsList);
        }

        public DataGridTestViewModel()
        {
            RandomRows = new ObservableCollection<RandomRowItem>()
            {
                new RandomRowItem(),
                new RandomRowItem(),
                new RandomRowItem()
            };

            SelectedItemsList = new ObservableCollection<RandomRowItem>();

            SelectedItemsList.CollectionChanged += OtherObservableListChanged;
        }

    }

}
