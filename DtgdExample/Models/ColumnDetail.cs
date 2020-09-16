using DtgdExample.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DtgdExample.Models
{
    [Serializable]
    public class ColumnDetail : ViewModelBase, ISerializable
    {
        #region private void 

        private string _nameColumn;
        private int _number;
        private bool _isShow;

        #endregion

        #region public

        /// <summary>
        /// Название колонки
        /// </summary>
        public string NameColumn
        {
            get { return _nameColumn; }
            set
            {
                if (_nameColumn != value)
                {
                    _nameColumn = value;
                    RaisePropertyChanged("NameColumn");
                }
            }
        }

        /// <summary>
        /// Текущий нмоер колонки
        /// </summary>
        public int Number
        {
            get { return _number; }
            set
            {
                if (_number != value)
                {
                    _number = value;
                    RaisePropertyChanged("Number");
                }
            }
        }

        /// <summary>
        /// Показывать или нет
        /// </summary>
        public bool IsShow
        {
            get { return _isShow; }
            set
            {
                if (_isShow != value)
                {
                    _isShow = value;
                    RaisePropertyChanged("IsShow");
                }
            }
        }



        #endregion

        public ColumnDetail(SerializationInfo info, StreamingContext context)
        {
            NameColumn = info.GetString("NameColumn");
            Number = info.GetInt32("Number");
            IsShow = info.GetBoolean("IsShow");

        }
        public ColumnDetail()
        {

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("NameColumn", NameColumn);
            info.AddValue("Number", Number);
            info.AddValue("IsShow", IsShow);

        }
    }
}
