
namespace IncomeDataStorage.Domain
{
    /// <summary>
    /// Ключевые данные, которые однозначно идентифицируют входящую сущность. (Собственник, номер. квартиры)
    /// </summary>
    public class KeyData
    {
        private string name; // фамилия или другой строковый идентификатор собственника
        private int floorNo;  // типа номер квартиры.

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int FloorNo
        {
            get { return floorNo; }
            set { floorNo = value; }
        }
    }
}
