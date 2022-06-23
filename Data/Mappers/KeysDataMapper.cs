using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;
using System;

namespace IncomeDataStorage.Data
{
    [Index(Columns = "FloorNo", Name = "floor_No")]  
    [Table(Name="KeysData")]
    /// <summary>
    /// Маппер, описывающий поля таблицы ключевых данных.
    /// </summary>
    public class KeysDataMapper : INotifyPropertyChanged, INotifyPropertyChanging, IComparable
    {
        private int id;                             // порядковый номер записи в таблице
        // Данные:
        private string buildAdress = "";            // Адрес дома.
        private int floorNo = 0;                    // номер квартиры
        private string name = "";                   // имя собственника
        private bool completed = false;             // флаг заполненности всех полей записи  
        private bool waterCounterIsDivide = false;  // указывает, есть ли разделение учетов по стоякам
       

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Id
        {
            get { return id; }
            set
            {
                NotifyPropertyChanging("Id");
                id = value;
                NotifyPropertyChanged("Id");
            }
        }

        [Column]
        public string BuildAdress
        {
            get { return buildAdress; }
            set
            {
                NotifyPropertyChanging("BuildAdress");
                buildAdress = value;
                NotifyPropertyChanged("BuildAdress");
                IsComplete();
            }
        }

        [Column]
        public int FloorNo
        {
            get { return floorNo; }
            set
            {
                NotifyPropertyChanging("FloorNo");
                floorNo = value;
                NotifyPropertyChanged("FloorNo");
                IsComplete();
            }
        }

        [Column]
        public string Name
        {
            get { return name; }
            set
            {
                NotifyPropertyChanging("Name");
                name = value;
                NotifyPropertyChanged("Name");
                IsComplete();
            }
        }

        [Column]
        public bool Completed
        {
            get { return completed; }
            set
            {
                NotifyPropertyChanging("Completed");
                completed = value;
                NotifyPropertyChanged("Completed");
            }
        }

        [Column]
        public bool WaterCounterIsDivide
        {
            get { return waterCounterIsDivide; }
            set
            {
                NotifyPropertyChanging("WaterCounterIsDivide");
                waterCounterIsDivide = value;
                NotifyPropertyChanged("WaterCounterIsDivide");
            }
        }

        [Column(IsVersion = true)]
        private Binary version;

        /// <summary>
        /// Метод проверяет все ли поля заполнены в полном объеме.
        /// </summary>
        private void IsComplete()
        {
            // Пока не буду учитывать адрес дома
            // if (buildAdress == 0) { Completed = false; return; }
            if (floorNo == 0) { Completed = false; return; }
            if (Name == "") { Completed = false; return; }
            Completed = true;
        }

        // реализация интефейса Notify:
        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // реализация интерфейса IComparable
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            KeysDataMapper otherKDM = obj as KeysDataMapper;
            if (otherKDM != null)
                return this.FloorNo.CompareTo(otherKDM.FloorNo);
            else
                throw new ArgumentException("Object is not a KeysDataMapper");
        }
    }
}
