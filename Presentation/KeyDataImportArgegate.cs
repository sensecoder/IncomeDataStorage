using System.Collections.Generic;
using IncomeDataStorage.Data;

namespace IncomeDataStorage
{
    public class KeyDataImportArgegate
    {
        public PrimaryKeyDataSet PrimaryDataSet { get; set; }
        public List<SecondaryKeyDataSet> SecondaryDataList { get; set; }
    }
}
