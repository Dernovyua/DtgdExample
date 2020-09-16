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
        History<ExampleModel> History;

        #region DraggedItem

        /// <summary>
        /// DraggedItem Dependency Property
        /// </summary>
        public static readonly DependencyProperty DraggedItemProperty =
            DependencyProperty.Register("DraggedItem", typeof(ExampleModel), typeof(Window));

        /// <summary>
        /// Gets or sets the DraggedItem property.  This dependency property 
        /// indicates ....
        /// </summary>
        public ExampleModel DraggedItem
        {
            get { return (ExampleModel)GetValue(DraggedItemProperty); }
            set { SetValue(DraggedItemProperty, value); }
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        public ObservableCollection<ExampleModel> ExampleModelCollection = new ObservableCollection<ExampleModel>();
        public ObservableCollection<ColumnDetail> ColumnDetails = new ObservableCollection<ColumnDetail>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(Properties.Settings.Default.PathSaveSetting))
            {
                Properties.Settings.Default.PathSaveSetting =
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DtgdExample";
                Properties.Settings.Default.Save();
            }

            History = new History<ExampleModel>();

            ExampleModelCollection = History.LoadColumnDetails(Properties.Settings.Default.PathSaveSetting);

            if (ExampleModelCollection.Count == 0)
            {
                //в случае обычно привязки все довольно просто
                for (int i = 0; i < 10; i++)
                {
                    ExampleModel model = new ExampleModel { Name = "Строка " + (i + 1), NumberRow = i + 1, ColorBackground = Colors.Aqua.ToString() };
                    model.AddColumn.Add(new DynamicColumnModel { Value = i * 4, Color = Colors.Red.ToString() });
                    ExampleModelCollection.Add(model);
                }
            }

            //правильно привязать ресурс к колонке добавленой из кода

            DtgdExample.ItemsSource = ExampleModelCollection;
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
            for (int i = 0; i < ExampleModelCollection.Count; i++)
            {
                ExampleModelCollection[i].AddColumn[0].Color = Colors.Green.ToString();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            History.SaveColumnDetails(ExampleModelCollection, Properties.Settings.Default.PathSaveSetting);
        }


        #region edit mode monitoring

        /// <summary>
        /// State flag which indicates whether the grid is in edit
        /// mode or not.
        /// </summary>
        public bool IsEditing { get; set; }

        private void OnBeginEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            IsEditing = true;
            //in case we are in the middle of a drag/drop operation, cancel it...
            if (IsDragging) ResetDragDrop();
        }

        private void OnEndEdit(object sender, DataGridCellEditEndingEventArgs e)
        {
            IsEditing = false;
        }

        #endregion

        #region Drag and Drop Rows

        /// <summary>
        /// Keeps in mind whether
        /// </summary>
        public bool IsDragging { get; set; }

        /// <summary>
        /// Initiates a drag action if the grid is not in edit mode.
        /// </summary>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsEditing) return;

            var row = UIHelpers.TryFindFromPoint<DataGridRow>((UIElement)sender, e.GetPosition(DtgdExample));
            if (row == null || row.IsEditing) return;

            //set flag that indicates we're capturing mouse movements
            IsDragging = true;
            DraggedItem = (ExampleModel)row.Item;
        }


        /// <summary>
        /// Completes a drag/drop operation.
        /// </summary>
        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsDragging || IsEditing)
            {
                return;
            }

            //get the target item
            ExampleModel targetItem = (ExampleModel)DtgdExample.SelectedItem;

            if (targetItem == null || !ReferenceEquals(DraggedItem, targetItem))
            {
                //get target index
                var targetIndex = ExampleModelCollection.IndexOf(targetItem);
                //remove the source from the list
                ExampleModelCollection.Remove(DraggedItem);

                //move source at the target's location
                ExampleModelCollection.Insert(targetIndex, DraggedItem);

                //select the dropped item
                DtgdExample.SelectedItem = DraggedItem;
            }

            //reset
            ResetDragDrop();
        }


        /// <summary>
        /// Closes the popup and resets the
        /// grid to read-enabled mode.
        /// </summary>
        private void ResetDragDrop()
        {
            IsDragging = false;
            popup1.IsOpen = false;
            DtgdExample.IsReadOnly = false;
        }


        /// <summary>
        /// Updates the popup's position in case of a drag/drop operation.
        /// </summary>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDragging || e.LeftButton != MouseButtonState.Pressed) return;

            //display the popup if it hasn't been opened yet
            if (!popup1.IsOpen)
            {
                //switch to read-only mode
                DtgdExample.IsReadOnly = true;

                //make sure the popup is visible
                popup1.IsOpen = true;
            }


            Size popupSize = new Size(popup1.ActualWidth, popup1.ActualHeight);
            popup1.PlacementRectangle = new Rect(e.GetPosition(this), popupSize);

            //make sure the row under the grid is being selected
            Point position = e.GetPosition(DtgdExample);
            var row = UIHelpers.TryFindFromPoint<DataGridRow>(DtgdExample, position);
            if (row != null) DtgdExample.SelectedItem = row.Item;
        }

        #endregion
    }


}

