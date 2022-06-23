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
using System.ComponentModel;
using Useful;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Navigation;
using IncomeDataStorage.Data;

namespace IncomeDataStorage.Presentation
{
    public delegate void PropertyChanged();
    
    /// <summary>
    /// Работает с полями ввода присоединенной инфы
    /// </summary>
    public class AttachedDataEnterField
    {
        // событие
        public event PropertyChanged PropChanged;
        
        // поля
        private int keyID = -1;
        private bool isActive = false;
        private bool countersIsDivide = false;  // указывает, есть ли разделение учетов по стоякам
        private int hotWaterMain;               // ГВС(общий счетчик или кухня)
        private int hotWaterSecondary;          // ГВС(ванная)
        private int coldWaterMain;              // ХВС(общий счетчик или кухня)
        private int coldWaterSecondary;         // ХВС(ванная)
        private int electricity;
        private hotWaterSwitch
                secondaryHotWaterButtState = 
                        hotWaterSwitch.Add;     // Состояние кнопки работы с разделением учетов по горячей воде
        public ColumnDefinition HotWaterAreaWidth = new ColumnDefinition();

        private double ActualWidth;
        
        private enum hotWaterSwitch
        { Add, Remove }                         

        // свойства
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
            }
        }
        private bool HotWaterIsDivide
        {
            get
            {
                return countersIsDivide;
            }
            set
            {
                countersIsDivide = value;
                if (keyID > 0)
                {
                    KeysDataWorker kdw = new KeysDataWorker(keyID);
                    if (!countersIsDivide)
                    {
                        secondaryHotWaterButtState = hotWaterSwitch.Add;
                        kdw.IsHotWaterDividedSet(false);
                    }
                    else
                    {
                        secondaryHotWaterButtState = hotWaterSwitch.Remove;
                        kdw.IsHotWaterDividedSet(true);
                    }
                }
                OnChanged();
            }
        }
        public SolidColorBrush CaptionColor                 // цвет надписей у полей ввода
        {
            get
            {
                if (isActive)
                    return new SolidColorBrush(Colors.White);
                else 
                    return new SolidColorBrush(Colors.Gray);
            }
        }
        public Visibility SaveButtVisible                   // видимость кнопки сохранения введенных данных
        {
            get
            {
                if (IsDataContains())
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            set
            {
            }
        }
        public Visibility SecondaryHotWaterButtVisible      // видимость кнопки для работы со вторичным учетом горячей воды
        {
            get
            {
                if (keyID>0)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            set
            {

            }
        }
        public SolidColorBrush SecondaryHotWaterButtColor   // цвет этой гребаннной кнопки1!1
        {
            get
            {
                if (secondaryHotWaterButtState == hotWaterSwitch.Add)
                    return new SolidColorBrush(Color.FromArgb(255,107,142,35));
                else
                    return new SolidColorBrush(Color.FromArgb(255,65,105,225));
            }
            set { }
        }
        public string SecondaryHotWaterButtText             // надпись на этой гребанной кнопке !111!!!111
        {
            get
            {
                if (secondaryHotWaterButtState == hotWaterSwitch.Add)
                    return " + ";
                else
                    return " - ";
            }
            set { }
        }
        public Visibility SecondaryHotWaterFieldVisible     // видимость поля для работы со вторичным учетом горячей воды
        {
            get
            {
                if ( countersIsDivide )
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            set
            {

            }
        }
        public Visibility SecondaryColdWaterFieldVisible    // видимость поля для работы со вторичным учетом холодной воды
        {
            get
            {
                if (countersIsDivide)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            set
            {

            }
        }
        public GridLength SecondaryHotWaterColumnWidth      // ширина колонки таблицы где появляется дополнительный учет горячей воды
        {
            get
            {
                if (countersIsDivide)
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
                if (!countersIsDivide)
                    return "ГВС:";
                else
                    return "ГВС(кухня):";
            }
            set { }
        }
        public string ColdWaterMainCaption                   // надпись над полем ввода
        {
            get
            {
                if (!countersIsDivide)
                    return "ХВС:";
                else
                    return "ХВС(кухня):";
            }
            set { }
        }
        
        public string HotWaterMain                          // перобразователь введенного значения
        {         
            get
            {
                if (hotWaterMain == 0)
                    return "";
                else
                    return hotWaterMain.ToString();
            }
            set
            {
                if (StringOperation.IsIntNumber(value))
                {
                    if (value == "")
                        hotWaterMain = 0;
                    else
                        hotWaterMain = int.Parse(value);
                }
                else hotWaterMain = 0;
                OnChanged();
            }
        }
        public string HotWaterSecondary                     // перобразователь введенного значения
        {
            get
            {
                if (hotWaterSecondary == 0)
                    return "";
                else
                    return hotWaterSecondary.ToString();
            }
            set
            {
                if (StringOperation.IsIntNumber(value))
                {
                    if (value == "")
                        hotWaterSecondary = 0;
                    else
                        hotWaterSecondary = int.Parse(value);
                }
                else hotWaterSecondary = 0;
                OnChanged();
            }
        }
        public string ColdWaterMain                          // перобразователь введенного значения
        {
            get
            {
                if (coldWaterMain == 0)
                    return "";
                else
                    return coldWaterMain.ToString();
            }
            set
            {
                if (StringOperation.IsIntNumber(value))
                {
                    if (value == "")
                        coldWaterMain = 0;
                    else
                        coldWaterMain = int.Parse(value);
                }
                else coldWaterMain = 0;
                OnChanged();
            }
        }
        public string ColdWaterSecondary                     // перобразователь введенного значения
        {
            get
            {
                if (coldWaterSecondary == 0)
                    return "";
                else
                    return coldWaterSecondary.ToString();
            }
            set
            {
                if (StringOperation.IsIntNumber(value))
                {
                    if (value == "")
                        coldWaterSecondary = 0;
                    else
                        coldWaterSecondary = int.Parse(value);
                }
                else coldWaterSecondary = 0;
                OnChanged();
            }
        }
        
        public string Electricity                           // перобразователь введенного значения
        {
            get
            {
                if (electricity == 0)
                    return "";
                else
                    return electricity.ToString();
            }
            set
            {
                if (StringOperation.IsIntNumber(value))
                {
                    if (value == "")
                        electricity = 0;
                    else
                        electricity = int.Parse(value);
                }
                else electricity = 0;
                OnChanged();
            }
        }

        /// <summary>
        /// проверяет есть ли введенные значение в одно из полейй ввода
        /// </summary>
        /// <returns></returns>
        internal bool IsDataContains()
        {
            if (hotWaterMain > 0 || hotWaterSecondary > 0 || coldWaterMain > 0 || coldWaterSecondary > 0 || electricity > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// событие сообщает клиентам о том, что произошли изменения в полях объекта
        /// </summary>
        public void OnChanged()
        {
            if (PropChanged != null)
                PropChanged();
        }

        public void Activate(int ID)
        {
            keyID = ID;
            if (!isActive)
            {
                IsActive = true;
            }
            CheckDBState();
        }

        public void DeActivate()
        {
            keyID = -1;
            IsActive = false;
            ClearEnterFields();
            CheckDBState();
        }

        private void ClearEnterFields()
        {
            HotWaterMain = "";
            HotWaterSecondary = "";
            ColdWaterMain = "";
            ColdWaterSecondary = "";
        }
        
        /// <summary>
        /// Проверяет каким должен быть вид ввода данных
        /// А также (хотелось бы) выводит предидущие данные учетов
        /// </summary>
        private void CheckDBState()
        {
            if (keyID > 0)
            {
                KeysDataWorker kdw = new KeysDataWorker(keyID);
                countersIsDivide = kdw.IsHotWaterDivided();
                OnChanged();
            }
            else
            {
                countersIsDivide = false;
                OnChanged();
            }
        }
        
        /// <summary>
        /// Вносит изменение в структуру присоединенных данных.
        /// А именно, добавляет или удаляет дополнительный учет по горячей воде.
        /// Ну и соответственно, изменяет состояние кнопки(переключателя), ответственной за это дело...
        /// </summary>
        public void HotWaterDivideSwitch()
        {   // все операции с БД происходят в свойстве HotWaterIsDivide
            if (!countersIsDivide)
                HotWaterIsDivide = true; // если нет деления, то сделаем разделимым
            else
            {
                HotWaterIsDivide = false;
                hotWaterSecondary = 0;
            }
            OnChanged();
        }

        internal void Save()
        {
            int newID;
            AttachedDataWorker saver = new AttachedDataWorker(keyID);
            newID = saver.AddNew(hotWaterMain, hotWaterSecondary, coldWaterMain, coldWaterSecondary, electricity);
            if (newID < 0) MessageBox.Show("Неполучилось создать запись с присоединенными данными в БД!");
        }
    }
}
