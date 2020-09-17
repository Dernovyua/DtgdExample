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
        public ObservableCollection<T> LoadDetails(string path, string fileName)
        {
            return (ObservableCollection<T>)XmlHistory<T>.GetXmlData(fileName, path);
        }

        #endregion

        #region Сохранение

        public void SaveDetails(ObservableCollection<T> symbolDetails,string path, string fileName)
        {
            
           XmlHistory<T>.SetXmlData(fileName, (Object)(symbolDetails.ToList()), path);
        }

        #endregion
    }
}
