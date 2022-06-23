using System.Data.Linq;

namespace IncomeDataStorage.Data
{
    public class IncomeDataContext : DataContext
    {
        public static string DBSource = "DataSource=isostore:/IncomeData.sdf";

        public IncomeDataContext(string ConnStr) : base(ConnStr) {}

        public Table<KeysDataMapper> KeysTable;
        public Table<AttachedDataMapper> AttachedTable;
    }
}
