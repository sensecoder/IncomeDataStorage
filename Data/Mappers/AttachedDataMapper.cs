using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;
using System;

namespace IncomeDataStorage.Data
{
    [Index(Columns = "DateOfIncome", Name = "date_Of_Income")]
    [Index(Columns = "DateOfRedaction", Name = "date_Of_Redaction")]
    [Table(Name = "AttachedData")]
    public class AttachedDataMapper : INotifyPropertyChanged, INotifyPropertyChanging, IComparable
    {
        private int id;                     // порядковый номер записи в таблице
        // Данные:
        private int keyId;                  // связывает с записью в таблице ключевых данных
        private int hotwaterMain;           // ГВС(общий счетчик или кухня)
        private int hotwaterSecondary;      // ГВС(ванная)
        private int coldwaterMain;          // ХВС(общий счетчик или кухня)
        private int coldwaterSecondary;     // ХВС(ванная)
        private int electricity;            // электричество 
        private DateTime dateOfIncome;      // дата первичного поступления записи
        private int noOfRedaction;          // номер редакции записи
        private DateTime dateOfRedaction;   // дата редакции.
        private bool validate;              // валидность записи (указывает на возможность включения ее в отчеты и в расчеты)
        private bool deleted;               // флаг, который тупо указывает удалена ли запись.
        private string note;                // примечание, которое может быть присоединено к записи.
                        
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
        public int KeyId
        {
            get { return keyId; }
            set
            {
                NotifyPropertyChanging("KeyId");
                keyId = value;
                NotifyPropertyChanged("KeyId");
            }
        }

        [Column]
        public DateTime DateOfIncome
        {
            get { return dateOfIncome; }
            set
            {
                NotifyPropertyChanging("DateOfIncome");
                dateOfIncome = value;
                NotifyPropertyChanged("DateOfIncome");
            }
        }

        [Column]
        public int HotWaterMain
        {
            get { return hotwaterMain; }
            set
            {
                NotifyPropertyChanging("HotWaterMain");
                hotwaterMain = value;
                NotifyPropertyChanged("HotWaterMain");
            }
        }

        [Column]
        public int HotWaterSecondary
        {
            get { return hotwaterSecondary; }
            set
            {
                NotifyPropertyChanging("HotWaterSecondary");
                hotwaterSecondary = value;
                NotifyPropertyChanged("HotWaterSecondary");
            }
        }

        [Column]
        public int ColdWaterMain
        {
            get { return coldwaterMain; }
            set
            {
                NotifyPropertyChanging("ColdWaterMain");
                coldwaterMain = value;
                NotifyPropertyChanged("ColdWaterMain");
            }
        }

        [Column]
        public int ColdWaterSecondary
        {
            get { return coldwaterSecondary; }
            set
            {
                NotifyPropertyChanging("ColdWaterSecondary");
                coldwaterSecondary = value;
                NotifyPropertyChanged("ColdWaterSecondary");
            }
        }

        [Column]
        public int Electricity
        {
            get { return electricity; }
            set
            {
                NotifyPropertyChanging("Electricity");
                electricity = value;
                NotifyPropertyChanged("Electricity");
            }
        }

        [Column]
        public int NoOfRedaction
        {
            get { return noOfRedaction; }
            set
            {
                NotifyPropertyChanging("NoOfRedaction");
                noOfRedaction = value;
                NotifyPropertyChanged("NoOfRedaction");
            }
        }
        
        [Column]
        public DateTime DateOfRedaction
        {
            get { return dateOfRedaction; }
            set
            {
                NotifyPropertyChanging("DateOfRedaction");
                dateOfRedaction = value;
                NotifyPropertyChanged("DateOfRedaction");
            }
        }

        [Column]
        public bool Validate
        {
            get { return validate; }
            set
            {
                NotifyPropertyChanging("Validate");
                validate = value;
                NotifyPropertyChanged("Validate");
            }
        }

        [Column]
        public bool Deleted
        {
            get { return deleted; }
            set
            {
                NotifyPropertyChanging("Deleted");
                deleted = value;
                NotifyPropertyChanged("Deleted");
            }
        }

        [Column]
        public string Note
        {
            get { return note; }
            set
            {
                NotifyPropertyChanging("Note");
                note = value;
                NotifyPropertyChanged("Note");
            }
        }

        [Column(IsVersion = true)]
        private Binary version;

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

            AttachedDataMapper otherADM = obj as AttachedDataMapper;
            if (otherADM != null)
                return this.DateOfRedaction.CompareTo(otherADM.DateOfRedaction);
            else
                throw new ArgumentException("Object is not a AttachedDataMapper");
        }
    }
}
