using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroGridTest.ViewModels
{
    public class MainViewModel : Caliburn.Micro.PropertyChangedBase
    {
        private const string WindowTitleDefault = "THE grid tester !";

        private string _windowTitle = WindowTitleDefault;


        private DataGridTestViewModel _dataGridTestViewModel;

        public MainViewModel()
        {
            DataGridTestView = new DataGridTestViewModel();
        }

        public DataGridTestViewModel DataGridTestView
        {
            get { return _dataGridTestViewModel; }
            set
            {
                _dataGridTestViewModel = value;
                NotifyOfPropertyChange(() => DataGridTestView);
            }
        }
        

        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                _windowTitle = value;
                NotifyOfPropertyChange(() => WindowTitle);
            }
        }

    }
}
