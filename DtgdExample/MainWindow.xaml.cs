using DtgdExample.Config;
using DtgdExample.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        History _history;

        public MainWindow()
        {
            InitializeComponent();
        }

        ObservableCollection<ExampleModel> _exampleModels = new ObservableCollection<ExampleModel>();
        ObservableCollection<ColumnDetail> _columnDetails = new ObservableCollection<ColumnDetail>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(Properties.Settings.Default.PathSaveSetting))
            {
                Properties.Settings.Default.PathSaveSetting =
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DtgdExample";
                Properties.Settings.Default.Save();
            }

            _history = new History();
            _columnDetails = _history.LoadColumnDetails(Properties.Settings.Default.PathSaveSetting);

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
                item.Header = "Дополнительная колонка " + (i+1);
                item.Binding = new Binding("AddColumn[" + i + "].Value");
                Style DynamicStyle = new Style();
                DynamicStyle.Setters.Add(new Setter(TextBlock.BackgroundProperty, new Binding("AddColumn[" + i + "].Color") { Converter = new ColorConverterBackGround() })); // Добавляю стиль с нужным конвертером и данные для него руками
                item.ElementStyle = DynamicStyle;
                DtgdExample.Columns.Add(item);

            }

        }

        private void BtnChangeColor_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _exampleModels.Count; i++)
            {
                _exampleModels[i].AddColumn[0].Color = Colors.Green.ToString();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            _history.SaveColumnDetails(_columnDetails, Properties.Settings.Default.PathSaveSetting);
        }
    }


}

