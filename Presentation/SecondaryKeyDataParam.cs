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

namespace IncomeDataStorage
{
    /// <summary>
    /// Агрегат параметров, которые передаются в процедуру заполнения набора 
    /// вторичных ключевых параметров.
    /// </summary>
    public class SecondaryKeyDataParam
    {
        /// <summary>
        /// Имя поля в записи ключевых данных
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// Метод заполнения
        /// </summary>
        public ProcessingMethod Method { get; set; }
        /// <summary>
        /// Дополнительная инфа, которая зависит от метода заполнения
        /// </summary>
        public Object Addition { get; set; }
    }

    public enum ProcessingMethod
    {
        byRule,
        byExcelSet,
        byAllTheSame
    }
}
