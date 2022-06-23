using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace IncomeDataStorage.Presentation
{
    /// <summary>
    /// Вариант выбора возможных значений, которые уже существуют в таблице ключевых данных.
    /// </summary>
    public class CaseOfSelect : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private string selectLine;  // просто информационная строка, которая содержит инфу о номере квартиры и имени собственника
        public int SelectedID;     // а это уже ID записи в таблице ключевых данных, точно идентифицирует ключевую сущность.

        public string SelectLine
        {
            get
            {
                return selectLine;
            }
            set
            {
                selectLine = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectLine"));
                }
            }
        }

    }
}
