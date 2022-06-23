using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IncomeDataStorage
{
    /// <summary>
    /// Вторичные ключевые данные хранятся тут.
    /// </summary>
    public class SecondaryKeyDataSet
    {
        /// <summary>
        /// Имя поля в записи ключевых данных.
        /// </summary>
        public string FieldName { get; set; }

        private List<Object> dataSet;
        /// <summary>
        /// Набор данных.
        /// </summary>
        public List<Object> DataSet
        {
            get
            {
                if (dataSet == null) dataSet = new List<object>();
                return dataSet;
            }
            set
            {
                if (dataSet == null) dataSet = new List<object>();
                dataSet = value;
            }
        }
    }
}
