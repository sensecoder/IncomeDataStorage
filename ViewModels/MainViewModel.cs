using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using IncomeDataStorage.Presentation;



namespace IncomeDataStorage
{
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Коллекция объектов ItemViewModel.
        /// </summary>
        public ObservableCollection<ItemViewModel> Items { get; private set; }

        public JournalViewParam ViewParam = new JournalViewParam();
        
        public MainViewModel()
        {
            this.Items = new ObservableCollection<ItemViewModel>();
        }

        private string _sampleProperty = "Пример значения свойства среды выполнения";
        /// <summary>
        /// Пример свойства ViewModel; это свойство используется в представлении для отображения его значения с помощью привязки
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Создает и добавляет несколько объектов ItemViewModel в коллекцию элементов.
        /// </summary>
        public void LoadData()
        {
            if (IsDataLoaded)
                this.Items.Clear();

            var journal = new JournalTableData();
            var journalList = journal.AllData(ViewParam);
            journalList.Reverse();
            
            var counterList = new List<ObservableCollection<ItemCounterModel>>();
            int curr, pre, diff;
            //string preStr, diffStr;
            int i = 0;

            foreach (var elem in journalList)
            {
                var counters = new ObservableCollection<ItemCounterModel>();
                var preData = journal.PreData(elem.AttData, journalList);

                if (elem.KeyData.WaterCounterIsDivide)
                {
                    if (elem.AttData.HotWaterMain != 0)
                    {
                        curr = elem.AttData.HotWaterMain;
                        if (preData != null)
                        {
                            pre = preData.HotWaterMain;
                            diff = curr - pre; // не учитывается ошибка если вдруг получится отрицательное значение.
                        }
                        else
                        {
                            pre = 0;
                            diff = 0;
                        }
                        if (curr > 0 && pre > 0)
                            counters.Add(new ItemCounterModel()
                            {
                                CounterName = "ГВ(к):",
                                CurrData = curr.ToString(),
                                PreData = pre.ToString(),
                                Difference = "разница: " + diff.ToString()
                            });
                        if (curr > 0 && pre <= 0)
                            counters.Add(new ItemCounterModel()
                            {
                                CounterName = "ГВ(к):",
                                CurrData = curr.ToString(),
                                PreData = "-",
                            });
                    }

                    if (elem.AttData.HotWaterSecondary != 0)
                    {
                        curr = elem.AttData.HotWaterSecondary;
                        if (preData != null)
                        {
                            pre = preData.HotWaterSecondary;
                            diff = curr - pre; // не учитывается ошибка если вдруг получится отрицательное значение.
                        }
                        else
                        {
                            pre = 0;
                            diff = 0;
                        }
                        if (curr > 0 && pre > 0)
                            counters.Add(new ItemCounterModel()
                            {
                                CounterName = "ГВ(в):",
                                CurrData = curr.ToString(),
                                PreData = pre.ToString(),
                                Difference = "разница: " + diff.ToString()
                            });
                        if (curr > 0 && pre <= 0)
                            counters.Add(new ItemCounterModel()
                            {
                                CounterName = "ГВ(в):",
                                CurrData = curr.ToString(),
                                PreData = "-",
                            });
                    }

                    if (elem.AttData.ColdWaterMain != 0)
                    {
                        curr = elem.AttData.ColdWaterMain;
                        if (preData != null)
                        {
                            pre = preData.ColdWaterMain;
                            diff = curr - pre; // не учитывается ошибка если вдруг получится отрицательное значение.
                        }
                        else
                        {
                            pre = 0;
                            diff = 0;
                        }
                        if (curr > 0 && pre > 0)
                            counters.Add(new ItemCounterModel()
                            {
                                CounterName = "ХВ(к):",
                                CurrData = curr.ToString(),
                                PreData = pre.ToString(),
                                Difference = "разница: " + diff.ToString()
                            });
                        if (curr > 0 && pre <= 0)
                            counters.Add(new ItemCounterModel()
                            {
                                CounterName = "ХВ(к):",
                                CurrData = curr.ToString(),
                                PreData = "-",
                            });
                    }

                    if (elem.AttData.ColdWaterSecondary != 0)
                    {
                        curr = elem.AttData.ColdWaterSecondary;
                        if (preData != null)
                        {
                            pre = preData.ColdWaterSecondary;
                            diff = curr - pre; // не учитывается ошибка если вдруг получится отрицательное значение.
                        }
                        else
                        {
                            pre = 0;
                            diff = 0;
                        }
                        if (curr > 0 && pre > 0)
                            counters.Add(new ItemCounterModel()
                            {
                                CounterName = "ХВ(в):",
                                CurrData = curr.ToString(),
                                PreData = pre.ToString(),
                                Difference = "разница: " + diff.ToString()
                            });
                        if (curr > 0 && pre <= 0)
                            counters.Add(new ItemCounterModel()
                            {
                                CounterName = "ХВ(в):",
                                CurrData = curr.ToString(),
                                PreData = "-",
                            });
                    }
                }
                else
                {
                    if (elem.AttData.HotWaterMain != 0)
                    {
                        curr = elem.AttData.HotWaterMain;
                        if (preData != null)
                        {
                            pre = preData.HotWaterMain;
                            diff = curr - pre; // не учитывается ошибка если вдруг получится отрицательное значение.
                        }
                        else
                        {
                            pre = 0;
                            diff = 0;
                        }
                        if (curr > 0 && pre > 0)
                            counters.Add(new ItemCounterModel()
                            {
                                CounterName = "ГВC:",
                                CurrData = curr.ToString(),
                                PreData = pre.ToString(),
                                Difference = "разница: " + diff.ToString()
                            });
                        if (curr > 0 && pre <= 0)
                            counters.Add(new ItemCounterModel()
                            {
                                CounterName = "ГВC:",
                                CurrData = curr.ToString(),
                                PreData = "-",
                            });
                    }

                    if (elem.AttData.ColdWaterMain != 0)
                    {
                        curr = elem.AttData.ColdWaterMain;
                        if (preData != null)
                        {
                            pre = preData.ColdWaterMain;
                            diff = curr - pre; // не учитывается ошибка если вдруг получится отрицательное значение.
                        }
                        else
                        {
                            pre = 0;
                            diff = 0;
                        }
                        if (curr > 0 && pre > 0)
                            counters.Add(new ItemCounterModel()
                            {
                                CounterName = "ХВC:",
                                CurrData = curr.ToString(),
                                PreData = pre.ToString(),
                                Difference = "разница: " + diff.ToString()
                            });
                        if (curr > 0 && pre <= 0)
                            counters.Add(new ItemCounterModel()
                            {
                                CounterName = "ХВC:",
                                CurrData = curr.ToString(),
                                PreData = "-",
                            });
                    }
                }

                counterList.Add(counters);

                Visibility redacterMark = new Visibility();

                if (elem.AttData.NoOfRedaction > 0)
                    redacterMark = Visibility.Visible;
                else
                    redacterMark = Visibility.Collapsed;

                this.Items.Add(new ItemViewModel()
                {
                    AttDataID = elem.AttData.Id,
                    FloorNumber =elem.KeyData.FloorNo.ToString() + "кв.",
                    OwnerName = elem.KeyData.Name,
                    IncomeDate = elem.AttData.DateOfIncome.ToShortDateString() + "     ",
                    IncomeTime = elem.AttData.DateOfIncome.ToShortTimeString(),
                    CounterItems = counterList[i],
                    ElemColor = GetElemColor(),
                    RedactedVisible = redacterMark
                });

                i++;
            }
            
            // Пример данных; замените реальными данными
            
           /* counters.Add(new ItemCounterModel()
                {
                    CounterName = "ГВC:",
                    CurrData = "32423",
                    PreData = "53422",
                    Difference = "разница: 234"
                });
            counters.Add(new ItemCounterModel()
            {
                CounterName = "ХВС:",
                CurrData = "436465",
                PreData = "345235",
                Difference = "разница: 434"
            });

            this.Items.Add(new ItemViewModel()
            {
                FloorNumber = "10 кв.",
                OwnerName = "Петрученко Федор Кузьмич",
                IncomeDate = "10.02.2012       ",
                IncomeTime = "10:11",
                CounterItems = counters
            });
            this.Items.Add(new ItemViewModel()
            {
                FloorNumber = "25 кв.",
                OwnerName = "Гвоздев Яков Михалыч",
                IncomeDate = "12.02.2012       ",
                IncomeTime = "15:33",
                СurrHotWaterMain = "745",
                PreHotWaterMain = "647",
                DiffHotWaterMain = "105",
                СurrHotWaterSecondary = "789",
                PreHotWaterSecondary = "658",
                DiffHotWaterSecondary = "123",
                СurrColdWater = "804",
                PreColdWater = "750",
                DiffColdWater = "54"
            });
            this.Items.Add(new ItemViewModel()
            {
                FloorNumber = "45 кв.",
                OwnerName = "Челобанов Сергей Викторыч",
                IncomeDate = "22.02.2012       ",
                IncomeTime = "11:25",
                СurrHotWaterMain = "546",
                PreHotWaterMain = "432",
                DiffHotWaterMain = "110",
                СurrHotWaterSecondary = "654",
                PreHotWaterSecondary = "345",
                DiffHotWaterSecondary = "310",
                СurrColdWater = "321",
                PreColdWater = "300",
                DiffColdWater = "21"
            }); */
            
            this.IsDataLoaded = true;
        }

        private SolidColorBrush currCol;
        
        private SolidColorBrush GetElemColor()
        {
            if (currCol == null)
            {
                currCol = new SolidColorBrush(Color.FromArgb(255, 62, 39, 68));
            }
            else
            {
                if (currCol.Color == Color.FromArgb(255, 62, 39, 68))
                    currCol = new SolidColorBrush(Color.FromArgb(255, 47, 38, 49));
                else
                    currCol = new SolidColorBrush(Color.FromArgb(255, 62, 39, 68));
            }
            return currCol;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}