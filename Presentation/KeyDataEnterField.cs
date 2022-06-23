using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using IncomeDataStorage.Data;
using IncomeDataStorage.Domain;
using System.ComponentModel;
using Useful;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Navigation;

namespace IncomeDataStorage.Presentation
{
    /// <summary>
    /// Описывает логику работы поля ввода ключевых данных на странице добавления входящей инфы.
    /// </summary>
    public class KeyDataEnterField : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private Visibility enterMaskVisible = Visibility.Visible;   // видимость маски ввода над полем ввода ключевых данных
        private bool enterFieldEnable = true;                       // указывает, задействовано ли поле ввода ключевых данных (tBx_KeyData)
        private Visibility coSVisible;                              // видимость поля вариантов выбора ключевых данных
        private Visibility addNewKeyButtVisible =
                                            Visibility.Collapsed;   // видимость кнопочки добавления новых ключевых данных
        //private Visibility saveButtVisible = Visibility.Collapsed;  // видимость кнопочки сохранения введенных даннх
        public ObservableCollection<CaseOfSelect> CaseOfSelectItems 
                                            { get; private set; }   // коллекция строк как вариантов выбора возможных значений ключевых данных
        private List<KeysDataMapper> keysData;                      // коллекция, дублирующая таблицу ключевых данных в БД
        private int selectedID = -1;                                // это самый важный параметра, тут думать надо....
        private string selectedKeyText = "";                        // текст, который отображается в поле ввода ключевых данных
        private string addButtonSymb = "+";                         // символ на кнопочке добавления ключевых данных.
        private SolidColorBrush addButtonColor = 
                                        new SolidColorBrush();      // цвет кнопочки добавления ключевых данных.

        private AttachedDataEnterField attData = 
                                      new AttachedDataEnterField(); // присоединенные значения.
        

        // свойства реализуют события для биндинга
        public Visibility EnterMaskVisible
        {
            get
            {
                return enterMaskVisible;
            }
            set
            {
                enterMaskVisible = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("EnterMaskVisible"));
                }
            }
        }
        public Visibility CoSVisible
        {
            get
            {
                return coSVisible;
            }
            set
            {
                coSVisible = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CoSVisible"));
                }
            }
        }
        public bool EnterFieldEnable
        {
            get
            {
                return enterFieldEnable;
            }
            set
            {
                enterFieldEnable = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("EnterFieldEnable"));
                }
            }
        }
        public Visibility AddNewKeyButtVisible
        {
            get
            {
                return addNewKeyButtVisible;
            }
            set
            {
                addNewKeyButtVisible = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("AddNewKeyButtVisible"));
                }
            }
        }
       /* public Visibility SaveButtVisible
        {
            get
            {
                if (attData.IsDataContains())
                    saveButtVisible = Visibility.Visible;
                else
                    saveButtVisible = Visibility.Collapsed;
                return saveButtVisible;
            }
            set
            {
                saveButtVisible = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SaveButtVisible"));
                }
            }
        } */
        public string IDmark
        {
            get
            {
                if (selectedID > 0) return selectedID.ToString();
                else return "???";
            }
        }
        public int SelectedID
        {
            get
            {
                return selectedID;
            }
            set
            {
                selectedID = value;
                if (selectedID > 0)
                    attData.Activate(selectedID);
                else
                    attData.DeActivate();
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("AttData"));
                }
            }
        }
        public string SelectedKeyText
        {
            get { return selectedKeyText; }
            set
            {
                selectedKeyText = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedKeyText"));
                }
            }
        }
        public string AddButtonSymb
        {
            get { return addButtonSymb; }
            set
            {
                addButtonSymb = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("AddButtonSymb"));
                }
            }
        }
        public SolidColorBrush AddButtonColor
        {
            get { return addButtonColor; }
            set
            {
                addButtonColor = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("AddButtonColor"));
                }
            }
        }
        public AttachedDataEnterField AttData
        {
            get
            {
                return attData;
            }
            set
            {
                attData = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("AttData"));
                }
            }
        }
        public GridLength HotWaterAreaWidth
        {
            get { return attData.HotWaterAreaWidth.Width; }
            set
            {
                attData.HotWaterAreaWidth.Width = value;
            }
        }

        // конструктор
        public KeyDataEnterField()
        {
            Refresh(true);
            attData.PropChanged += AttachDataCahgedHandler;
            // PropertyChanged(this, new PropertyChangedEventArgs("AttData"));
            //AddButtonColor.BorderThickness = new Thickness(2);          
        }

        internal void Refresh(bool first = false)
        {
            CoSVisible = Visibility.Collapsed;
            if (CaseOfSelectItems == null)
                CaseOfSelectItems = new ObservableCollection<CaseOfSelect>();
            CaseOfSelectItems.Clear();
            KeysDataWorker filler = new KeysDataWorker();
            keysData = filler.FillList();
            if (keysData!=null)
                keysData.Sort();

            if (!first)
            {
                if (App.CorrectionID > 0)
                {
                    SelectedID = App.CorrectionID;
                    App.CorrectionID = -1;
                    CheckEnterKeyData(selectedKeyText);
                }
                
            }
        }

        internal void EnterKeyData()
        {
            EnterMaskVisible = Visibility.Collapsed;
            //EnterFieldEnable = true;
        }

        internal void CheckEnterKeyData(string keydata)
        {
            
            if (keydata.Trim() == "")
            {
                EnterMaskVisible = Visibility.Visible;
                //EnterFieldEnable = false;
                SelectedID = - 1;
            }
            if (selectedID > 0)
            {
                //CoSVisible = Visibility.Collapsed;
                var req = from KeysDataMapper kd in keysData  where kd.Id == selectedID select kd;
                foreach (KeysDataMapper kd in req)
                {
                    if (kd.Completed == false)
                    {
                        AddButtonSymb = "?";
                        AddNewKeyButtVisible = Visibility.Visible;
                        AddButtonColor.Color = Colors.Yellow;
                    }
                    else
                    {
                        AddNewKeyButtVisible = Visibility.Collapsed;
                    }
                }
            }
        }

        /// <summary>
        /// Находит в БД ключевой параметр включающий входящее значение и выбрасывает список выбора под полем ввода.
        /// Либо. Не находит в БД нихуя и тогда открывает возможность добавить новые данные.
        /// </summary>
        /// <param name="text">Входящее значение</param>
        internal void FindKeyData(string text)
        {
            // SelectedID = -1;
            if (text.Trim() == "")
            {
                AddNewKeyButtVisible = Visibility.Collapsed;
                CoSVisible = Visibility.Collapsed;
                SelectedID = -1;
                return;
            }
            if (StringOperation.IsIntNumber(text))
            {
                // ну чо, поехали искать номер квартиры
                KeyDataFinder finder = new KeyDataFinder(keysData);
                List<KeysDataMapper> findresult = finder.SearchFoloorsNoStartWith(text);
                bool contains = false; // флаг, на тот случай, когда список выбора содержит введенное значение.
                if (findresult == null || findresult.Count == 0)
                {
                    AddNewKeyButtVisible = Visibility.Visible;
                    AddButtonSymb = "+";
                    AddButtonColor.Color = Colors.Green;
                    CoSVisible = Visibility.Collapsed;
                    SelectedID = -1;
                    return;
                }
                else
                {
                    this.CaseOfSelectItems.Clear();
                    CoSVisible = Visibility.Visible;
                    foreach (var item in findresult)
                    {
                        CaseOfSelect cos = new CaseOfSelect();
                        cos.SelectLine = item.FloorNo.ToString() + " кв. (" + item.Name + ")";
                        cos.SelectedID = item.Id;
                        this.CaseOfSelectItems.Add(cos);
                        if (item.FloorNo == int.Parse(text))
                        {
                            contains = true;
                            SelectedID = item.Id;
                        }
                    }
                    if (contains) AddNewKeyButtVisible = Visibility.Collapsed;
                    else
                    {
                        AddNewKeyButtVisible = Visibility.Visible;
                        AddButtonSymb = "+";
                        AddButtonColor.Color = Colors.Green;
                        SelectedID = -1;
                    }
                }                
            }
        }

        /// <summary>
        /// Добавляет новые ключевые данные в БД.
        /// </summary>
        internal void AddNewKeyData(string text)
        {
            if (addButtonSymb == "+")
            {   // необходимо добавить новые данные в БД.
                KeysDataWorker adder = new KeysDataWorker(text);
                SelectedID = adder.AddSomeNew();
                keysData.Clear();                   // очищу известный список ключевых значений
                keysData = adder.FillList();        // и снова его заполню
                keysData.Sort();                    // сортировка для порядку
                CoSVisible = Visibility.Collapsed;  // скрыть список выбора возможных значений, виден он или нет - пох, но сейчас точно не нужен.
                CheckEnterKeyData(text);
            }        
        }

        /// <summary>
        /// Делаю выбор в выпадающем списке возможных вариантов.
        /// </summary>
        /// <param name="p"></param>
        internal void SelectFromCaseOfSelectItems(object p)
        {
            TextBlock tb = p as TextBlock;
            CaseOfSelect cos = tb.DataContext as CaseOfSelect;
            SelectedID = cos.SelectedID;
            SelectedKeyText = cos.SelectLine;
            CoSVisible = Visibility.Collapsed;  // скрыть список выбора возможных значений, виден он или нет - пох, но сейчас точно не нужен.
        }

        private void AttachDataCahgedHandler()
        {
            PropertyChanged(this, new PropertyChangedEventArgs("AttData"));
        }

        internal void ClearData()
        {
            SelectedID = 0;
            SelectedKeyText = "";
            CheckEnterKeyData("");
        }
    }
}
