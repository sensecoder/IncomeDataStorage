using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace IncomeDataStorage
{
    public class ItemViewModel : INotifyPropertyChanged
    {
        public ItemViewModel()
        {
          //  CounterItems = new ObservableCollection<ItemCounterModel>();
        }

        private SolidColorBrush _elemColor;

        public SolidColorBrush ElemColor
        {
            get
            {
                return _elemColor;
            }
            set
            {
                if (value != _elemColor)
                {
                    _elemColor = value;
                    NotifyPropertyChanged("ElemColor");
                }
            }
        }

        private Visibility _redactedVisible;
        /// <summary>
        /// Опеределяет видимость метки о том что запись была отредактирована
        /// </summary>
        public Visibility RedactedVisible
        {
            get
            {
                return _redactedVisible;
            }
            set
            {
                if (value != _redactedVisible)
                {
                    _redactedVisible = value;
                    NotifyPropertyChanged("RedactedVisible");
                }
            }
        }
        
        private int _attDataID;
        /// <summary>
        /// Идентификационный номер записи в таблице присоединенных данных
        /// </summary>
        public int AttDataID
        {
            get
            {
                return _attDataID;
            }
            set
            {
                if (value != _attDataID)
                {
                    _attDataID = value;
                   // тут не нужно никакого оповещения... NotifyPropertyChanged("AttDataID");
                }
            }
        }

        private string _floorNumber;
        /// <summary>
        /// Пример свойства ViewModel; это свойство используется в представлении для отображения его значения с помощью привязки.
        /// </summary>
        /// <returns></returns>
        public string FloorNumber
        {
            get
            {
                return _floorNumber;
            }
            set
            {
                if (value != _floorNumber)
                {
                    _floorNumber = value;
                    NotifyPropertyChanged("FloorNumber");
                }
            }
        }

        private string _ownerName;
        /// <summary>
        /// Пример свойства ViewModel; это свойство используется в представлении для отображения его значения с помощью привязки.
        /// </summary>
        /// <returns></returns>
        public string OwnerName
        {
            get
            {
                return _ownerName;
            }
            set
            {
                if (value != _ownerName)
                {
                    _ownerName = value;
                    NotifyPropertyChanged("OwnerName");
                }
            }
        }

        private string _incomeDate;
        /// <summary>
        /// Пример свойства ViewModel; это свойство используется в представлении для отображения его значения с помощью привязки.
        /// </summary>
        /// <returns></returns>
        public string IncomeDate
        {
            get
            {
                return _incomeDate;
            }
            set
            {
                if (value != _incomeDate)
                {
                    _incomeDate = value;
                    NotifyPropertyChanged("IncomeDate");
                }
            }
        }

        private string _incomeTime;
        
        public string IncomeTime
        {
            get
            {
                return _incomeTime;
            }
            set
            {
                if (value != _incomeTime)
                {
                    _incomeTime = value;
                    NotifyPropertyChanged("IncomeTime");
                }
            }
        }

        // вот задачка... нужно внутри этого объекта создать коллекцию в которой бы отображалась 
        // необходимая конфигурация учетов.
        // получается что это будет коллекция неких новых объектов...
        // вложение во вложение...

        public ObservableCollection<ItemCounterModel> CounterItems { get; set; }

        // так чтоле... работает!!!

     /*   private string _currHotWaterMain;

        public string СurrHotWaterMain
        {
            get
            {
                return _currHotWaterMain;
            }
            set
            {
                if (value != _currHotWaterMain)
                {
                    _currHotWaterMain = value;
                    NotifyPropertyChanged("CurrHotWaterMain");
                }
            }
        }

        private string _preHotWaterMain;

        public string PreHotWaterMain
        {
            get
            {
                return _preHotWaterMain;
            }
            set
            {
                if (value != _preHotWaterMain)
                {
                    _preHotWaterMain = value;
                    NotifyPropertyChanged("PreHotWaterMain");
                }
            }
        }

        private string _diffHotWaterMain;

        public string DiffHotWaterMain
        {
            get
            {
                return _diffHotWaterMain;
            }
            set
            {
                if (value != _diffHotWaterMain)
                {
                    _diffHotWaterMain = value;
                    NotifyPropertyChanged("DiffHotWaterMain");
                }
            }
        }

        private string _currHotWaterSecondary;

        public string СurrHotWaterSecondary
        {
            get
            {
                return _currHotWaterSecondary;
            }
            set
            {
                if (value != _currHotWaterSecondary)
                {
                    _currHotWaterSecondary = value;
                    NotifyPropertyChanged("CurrHotWaterSecondary");
                }
            }
        }

        private string _preHotWaterSecondary;

        public string PreHotWaterSecondary
        {
            get
            {
                return _preHotWaterSecondary;
            }
            set
            {
                if (value != _preHotWaterSecondary)
                {
                    _preHotWaterSecondary = value;
                    NotifyPropertyChanged("PreHotWaterSecondary");
                }
            }
        }

        private string _diffHotWaterSecondary;

        public string DiffHotWaterSecondary
        {
            get
            {
                return _diffHotWaterSecondary;
            }
            set
            {
                if (value != _diffHotWaterSecondary)
                {
                    _diffHotWaterSecondary = value;
                    NotifyPropertyChanged("DiffHotWaterSecondary");
                }
            }
        }

        private string _currColdWater;

        public string СurrColdWater
        {
            get
            {
                return _currColdWater;
            }
            set
            {
                if (value != _currColdWater)
                {
                    _currColdWater = value;
                    NotifyPropertyChanged("СurrColdWater");
                }
            }
        }

        private string _preColdWater;

        public string PreColdWater
        {
            get
            {
                return _preColdWater;
            }
            set
            {
                if (value != _preColdWater)
                {
                    _preColdWater = value;
                    NotifyPropertyChanged("PreColdWater");
                }
            }
        }

        private string _diffColdWater;

        public string DiffColdWater
        {
            get
            {
                return _diffColdWater;
            }
            set
            {
                if (value != _diffColdWater)
                {
                    _diffColdWater = value;
                    NotifyPropertyChanged("DiffColdWater");
                }
            }
        }

        private string _currElectricity;

        public string СurrElectricity
        {
            get
            {
                return _currElectricity;
            }
            set
            {
                if (value != _currElectricity)
                {
                    _currElectricity = value;
                    NotifyPropertyChanged("CurrElectricity");
                }
            }
        }

        private string _preElectricity;

        public string PreElectricity
        {
            get
            {
                return _preElectricity;
            }
            set
            {
                if (value != _preElectricity)
                {
                    _preElectricity = value;
                    NotifyPropertyChanged("PreElectricity");
                }
            }
        }

        private string _diffElectricity;

        public string DiffElectricity
        {
            get
            {
                return _diffElectricity;
            }
            set
            {
                if (value != _diffElectricity)
                {
                    _diffElectricity = value;
                    NotifyPropertyChanged("DiffElectricity");
                }
            }
        }  */

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