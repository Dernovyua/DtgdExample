using DtgdExample.Config;
using DtgdExample.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DtgdExample
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        ObservableCollection<ExampleModel> _exampleModels = new ObservableCollection<ExampleModel>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //в случае обычно привязки все довольно просто
            for (int i = 0; i < 10; i++)
            {
                ExampleModel model = new ExampleModel { Name = "Строка " + (i + 1), NumberRow = i + 1, ColorBackground = Colors.Aqua.ToString() };
                model.AddColumn.Add(new DynamicColumnModel { Value = i * 4, Color = Colors.Red.ToString() });
                _exampleModels.Add(model);
            }

            //правильно привязать ресурс к колонке добавленой из кода

            DtgdExample.ItemsSource = _exampleModels;
            SetNewColumn();
        }

        /// <summary>
        /// Добавление колонок в датаГрид
        /// Здесь необходимо сделать привязку к бэкграунду ячейки
        /// </summary>
        private void SetNewColumn()
        {
            for (int i = 0; i < 1; i++)
            {
                DataGridTextColumn item = new DataGridTextColumn();
                Style st = new Style();
                st.Setters.Add(new Setter(TextBlock.BackgroundProperty, new Binding("ColorBackground") { Converter = new ColorConverterBackGround() }));
                item.ElementStyle = st;
                item.Header = "Дополнительная колонка " + (i+1);
                item.Binding = new Binding("AddColumn[" + i + "].Value");
                //item.CellStyle = (Style)Resources["ColorConverterBackground"+i]; //??? не подтягивает значения, возможно чего-то не хватает
                DtgdExample.Columns.Add(item);

            }

        }


    }


}

