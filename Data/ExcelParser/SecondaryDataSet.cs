using System;
using System.Collections.Generic;

namespace IncomeDataStorage.Data.ExcelParser
{
    public class SecondaryDataSet
    {
        public string KeyBDTableFieldName { get; set; }
        public SuppDataType KeyBDTableDataType { get; set; }

        /// <summary>
        /// Список значений, который по индексам коррелирует со словарем в PrimaryDataSet.
        /// </summary>
        public List<Object> Values = new List<object>();
    }
}
