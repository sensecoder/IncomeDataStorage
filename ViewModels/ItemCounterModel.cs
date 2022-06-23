using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace IncomeDataStorage
{
    public class ItemCounterModel : INotifyPropertyChanged
    {
        private string _counterName;

        public string CounterName
        {
            get
            {
                return _counterName;
            }
            set
            {
                if (value != _counterName)
                {
                    _counterName = value;
                    NotifyPropertyChanged("CounterName");
                }
            }
        }

        private string _currData;

        public string CurrData
        {
            get
            {
                return _currData;
            }
            set
            {
                if (value != _currData)
                {
                    _currData = value;
                    NotifyPropertyChanged("CurrData");
                }
            }
        }

        private string _preData;

        public string PreData
        {
            get
            {
                return _preData;
            }
            set
            {
                if (value != _preData)
                {
                    _preData = value;
                    NotifyPropertyChanged("PreData");
                }
            }
        }

        private string _difference;

        public string Difference
        {
            get
            {
                return _difference;
            }
            set
            {
                if (value != _difference)
                {
                    _difference = value;
                    NotifyPropertyChanged("Difference");
                }
            }
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
