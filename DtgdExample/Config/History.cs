using DtgdExample.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DtgdExample.Config
{
    public class History
    {
        #region Загрузка

        public ObservableCollection<ColumnDetail> LoadColumnDetails(string path)
        {
            return (ObservableCollection<ColumnDetail>)XmlHistory.GetXmlData(XmlHistory.FilenameListColumns, path);
        }

        #endregion

        #region Сохранение

        public void SaveColumnDetails(ObservableCollection<ColumnDetail> symbolDetails, string path)
        {
            XmlHistory.SetXmlData(XmlHistory.FilenameListColumns, (Object)(symbolDetails.ToList()), path);
        }

        #endregion
    }
}
