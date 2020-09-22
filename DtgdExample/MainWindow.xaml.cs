using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using DtgdExample.Config;
using DtgdExample.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
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
    public class MyArray
    {
        public int value { get; set; }
        public string title { get; set; }
    }

    public class Root
    {
        public List<MyArray> MyArray { get; set; }
    }
    public partial class MainWindow : Window
    {
        public string TypeStratetgyContanier = (string)Application.Current.FindResource("m_Container");
        History<ExampleModel> HistoryRowsDetails;
        History<ColumnDetail> HistoryColumnsDetails;

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
            App.LanguageChanged += LanguageChanged;

            CultureInfo currLang = App.Language;

            //Заполняем меню смены языка:
            menuLanguage.Items.Clear();
            foreach (var lang in App.Languages)
            {
                MenuItem menuLang = new MenuItem();
                menuLang.Header = lang.DisplayName;
                menuLang.Tag = lang;
                menuLang.IsChecked = lang.Equals(currLang);
                menuLang.Click += ChangeLanguageClick;
                menuLanguage.Items.Add(menuLang);
            }

        }
        private void LanguageChanged(Object sender, EventArgs e)
        {
            CultureInfo currLang = App.Language;

            //Отмечаем нужный пункт смены языка как выбранный язык
            foreach (MenuItem i in menuLanguage.Items)
            {
                CultureInfo ci = i.Tag as CultureInfo;
                i.IsChecked = ci != null && ci.Equals(currLang);
            }
        }

        private void ChangeLanguageClick(Object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                CultureInfo lang = mi.Tag as CultureInfo;
                if (lang != null)
                {
                    App.Language = lang;
                }
            }
        }
        

        public ObservableCollection<ExampleModel> ExampleModelCollection = new ObservableCollection<ExampleModel>();
        public ObservableCollection<ColumnDetail> ColumnDetails = new ObservableCollection<ColumnDetail>();

        public static void InitiateSSLTrust()
        {
            try
            {
                //Change SSL checks so that all checks pass
                ServicePointManager.ServerCertificateValidationCallback =
                   new RemoteCertificateValidationCallback(
                        delegate
                        { return true; }
                    );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace, "Alert");
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            System.Net.WebClient client = new System.Net.WebClient();
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            string sHttpResonse = client.DownloadString("https://www.finam.ru/profile/moex-akcii/gazprom/export");
            List<string> hrefTags = new List<string>();


            var parser = new HtmlParser();
            var document = parser.Parse(sHttpResonse);
            foreach (IElement element in document.QuerySelectorAll("div.content"))
            {
                string str = element.TextContent.Replace($"\t", "").Replace($"\n", "");
                if (element.TextContent.Contains("Германии")) hrefTags.Add('[' + str.Split('[', ']')[1] +']') ;
            }
            List<MyArray> result = JsonConvert.DeserializeObject<List<MyArray>>(hrefTags[0]);
            if (String.IsNullOrEmpty(Properties.Settings.Default.PathSaveSetting))
            {
                Properties.Settings.Default.PathSaveSetting =
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DtgdExample";
                Properties.Settings.Default.Save();
            }

            HistoryRowsDetails = new History<ExampleModel>();
            HistoryColumnsDetails = new History<ColumnDetail>();

            ExampleModelCollection = HistoryRowsDetails.LoadDetails(Properties.Settings.Default.PathSaveSetting, "\\RowsDetail.xml");
            ColumnDetails = HistoryColumnsDetails.LoadDetails(Properties.Settings.Default.PathSaveSetting, "\\ColumnsDetail.xml");
            if (ExampleModelCollection.Count == 0)
            {
                //в случае обычно привязки все довольно просто
                for (int i = 0; i < 10; i++)
                {
                    ExampleModel model = new ExampleModel { Name = TypeStratetgyContanier + (i + 1), NumberRow = i + 1, ColorBackground = Colors.Aqua.ToString() };
                    model.AddColumn.Add(new DynamicColumnModel { Value = i * 4, Color = Colors.Red.ToString() });
                    ExampleModelCollection.Add(model);
                }
            }

            //правильно привязать ресурс к колонке добавленой из кода

            DtgdExample.ItemsSource = ExampleModelCollection;
            SetNewColumn();
            foreach(var res in result)
            {
                tree.Items.Add(res.title);
            }
            if (ColumnDetails.Count != 0)
            {
                try
                {
                    foreach (var column in DtgdExample.Columns)
                    {
                        var TargetRow = ColumnDetails.Where(x => x.NameColumn == column.Header.ToString()).FirstOrDefault();
                        if (TargetRow != null)
                        {
                            column.Visibility = TargetRow.IsShow == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                            column.DisplayIndex = TargetRow.Number;
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace, "Alert");
                }
            }
            foreach (var column in DtgdExample.Columns)
            {
                var checkBox = new CheckBox();
                checkBox.Content = column.Header.ToString();
                checkBox.Checked += CheckBox_Checked;
                checkBox.Unchecked += CheckBox_Unchecked;
                dtgrdContextMenu.Items.Add(checkBox);
            }
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
            HistoryRowsDetails.SaveDetails(ExampleModelCollection, Properties.Settings.Default.PathSaveSetting, "\\RowsDetail.xml");
            ColumnDetails.Clear();
            foreach (var column in DtgdExample.Columns)
            {
                ColumnDetails.Add(new ColumnDetail { NameColumn = column.Header.ToString(), IsShow = column.Visibility == System.Windows.Visibility.Visible ? true : false, Number = column.DisplayIndex });
            }
            HistoryColumnsDetails.SaveDetails(ColumnDetails, Properties.Settings.Default.PathSaveSetting, "\\ColumnsDetail.xml");
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


        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox item = sender as CheckBox;
            try
            {
                DtgdExample.Columns.Where(x => x.Header.ToString() == item.Content.ToString()).FirstOrDefault().Visibility = System.Windows.Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace, "Alert");
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox item = sender as CheckBox;
            try
            {
                DtgdExample.Columns.Where(x => x.Header.ToString() == item.Content.ToString()).FirstOrDefault().Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace, "Alert");
            }

        }
    }


}

