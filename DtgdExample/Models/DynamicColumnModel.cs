using DtgdExample.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DtgdExample.Models
{
    public class DynamicColumnModel : ViewModelBase
    {
        #region private 

        private double _value;
        private string _color;

        #endregion

        #region public 

        /// <summary>
        /// . 
        /// </summary>
        public double Value
        {
            get { return _value; }
            set
            {
                if (Math.Abs(_value - value) > 0.0000001)
                {
                    _value = value;
                    RaisePropertyChanged("Value");
                }
            }
        }

        /// <summary>
        /// . 
        /// </summary>
        public string Color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    RaisePropertyChanged("Color");
                }
            }
        }



        #endregion
    }
}
