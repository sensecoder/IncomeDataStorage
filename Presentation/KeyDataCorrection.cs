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
    /// Класс представляет слой представления для формы коррекции ключевых данных.
    /// </summary>
    public class KeyDataCorrection : INotifyPropertyChanged
    {
        // поля:
        private bool correctionComplete = false;
        private CorrectedField floorNo = new CorrectedField();
        private CorrectedField name = new CorrectedField();
        private Visibility saveBtnVisible = Visibility.Collapsed;

        // свойства:
        public CorrectedField FloorNo
        {
            get
            {
                return floorNo;
            }
            set
            {
                floorNo = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("FloorNo"));
                }
            }
        }
        public CorrectedField Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }
        public Visibility SaveBtnVisible
        {
            get
            {
                return saveBtnVisible;
            }
            set
            {
                saveBtnVisible = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SaveBtnVisible"));
                }
            }
        }
       /* public string FloorNoCaption
        {
            get
            {
                return floorNo.Caption;
            }
        }  */
        
        // конструктор:
        public KeyDataCorrection()
        {
            CheckDB(); 
        }

        /// <summary>
        /// Метод, проверяющий наличие данных в БД.
        /// </summary>
        private void CheckDB()
        {
            IncomeDataContext db = new IncomeDataContext(IncomeDataContext.DBSource);
            try
            {
                var query = from KeysDataMapper data in db.KeysTable where data.Id == App.CorrectionID select data;
                foreach (var data in query)
                {
                    if (data.FloorNo > 0)
                    {
                        FloorNo.Text = data.FloorNo.ToString();
                        FloorNo.State = FieldState.stored;
                    }
                    else
                    {
                        FloorNo.Text = "";
                        FloorNo.State = FieldState.empty;
                    }
                    if (data.Name != "")
                    {
                        Name.Text = data.Name;
                        Name.State = FieldState.stored;
                    }
                    else
                    {
                        Name.Text = "";
                        Name.State = FieldState.empty;
                    }
                    return; // обрабатываем только одну запись, она и должна быть одна.
                }
            }
            catch (Exception e)
            {
                // это вроде как состояние ошибки... описывать лень..
                MessageBox.Show(e.Message);
                return;
            }
        }

        /// <summary>
        /// Метод проверяющий состояние полей на форме.
        /// </summary>
        public void CheckState()
        {
            if (floorNo.State == FieldState.empty || floorNo.State == FieldState.filled)
                if (floorNo.Text != "") FloorNo.State = FieldState.filled;
                else FloorNo.State = FieldState.empty;
            if (name.State == FieldState.empty || name.State == FieldState.filled)
                if (name.Text != "") Name.State = FieldState.filled;
                else Name.State = FieldState.empty;
            if (floorNo.State != FieldState.empty && name.State != FieldState.empty)
                SaveBtnVisible = Visibility.Visible;
            else
                SaveBtnVisible = Visibility.Collapsed;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("FloorNo"));
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        /// <summary>
        /// Метод сохраняет дополненную инфу в БД.
        /// </summary>
        internal void SaveData()
        {
            KeysDataWorker corr = new KeysDataWorker();
            if (FloorNo.State == FieldState.filled)
                corr.FloorNoCorrection(int.Parse(FloorNo.Text));
            if (Name.State == FieldState.filled)
                corr.NameCorrection(Name.Text);
            //App.CorrectionID = -1;
        }
        
        /// <summary>
        /// Событие, которое класс может генерировать при изменении состояния его свойств
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

    }



    public enum FieldState
    {
        stored,
        filled,
        empty
    }

    public class CorrectedField
    {
        public FieldState State { get; set; }
        public string Text { get; set; }

        public string Caption           // значек рядом с полем
        {
            get
            {
                switch (State)
                {
                    case FieldState.empty:
                        return "?";
                    case FieldState.filled:
                        return "...";
                    case FieldState.stored:
                        return "OK";
                    default:
                        return "?";
                }
            }
        }

        public SolidColorBrush Color    // цвет значка 
        {
            get
            {
                switch (State)
                {
                    case FieldState.empty:
                        return new SolidColorBrush(Colors.Yellow);
                    case FieldState.filled:
                        return new SolidColorBrush(Colors.Cyan);
                    case FieldState.stored:
                        return new SolidColorBrush(Colors.Green);
                    default:
                        return new SolidColorBrush(Colors.Yellow);
                }
            }
        }

        public bool Enabled             // задействованность поля для ввода данных
        {
            get
            {
                if (State == FieldState.stored)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
