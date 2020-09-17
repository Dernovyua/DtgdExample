using DtgdExample.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace DtgdExample.Config
{
    public class XmlHistory<T>
    {

        

        public static object GetXmlData(string filename, string pathSave)
        {
            var path = pathSave;
            if (!Directory.Exists(path))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                dir.Create();
            }

            #region ColumnsDetail
            
                try
                {
                    if (File.Exists(path + filename))
                    {
                        var reader = new XmlSerializer(typeof(List<T>));
                        StreamReader file = new StreamReader(path + filename);
                        List<T> result = new List<T>();
                        result = (List<T>)reader.Deserialize(file);

                        var res = new ObservableCollection<T>();
                        for (int i = 0; i < result.Count; i++)
                        {
                            res.Add(result[i]);
                        }

                        return res;
                    }
                    return new ObservableCollection<T>();
                }
                catch (Exception)
                {
                    return new ObservableCollection<T>();
                }
            

            #endregion
        }




        public static void SetXmlData(string filename, object list, string pathSave)
        {
            String path = pathSave;

            if (!Directory.Exists(path))
            {
                var dir = new DirectoryInfo(path);
                dir.Create();
            }

            path = pathSave;

            if (!Directory.Exists(path))
            {
                var dir = new DirectoryInfo(path);
                dir.Create();
            }

            #region ColumnsDetail

            
                try
                {
                    XmlSerializer write = new XmlSerializer(typeof(List<T>));
                    using (StreamWriter file = new StreamWriter(path + filename))
                        write.Serialize(file, list);
                }
                catch(Exception ex)
                {
                     //MainWindow.Log.Error(DateTime.Now + "   " + "Ошибка. Сохранение: " + ex.Message);
                     MessageBox.Show(ex.Message + ex.StackTrace,"Alert");
                }
            

            #endregion

        }


    }
}