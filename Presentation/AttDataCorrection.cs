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
using System.Linq;
using System.ComponentModel;

namespace IncomeDataStorage.Presentation
{
    /// <summary>
    /// Класс представляет слой представления для формы коррекции присоединенных данных.
    /// </summary>
    public class AttDataCorrection : INotifyPropertyChanged
    {
        // поля:
        private AttachedDataMapper correctedData;
        private KeysDataMapper keysData;
        private bool corrected = false;                    // флаг указывает на то, была ли отредактирована запись.
        private int hotwaterMain;                           // ГВС(общий счетчик или кухня)
        private int hotwaterSecondary;                      // ГВС(ванная)
        private int coldwaterMain;                          // ХВС(общий счетчик или кухня)
        private int coldwaterSecondary;                     // ХВС(ванная)
        private int electricity;                            // электричество 

        // свойства:
        private bool Corrected
        {
            get { return corrected; }
            set
            {
                corrected = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SaveBtnVisible"));
                }
            }
        }
        public string CorrID
        {
            get
            {
                string addstr;
                if (correctedData != null)
                    addstr = correctedData.Id.ToString();
                else
                    addstr = "Хуй вам а не данные!";
                return "Идентификатор редактируемых данных: " + addstr;
            }
            set
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CorrID"));
                }
            }
        }
        public string BaseDataDate                          // дата первичного занесения данных. 
        {
            get
            {
                string datestr;
                if (correctedData != null)
                    datestr = correctedData.DateOfIncome.ToString();
                else
                    datestr = "Хуй вам а не данные!";
                return datestr;
            }
            set
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("BaseDataDate"));
                }
            }
        }
        public string FloorNo                               // номер квартиры
        {
            get
            {
                string nostr;
                if (correctedData != null)
                    nostr = keysData.FloorNo.ToString();
                else
                    nostr = "ХX";
                return nostr + "кв.";
            }
            set
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("FloorNo"));
                }
            }
        }
        public string OwnerName                             // номер квартиры
        {
            get
            {
                string namestr;
                if (correctedData != null)
                    namestr = keysData.Name.ToString();
                else
                    namestr = "ХX";
                return namestr;
            }
            set
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("OwnerName"));
                }
            }
        }
        public GridLength SecondaryCounterColumnWidth       // ширина колонки таблицы где появляется дополнительный учет 
        {
            get
            {
                if (keysData.WaterCounterIsDivide)
                    return new GridLength(1, GridUnitType.Star);
                else
                    return new GridLength(1, GridUnitType.Auto);
            }
            set { }
        }
        public string HotWaterMainCaption                   // надпись над полем ввода
        {
            get
            {
                if (!keysData.WaterCounterIsDivide)
                    return "ГВС:";
                else
                    return "ГВС(кухня):";
            }
            set { }
        }
        public string ColdWaterMainCaption                  // надпись над полем ввода
        {
            get
            {
                if (!keysData.WaterCounterIsDivide)
                    return "ХВС:";
                else
                    return "ХВС(кухня):";
            }
            set { }
        }
        public Visibility SecondaryCounterFieldVisible      // видимость поля для работы со вторичным учетом
        {
            get
            {
                if (keysData.WaterCounterIsDivide)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            set
            {

            }
        }
        public Visibility SaveBtnVisible                    // видимость кнопки сохранения изменений
        {
            get
            {
                if (corrected)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            set
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SaveBtnVisible"));
                }
            }
        }
        public string HotWaterSecondary                     // что в поле ввода
        {
            get
            {
                string countstr;
                if (correctedData != null)
                    countstr = correctedData.HotWaterSecondary.ToString();
                else
                    countstr = "ХX";
                return countstr;
            }
            set
            {
                if (value != "")
                    if (Useful.StringOperation.IsIntNumber(value))
                        correctedData.HotWaterSecondary = int.Parse(value);
                    else
                        HotWaterSecondary = correctedData.HotWaterSecondary.ToString();
                else
                    HotWaterSecondary = correctedData.HotWaterSecondary.ToString();

                CheckChanges();

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("HotWaterSecondary"));
                }
            }
        }
        public string HotWaterMain                          // что в поле ввода
        {
            get
            {
                string countstr;
                if (correctedData != null)
                    countstr = correctedData.HotWaterMain.ToString();
                else
                    countstr = "ХX";
                return countstr;
            }
            set
            {
                if (value != "")
                    if (Useful.StringOperation.IsIntNumber(value))
                        correctedData.HotWaterMain = int.Parse(value);
                    else
                        HotWaterMain = correctedData.HotWaterMain.ToString();
                else
                    HotWaterMain = correctedData.HotWaterMain.ToString();

                CheckChanges();

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("HotWaterMain"));
                }
            }
        }
        public string ColdWaterMain                         // что в поле ввода
        {
            get
            {
                string countstr;
                if (correctedData != null)
                    countstr = correctedData.ColdWaterMain.ToString();
                else
                    countstr = "ХX";
                return countstr;
            }
            set
            {
                if (value != "")
                    if (Useful.StringOperation.IsIntNumber(value))
                        correctedData.ColdWaterMain = int.Parse(value);
                    else
                        HotWaterMain = correctedData.ColdWaterMain.ToString();
                else
                    HotWaterMain = correctedData.ColdWaterMain.ToString();

                CheckChanges();

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ColdWaterMain"));
                }
            }
        }
        public string ColdWaterSecondary                    // что в поле ввода
        {
            get
            {
                string countstr;
                if (correctedData != null)
                    countstr = correctedData.ColdWaterSecondary.ToString();
                else
                    countstr = "ХX";
                return countstr;
            }
            set
            {
                if (value != "")
                    if (Useful.StringOperation.IsIntNumber(value))
                        correctedData.ColdWaterSecondary = int.Parse(value);
                    else
                        HotWaterMain = correctedData.ColdWaterSecondary.ToString();
                else
                    HotWaterMain = correctedData.ColdWaterSecondary.ToString();

                CheckChanges();

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ColdWaterSecondary"));
                }
            }
        }

        // конструкторs:
        public AttDataCorrection() { }
        public AttDataCorrection(int ID)
        {
            AttachedDataWorker creator = new AttachedDataWorker();
            correctedData = creator.ReturnMapper(ID);
            keysData = creator.ReturnKeysMapper(correctedData.KeyId);
            hotwaterMain = correctedData.HotWaterMain;
            hotwaterSecondary = correctedData.HotWaterSecondary;
            coldwaterMain = correctedData.ColdWaterMain;
            coldwaterSecondary = correctedData.ColdWaterSecondary;
            electricity = correctedData.Electricity;
        }

        // проверяет были ли внесены изменения в запись
        private void CheckChanges()
        {
            if (correctedData.HotWaterMain != hotwaterMain ||
                correctedData.HotWaterSecondary != hotwaterSecondary ||
                correctedData.ColdWaterMain != coldwaterMain ||
                correctedData.ColdWaterSecondary != coldwaterSecondary ||
                correctedData.Electricity != electricity)
                Corrected = true;
            else Corrected = false;
        }
        
        /// <summary>
        /// Событие, которое класс может генерировать при изменении состояния его свойств
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// сохраняет отредактированные данные в БД.
        /// </summary>
        internal bool SaveData()
        {
            AttachedDataWorker saver = new AttachedDataWorker();
            int res = saver.AddRedacted(correctedData);
            if (res > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Удаляет запись из БД. (вернее помечает как удаленную)
        /// </summary>
        internal void DeleteRecord()
        {
            AttachedDataWorker deleter = new AttachedDataWorker();
            deleter.Delete(correctedData.Id);

        }
    }
}
