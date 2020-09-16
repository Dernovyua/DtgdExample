using DtgdExample.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DtgdExample.Config
{
    public class History<T>
    {
        #region Загрузка
        public ObservableCollection<T> LoadColumnDetails(string path)
        {
            return (ObservableCollection<T>)XmlHistory<T>.GetXmlData(XmlHistory<T>.FilenameListColumns, path);
        }

        #endregion

        #region Сохранение

        public void SaveColumnDetails(ObservableCollection<T> symbolDetails, string path)
        {
            
           XmlHistory<T>.SetXmlData(XmlHistory<T>.FilenameListColumns, (Object)(symbolDetails.ToList()), path);
        }

        #endregion
    }
}
