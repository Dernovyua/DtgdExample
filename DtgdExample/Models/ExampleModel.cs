using DtgdExample.Config;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DtgdExample.Models
{
    public class ExampleModel: ViewModelBase
    {
        #region private 

        private string _name;
        private string _colorBackground;
        private int _numberRow;
        private ObservableCollection<DynamicColumnModel> _addColumn;

        #endregion

        #region public

        /// <summary>
        /// . 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public string ColorBackground
        {
            get { return _colorBackground; }
            set
            {
                if (_colorBackground != value)
                {
                    _colorBackground = value;
                    RaisePropertyChanged("ColorBackground");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public int NumberRow
        {
            get { return _numberRow; }
            set
            {
                if (_numberRow != value)
                {
                    _numberRow = value;
                    RaisePropertyChanged("NumberRow");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public ObservableCollection<DynamicColumnModel> AddColumn
        {
            get { return _addColumn ?? (_addColumn = new ObservableCollection<DynamicColumnModel>()); }
            set
            {
                if (_addColumn != value)
                {
                    _addColumn = value;
                    RaisePropertyChanged("AddColumn");
                }
            }
        }





        #endregion
    }
}
