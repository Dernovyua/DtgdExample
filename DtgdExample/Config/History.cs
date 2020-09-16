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

        public ObservableCollection<ExampleModel> LoadColumnDetails(string path)
        {
            return (ObservableCollection<ExampleModel>)XmlHistory.GetXmlData(XmlHistory.FilenameListColumns, path);
        }

        #endregion

        #region Сохранение

        public void SaveColumnDetails(ObservableCollection<ExampleModel> symbolDetails, string path)
        {
            XmlHistory.SetXmlData(XmlHistory.FilenameListColumns, (Object)(symbolDetails.ToList()), path);
        }

        #endregion
    }
}
