using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Navigation;
using System.IO;
using System.IO.IsolatedStorage;
using SharpCompress.Archive.Zip;
using SharpCompress.Reader;
using IncomeDataStorage.Presentation;
using IncomeDataStorage.Data;
using System.ComponentModel;
using System.Threading;
using System.Collections.ObjectModel;

namespace IncomeDataStorage
{
    public delegate void ScenarioStep();
    public delegate void SecondaryKeyDataProcessing(SecondaryKeyDataParam Param);
    
    public partial class ExcelFileParsePage : PhoneApplicationPage,  INotifyPropertyChanged
    {     
        private ProgressIndicator progress;
        private ExcelFileParser parser;
        private DataSetSelector selector;
        private List<Grid> rowGrids;
        private List<Grid> colGrids;
        private Border selectedBorder; // выбранная граница.
        private List<Border> confirmedBorders; // сюда попадают границы, которые были подтверждены. сам понял что написал?
        private bool selectionValid; // указывает возможность выбора набора данных из таблицы.
        private Button selectionConfitmationBtn; // Кнопка, подтверждающая выбор набора данных из таблицы экселя.

        private KeyField selectedKeyField;
        private Grid ReselectPrimaryDatasetArea;
        public StackPanel PrimaryAnalysisResultArea;
        private int nextExamPair;

        public class KeyField
        {
            public string Name { get; set; }
            public string DBName { get; set; }
        }
        
        private List<KeyField> keyFieldList;
        public List<KeyField> KeyFieldList
        {
            get
            {
                return keyFieldList;
            }
            set
            {
                keyFieldList = value;
                NotifyPropertyChanged("KeyFieldList");
            }
        }
        /*public ObservableCollection<ListPickerItem> KeyFieldList
        {
            get
            {
                return keyFieldList;
            }
            set
            {
                keyFieldList = value;
                NotifyPropertyChanged("KeyFieldList");
            }
        } */

        private Grid ReselectPrimaryAnalysisResultArea;

        private string status;
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                NotifyPropertyChanged("Status");
            }
        }

        private Visibility dialogVisibility;
        public Visibility DialogVisibility
        {
            get
            {
                return dialogVisibility;
            }
            set
            {
                dialogVisibility = value;
                NotifyPropertyChanged("DialogVisibility");
            }
        }
        
        public ExcelFileParsePage()
        {
            InitializeComponent();
            progress = new ProgressIndicator();
            progress.IsIndeterminate = true;
            SystemTray.SetProgressIndicator(this, progress);

            this.DataContext = this;
            //this.RowList.ItemsSource = 

            DialogVisibility = System.Windows.Visibility.Collapsed;

            selectionValid = true;
            confirmedBorders = new List<Border>();
            
            InitialKeyFieldList();
            // этого уже нет this.lp_KeyField.ItemsSource = KeyFieldList;
            //TryExtract();  // попытаемся разархивировать экселевский файл
        }

        private void InitialKeyFieldList()
        {
            if (keyFieldList == null) keyFieldList = new List<KeyField>();
            else keyFieldList.Clear();

            KeyFieldList.Add(new KeyField() { Name = "Номер квартиры", DBName = "FloorNo" });
            KeyFieldList.Add(new KeyField() { Name = "Ф.И.О. собственника", DBName = "Name" });
            KeyFieldList.Add(new KeyField() { Name = "Разделение учетов", DBName = "WaterCounterIsDivide" });
            KeyFieldList.Add(new KeyField() { Name = "Адрес дома", DBName = "BuildAdress" });
            /* if (KeyFieldList == null) KeyFieldList = new ObservableCollection<ListPickerItem>();
            else KeyFieldList.Clear();

            KeyFieldList.Add(new ListPickerItem() { Content = "Номер квартиры" });
            KeyFieldList.Add(new ListPickerItem() { Content = "Ф.И.О. собственника" });
            KeyFieldList.Add(new ListPickerItem() { Content = "Разделение учетов" });
            KeyFieldList.Add(new ListPickerItem() { Content = "Адрес дома" }); */
        }

        private void ProgressStartHandler()
        {
            progress.IsIndeterminate = true;
            progress.IsVisible = true;
        }

        private void ProgressStopHandler()
        {
            progress.IsIndeterminate = false;
            progress.IsVisible = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.ContainsKey("FileName"))
            {
                var parsedFile = NavigationContext.QueryString["FileName"];
                this.ImportParamField.Visibility = System.Windows.Visibility.Collapsed;
                parser = new ExcelFileParser(parsedFile);

                Status = "Идет открытие файла...";
                ProgressStartHandler();
                this.Dispatcher.BeginInvoke(new ScenarioStep(FirstStep));
            }
            else
            {
                MessageBox.Show("Нет файла!?!?");
            }
        }

        private void FirstStep()
        {             
            BackgroundWorker bcw = new BackgroundWorker();
            bcw.DoWork += new DoWorkEventHandler(bcw_DoWork);

            bcw.RunWorkerAsync();
            //CreateExcelTableMask();

            while (bcw.IsBusy) { }

            //var dis = this.Dispatcher;
            CreateExcelTableMask();
            ProgressStopHandler();
            Status = "Для начала, нужны ключевые данные:";
            ShowSelector();
        }

        private void ShowSelector()
        {
            this.ImportParamField.Visibility = System.Windows.Visibility.Visible;
            if (selector == null) selector = new DataSetSelector(parser.SheetData);
            selectionConfitmationBtn = btn_Accept;
            selectionConfitmationBtn.Click += btn_Accept_Click;
            SetSelectionMode();
        }

        private void SetSelectionMode()
        {
            if (selectedBorder != null)
            {
                selectedBorder.BorderThickness = new Thickness(1);
                selectedBorder.BorderBrush = new SolidColorBrush(Colors.DarkGray);
            }
            selector.MaskSelectedInd = -1;
            btn_Accept.Visibility = System.Windows.Visibility.Collapsed;
            if (listPickerMethod.SelectedIndex == 0)
            {
                selector.SelMode = Presentation.SelectionMode.Col;
                foreach (var grd in colGrids) grd.IsHitTestVisible = true;
                foreach (var grd in rowGrids) grd.IsHitTestVisible = false;
            }
            if (listPickerMethod.SelectedIndex == 1)
            {
                selector.SelMode = Presentation.SelectionMode.Row;
                foreach (var grd in colGrids) grd.IsHitTestVisible = false;
                foreach (var grd in rowGrids) grd.IsHitTestVisible = true;
            }
        }

        void bcw_DoWork(object sender, DoWorkEventArgs e)
        {
            parser.TryToExtract();        // попытка распаковать экселевский файл
            parser.TryToCollectTableData(); // попытка отобразить таблицу. ну на самом деле это не попытка отобразить, а 
                                            // попытка собрать инфу о таблице.
        }

        private void CreateExcelTableMask()
        {
            // буду считать что информация собрана и теперь необходимо отобразить таблицу на экранчике.
            ExcelDataTableField.Background = new SolidColorBrush(Colors.Transparent);

            // Исходя из найденой размерности таблицы, задам количество столбцов и строк в гриде:
            int i = 1;
            int count = parser.SheetData.Dimension.ColsCount;
            while (i <= count)
            {
                ExcelDataTableField.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100) });
                i++;
            }
            i = 1;
            count = parser.SheetData.Dimension.RowsCount;
            while (i <= count)
            {
                ExcelDataTableField.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
                i++;
            }
            
            // Украшательство. Чтобы были видны границы ячеек.
            var colCount = this.ExcelDataTableField.ColumnDefinitions.Count;
            var rowCount = this.ExcelDataTableField.RowDefinitions.Count;
            rowGrids = new List<Grid>();
            for (i = 0; i <= rowCount; i++)
            {
                Grid grd = new Grid();
                grd.Background = new SolidColorBrush(Colors.Transparent);
                Border brd = new Border();
                grd.SetValue(Grid.RowProperty, i);
                grd.SetValue(Grid.ColumnProperty, 0);
                ExcelDataTableField.Children.Add(grd);
                grd.IsHitTestVisible = false;
                grd.SetValue(Grid.ColumnSpanProperty, ExcelDataTableField.ColumnDefinitions.Count);
                brd.BorderThickness = new Thickness(1);
                brd.BorderBrush = new SolidColorBrush(Colors.DarkGray);
                grd.Tap += ExcelTableMask_Tap;
                grd.Children.Add(brd);
                rowGrids.Add(grd);
            }

            colGrids = new List<Grid>();
            for (i = 0; i <= colCount; i++)
            {
                Grid grd = new Grid();
                grd.Background = new SolidColorBrush(Colors.Transparent);
                Border brd = new Border();
                //brd.SetValue(Grid.ColumnProperty, i);
                //brd.SetValue(Grid.RowSpanProperty, ExcelDataTableField.RowDefinitions.Count); 
                grd.SetValue(Grid.ColumnProperty, i);
                grd.SetValue(Grid.RowSpanProperty, ExcelDataTableField.RowDefinitions.Count); 
                brd.BorderThickness = new Thickness(1);
                brd.BorderBrush = new SolidColorBrush(Colors.Gray);
                //brd.IsHitTestVisible = true;
                //brd.Tap += ExcelTableMask_Tap;
                grd.Tap += ExcelTableMask_Tap;
                grd.Children.Add(brd);
                grd.IsHitTestVisible = false;
                ExcelDataTableField.Children.Add(grd);
                colGrids.Add(grd);
            } 

            // Теперь заполню грид значениями ячеек:
            foreach (var cell in parser.SheetData.Cells)
            {
                Grid grd = new Grid();
                TextBlock tb = new TextBlock();
                var rowInd = int.Parse(cell.RowInd) - 1;
                var colInd = Useful.ExcelColNoCalc.ColNo(cell.CollInd) - 1;
                //tb.SetValue(Grid.RowProperty, rowInd);
                //tb.SetValue(Grid.ColumnProperty, colInd);
                grd.SetValue(Grid.RowProperty, rowInd);
                grd.SetValue(Grid.ColumnProperty, colInd);
                grd.IsHitTestVisible = false;
                ExcelDataTableField.Children.Add(grd);
                tb.Text = cell.Value;
                tb.Margin = new Thickness(3, 0, 0, 0);
                tb.Foreground = new SolidColorBrush(Colors.White);
                //tb.IsHitTestVisible = false;
                //grd.Tap += ExcelTableMask_Tap;
                grd.Children.Add(tb);
            } 

            this.tBl_RowCount.Text = ExcelDataTableField.Children[0].GetType().Name; // Это чисто для эксперимента.
        }

        // сообщатель события:
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void listPickerMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listPickerMethod == null) return;
            if (selector == null)
            {
                listPickerMethod.SelectedIndex = 0;
                return;
            }
            SetSelectionMode();
        }

        private void ExcelTableMask_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            excelDataTableFieldTouch = true;

            if (!selectionValid) return;

            if (selectedBorder != null)
            {
                selectedBorder.BorderThickness = new Thickness(1);
                selectedBorder.BorderBrush = new SolidColorBrush(Colors.DarkGray);
            }
            //Point pt = e.GetPosition((UIElement)sender);
            //VisualTreeHelper.FindElementsInHostCoordinates(pt, 
            Grid grd = sender as Grid;
            //int selCol = (int)grd.ColumnDefinitions.GetValue(Grid.ColumnProperty);
            //this.tBl_RowCount.Text = grd.GetValue(Grid.RowProperty).ToString();
            Border brd = grd.Children[0] as Border;

            if (confirmedBorders.Contains(brd))
            {
                selectedBorder = null;
                selectionConfitmationBtn.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }

            brd.BorderThickness = new Thickness(3);
            brd.BorderBrush = new SolidColorBrush(Colors.Yellow);
            selectedBorder = brd;
            if (selector != null) 
            {
                if (selector.SelMode == Presentation.SelectionMode.Col)
                {
                    selector.MaskSelectedInd = (int)grd.GetValue(Grid.ColumnProperty);
                }
                if (selector.SelMode == Presentation.SelectionMode.Row)
                {
                    selector.MaskSelectedInd = (int)grd.GetValue(Grid.RowProperty);
                }

                 selectionConfitmationBtn.Visibility = System.Windows.Visibility.Visible;
            }
            this.tBl_RowCount.Text = selector.MaskSelectedInd.ToString();
        }

        private void btn_Accept_Click(object sender, RoutedEventArgs e)
        {
            selectionValid = false; // пользователю нельзя будет выбрать набор из таблицы пока не пройдут все этапы анализа.

            selectedKeyField = keyFieldList[0];
            if (selector != null)
                if (selector.MaskSelectedInd != -1)
                {
                    selector.AnalysePrimaryKeyDataSet(selectedKeyField.DBName);
                    // Проверить сперва нужно усе ли в поряде?
                    if (selector.PrimaryKeyDataSetAnalysisComplete)
                    {
                        KeyPrimaryDataSetSelectArea1.Visibility = Visibility.Collapsed;
                        KeyPrimaryDataSetSelectArea2.Visibility = Visibility.Collapsed;
                        if (selectedBorder != null)
                        {
                            selectedBorder.BorderBrush = new SolidColorBrush(new Color() { A = 255, R = 60, G = 179, B = 113 });
                        }
                        ShowPrimaryAnalysisResult();
                    }
                    else
                        MessageBox.Show("Чета не получилось с анализом... Возможно следует выбрать другой набор данных, либо.. не судьба...");
                    //tbl_TempData.Text = selector.TempData;
                }
                else
                    MessageBox.Show("Не выбран набор данных!"); 

        }

        private void ShowPrimaryAnalysisResult()
        {
            StatusText.Text = "Подтвердите результаты анализа:";

            ReselectPrimaryDatasetArea = new Grid()
            {
                Background = new SolidColorBrush(new Color() { A = 255, R = 60, G = 179, B = 113 }),
                Height = 30
            };
            TextBlock ReselectCaption = new TextBlock() 
            {   
                Text = "Первичный набор данных выбран.", FontSize = 18, 
                Foreground = new SolidColorBrush(Colors.Black), 
                Margin = new Thickness(10, 5, 0, 0)
            };
            TextBlock ReselectAction = new TextBlock() 
            { 
                Text = "  ...  ", TextAlignment = TextAlignment.Right, 
                FontSize = 30, FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.Black),
                Margin = new Thickness(0, -20, 0, 0)
            };
            ReselectPrimaryDatasetArea.Children.Add(ReselectCaption);
            ReselectPrimaryDatasetArea.Children.Add(ReselectAction);
            DataSelector.Children.Add(ReselectPrimaryDatasetArea);

            if (PrimaryAnalysisResultArea != null) PrimaryAnalysisResultArea = null;
            PrimaryAnalysisResultArea = new StackPanel();
            DataSelector.Children.Add(PrimaryAnalysisResultArea);

            Grid CaptionArea = new Grid() { Height = 45 };
            TextBlock Caption = new TextBlock()
            {
                Text = "Результаты анализа:",
                FontSize = 28,
                Margin = new Thickness(10, 5, 5, 0)
            };
            CaptionArea.Children.Add(Caption);
            PrimaryAnalysisResultArea.Children.Add(CaptionArea);

            var pair = selector.PrimaryKeyData.KeyMaskDic.ElementAt<KeyValuePair<Data.Cell, Mask>>(0);

            nextExamPair = 0;
            PrimaryAnalysisConfirmation(nextExamPair);
             
           /* foreach (var primaPair in selector.PrimaryData.MaskDic)
            {
                if (primaPair.Value.IsHeader)
                {
                    //Grid HeaderArea = new Grid();
                    //StackPanel HeaderPanel = new StackPanel();

                    Grid HeaderArea = new Grid();
                    HeaderArea.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                    HeaderArea.ColumnDefinitions.Add(new ColumnDefinition());
                    HeaderArea.RowDefinitions.Add(new RowDefinition());
                    HeaderArea.RowDefinitions.Add(new RowDefinition());

                    StackPanel panel = new StackPanel();
                    panel.SetValue(Grid.ColumnProperty, 1);

                    Grid selectionGrid = new Grid();
                    selectionGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    selectionGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    selectionGrid.SetValue(Grid.RowProperty, 1);
                    selectionGrid.SetValue(Grid.ColumnProperty, 1);

                    TextBlock Sign = new TextBlock()
                    {
                        Text = "? ",
                        Foreground = new SolidColorBrush(Colors.Yellow),
                        FontSize = 36, FontWeight = FontWeights.Bold,
                        Margin = new Thickness(5, 5, 5, 5)
                    };
                    Sign.SetValue(Grid.ColumnProperty, 0);
                    Sign.SetValue(Grid.RowProperty, 0);

                    TextBlock HeaderCaption = new TextBlock() { Text = "Возможно найден заголовок:", FontSize = 24 };
                    TextBlock HeaderCellData = new TextBlock() { Text = "Ячейка: " + primaPair.Key.Name + " '" + primaPair.Key.Value + "'", FontSize = 24 };

                    ListPicker selectionPicker = new ListPicker() { Margin = new Thickness(0, 3, 0, 0) };
                    selectionPicker.Items.Add("да, это так!");
                    selectionPicker.Items.Add("нет, в игнор!");
                    selectionPicker.SetValue(Grid.ColumnProperty, 0);
                    Button HeaderAceptionBtn = new Button() { Content = "подтвердить" };
                    HeaderAceptionBtn.SetValue(Grid.ColumnProperty, 1);
                    
                    panel.Children.Add(HeaderCaption);
                    panel.Children.Add(HeaderCellData);
                    
                    selectionGrid.Children.Add(selectionPicker);
                    selectionGrid.Children.Add(HeaderAceptionBtn);
                    
                    HeaderArea.Children.Add(Sign);
                    HeaderArea.Children.Add(panel);
                    HeaderArea.Children.Add(selectionGrid);
                    //HeaderPanel.Children.Add(HeaderArea);
                    //HeaderArea.Children.Add(HeaderPanel);
                    PrimaryAnalysisResultArea.Children.Add(HeaderArea);
                }
            } */
        }

        /// <summary>
        /// Работает с областью подтверждения данных, которые были выбраны, как
        /// первичный набор данных. 
        /// </summary>
        /// <param name="index">Порядковый номер в словаре со значениями ячеек первичного набора и их масок</param>
        private void PrimaryAnalysisConfirmation(int index)
        {
            if (index > (selector.PrimaryKeyData.KeyMaskDic.Count - 1))
            { // вообщето, это означает что подтверждение данных закончено.
                ShowPrimaryAnalysisSummary();
                return;
            }

            var primaPair = selector.PrimaryKeyData.KeyMaskDic.ElementAt<KeyValuePair<Data.Cell, Mask>>(index);
            if (primaPair.Value.IsHeader)
            {
                HeaderArea area = new HeaderArea(PrimaryAnalysisResultArea, primaPair);
                area.GoNextStep += NextStepHandler;
                area.Show();
                return;
            }
            if (primaPair.Value.AssIndex == -1)
            {
                // Нужно проверить на <ERROR>, возможно это ошибка ввода данных?
                // В любом случае отрицательный жопоиндекс говорит о том что не все впорядке с данными.
                if (primaPair.Value.MaskSyntax == "<ERROR>")
                {
                    ErrorArea area = new ErrorArea(PrimaryAnalysisResultArea, primaPair);
                    area.GoNextStep += NextStepHandler;
                    area.Show();
                    return;
                }
                else // возможно имеется некая "сложная маска"
                { // в таком случае необходимо проработать вариант объединения ячеек по значению и разделения по индексу.
                    // Для того чтобы объединить нужно выделить необходимое значение из того что записано в ячейке и найти
                    // еще ячейки с такими же значениями.
                    var value = primaPair.Key.GetValueByMask(primaPair.Value.MaskSyntax);
                    // Чтобы поработать с другими ячейками, следует сначала их всех собрать в один список и сделать 
                    // небольшую статистику. Статистику о том, какие маски встречаются.... так чтоли?
                    var sameValArr = selector.PrimaryKeyData.GetSameValueArr(primaPair);
                    if (sameValArr != null)
                    {
                        ComplexMaskArea area = new ComplexMaskArea(PrimaryAnalysisResultArea, sameValArr, selector.PrimaryKeyData);
                        area.GoNextStep += NextStepHandler;
                        area.Show();
                    }
                    else
                    {
                        MessageBox.Show("Не найдено ячеек с одинаковыми значениями!");
                    }

                    return;
                }

                //MessageBox.Show("жопоиндекс отрицателен!");
                //return;
            }
            NextStepHandler();
        }

        /// <summary>
        /// Показывает, то что имеем после подтверждения результатов анализа первичного набора данных.
        /// </summary>
        private void ShowPrimaryAnalysisSummary()
        {
            Grid CaptionArea = new Grid() { Height = 45 };
            TextBlock Caption = new TextBlock()
            {
                Text = "В итоге:",
                FontSize = 28,
                Margin = new Thickness(10, 5, 5, 0)
            };
            CaptionArea.Children.Add(Caption);
            PrimaryAnalysisResultArea.Children.Add(CaptionArea);

            TextBlock infoStr = new TextBlock()
            {
                Text = "Для ключевого поля '" + selectedKeyField.Name + "' найдено " 
                        + selector.PrimaryKeyData.UniqValueCount + " уникальных значений. "
                        + "Для добавления ключевых записей в БД, необходимо заполнить дополнительные поля.",
                FontSize = 24,
                Margin = new Thickness(15, 5, 5, 0),
                TextWrapping = TextWrapping.Wrap
            };
            PrimaryAnalysisResultArea.Children.Add(infoStr);

            Button ContBtn = new Button()
            {
                Content = "Продолжить",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center
            };
            ContBtn.Tap += ContBtn_Tap;
            PrimaryAnalysisResultArea.Children.Add(ContBtn);
        }

        private void ContBtn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HidePrimaryAnalysisResultArea();
            selectedBorder.BorderBrush = new SolidColorBrush(Colors.Red);
            confirmedBorders.Add(selectedBorder);
            selectedBorder = null;
            ShowAdditionalKeyFieldSelectorArea();
        }

        private void HidePrimaryAnalysisResultArea()
        {
            ReselectPrimaryDatasetArea.Visibility = System.Windows.Visibility.Collapsed;
            PrimaryAnalysisResultArea.Visibility = System.Windows.Visibility.Collapsed;

            ReselectPrimaryAnalysisResultArea = new Grid()
            {
                Background = new SolidColorBrush(new Color() { A = 255, R = 60, G = 179, B = 113 }),
                Height = 30
            };
            TextBlock ReselectCaption = new TextBlock()
            {
                Text = "Набор первичных ключевых полей выбран.",
                FontSize = 18,
                Foreground = new SolidColorBrush(Colors.Black),
                Margin = new Thickness(10, 5, 0, 0)
            };
            TextBlock ReselectAction = new TextBlock()
            {
                Text = "  ...  ",
                TextAlignment = TextAlignment.Right,
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.Black),
                Margin = new Thickness(0, -20, 0, 0)
            };
            ReselectPrimaryAnalysisResultArea.Children.Add(ReselectCaption);
            ReselectPrimaryAnalysisResultArea.Children.Add(ReselectAction);
            DataSelector.Children.Add(ReselectPrimaryAnalysisResultArea);
        }

        StackPanel SecondaryKeyDataSelectionArea;
        int nextAddedSecondaryKeyData = 0;
        
        private void ShowAdditionalKeyFieldSelectorArea()
        {
            StatusText.Text = "Добавте дополнительные ключевые поля:";

            if (SecondaryKeyDataSelectionArea != null) SecondaryKeyDataSelectionArea = null;
            SecondaryKeyDataSelectionArea = new StackPanel();
            DataSelector.Children.Add(SecondaryKeyDataSelectionArea);

            nextAddedSecondaryKeyData = 0;
            SecondaryKeyDataAdditon(nextAddedSecondaryKeyData);
        }

        /// <summary>
        /// Предоставляет пользователю возможность выбора вторичных ключевых данных.
        /// </summary>
        /// <param name="next">Условный номер выбираемых данных.</param>
        private void SecondaryKeyDataAdditon(int next)
        {
            switch (next)
            {
                case 0: // "Разделение учетов"
                    WaterCounterIsDivideSelectedArea area = new WaterCounterIsDivideSelectedArea(SecondaryKeyDataSelectionArea);
                    area.GoNext += SecondaryKeyDataConfirmation;
                    break;
                case 1: // "Ф.И.О. собственника"
                    OwnerNameSelectedArea area1 = new OwnerNameSelectedArea(SecondaryKeyDataSelectionArea);
                    area1.GoNext += SecondaryKeyDataConfirmation;
                    break;
                case 2: // "Адрес дома"
                    BuildAdressSelectedArea area2 = new BuildAdressSelectedArea(SecondaryKeyDataSelectionArea);
                    area2.GoNext += SecondaryKeyDataConfirmation;
                    break;
                case 3: // Подведение итогов.
                    SummaryKeyDataSelectionArea area3 = new SummaryKeyDataSelectionArea(SecondaryKeyDataSelectionArea);
                    DBImporter importer = new DBImporter(selector.KeyDataAgregate);
                    importer.CheckExistedKeyRecords();
                    area3.ShowStatusArea(importer.Status);
                    break;
                default:
                    MessageBox.Show("Ошибочка отображения поля выбора вторичных ключевых данных.");
                    break;
            }
        }

        private void SecondaryKeyDataConfirmation(SecondaryKeyDataParam param)
        {
            if (param != null)
            {
                // нужно предусмотреть возможность выбора набора данных из таблицы экселя.
                if (param.Method == ProcessingMethod.byExcelSet)
                {
                    if (param.Addition == null)
                    {
                        SelectSeconadyKeySetFromExcelSheet(param); // Отправляемся выбирать данные из таблицы экселя.
                        return;
                    }
                    selector.AddSecondaryKeyDataSet(param);
                }
                else
                    selector.AddSecondaryKeyDataSet(param);
            }

            nextAddedSecondaryKeyData++;
            SecondaryKeyDataAdditon(nextAddedSecondaryKeyData);
           /* switch (nextAddedSecondaryKeyData)
            {
                case 0: // "Разделение учетов"
                    if (param.Method == ProcessingMethod.byRule) selector.AddSecondaryKeyDataSet(param);
                    break;
                case 1: // "Ф.И.О. собственника"
                    break;
                case 2: // "Адрес дома"
                    break;
                default:
                    MessageBox.Show("Ошибочка отображения поля выбора вторичных ключевых данных.");
                    break;
            } */
        }

        private void SelectSeconadyKeySetFromExcelSheet(SecondaryKeyDataParam param)
        {
            string mode;
            if (selector.SelMode == Presentation.SelectionMode.Col)
                mode = "Выберите столбец:";
            else
                mode = "Выберите строку:";
            selectionValid = true;

            TextBlock tbl = new TextBlock() { Text = mode, FontSize = 24, VerticalAlignment = System.Windows.VerticalAlignment.Center };
            Button btn = new Button() { Content = "подтвердить", HorizontalAlignment = System.Windows.HorizontalAlignment.Right };
            selectionConfitmationBtn = btn;
            selectionConfitmationBtn.DataContext = param;
            selectionConfitmationBtn.Visibility = System.Windows.Visibility.Collapsed;
            selectionConfitmationBtn.Tap += SecondaryKeySetFromExcelConfirmation;
            Grid area = new Grid() { Height = 73 };

            area.Children.Add(tbl);
            area.Children.Add(btn);
            SecondaryKeyDataSelectionArea.Children.Add(area);
        }

        private void SecondaryKeySetFromExcelConfirmation(object sender, System.Windows.Input.GestureEventArgs e)
        {
            selectionValid = false;

            Button btn = sender as Button;
            Grid area = btn.Parent as Grid;
            area.Visibility = System.Windows.Visibility.Collapsed;

            SecondaryKeyDataParam param = btn.DataContext as SecondaryKeyDataParam;
            param.Addition = selector.MaskSelectedInd;

            SecondaryKeyDataConfirmation(param);
        }

        private void NextStepHandler()
        {
            nextExamPair++;
            //excelDataTableFieldTouch = false;
            PrimaryAnalysisConfirmation(nextExamPair);
        }

        private void DataSelector_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DataSelectorScrollViewer.ScrollToVerticalOffset(500);
        }

        private void ContentStack_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ExcelDataTableFieldScrollViewer.Height = 660 - ContentStack.ActualHeight;
            ContentStack.UpdateLayout();
            if (!excelDataTableFieldTouch)
            {
                ExcelDataTableField_LostFocus(new object(), new RoutedEventArgs());
                excelDataTableFieldTouch = false;
            }
        }

        bool excelDataTableFieldTouch = false;

        private void ExcelDataTableField_GotFocus(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("dvdv");
            var ah = DataSelectorScrollViewer.ActualHeight;
            if (ah > 250)
                DataSelectorScrollViewer.Height = 200;
            //DataSelectorScrollViewer.ScrollToVerticalOffset(ah);
        }

        private void ExcelDataTableField_LostFocus(object sender, RoutedEventArgs e)
        {
            DataSelectorScrollViewer.Height = Double.NaN;
            excelDataTableFieldTouch = false;
            //DataSelectorScrollViewer.ScrollToVerticalOffset(500);
        }

        /* этого уже нет
        private void lp_KeyField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lp_KeyField == null) return;
            selectedKeyField = (KeyField)lp_KeyField.SelectedItem;
        } */
    }
}