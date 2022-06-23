
namespace IncomeDataStorage.Data
{
    /// <summary>
    /// Ячейка таблицы экселя
    /// </summary>
    public class Cell
    {
        public string CollInd;
        public string RowInd;
        public string Name 
        { 
            get
            {
                return CollInd + RowInd;
            }
        }
        public string Value;
        public string ValueWithoutMask = null;
        public SuppDataType Type;

        // конструктор(ы):
        public Cell()
        { }

        /// <summary>
        /// Используя маску, выделяет из значения в ячейке, то значение, 
        /// что "зашифровано" в маске. Если такое возможно, конечно.
        /// </summary>
        /// <param name="Mask">Строковая маска вида: "ххх<VALUE>ххх" </param>
        /// <returns>null в случае ошибки</returns>
        public string GetValueByMask(string Mask)
        {
            string value = "";
            try
            {
                if (Mask == "<VALUE>")
                    value = Value;
                else
                {
                    // Вообщем, нужно отбросить лишнее и оставить то, что спрятано за маркером <VALUE>
                    // т.е. сначала посмотрим, чем начинается маска, т.е. выделим ту часть что стоит перед маркером.
                    // да зачем выделять? нужно просто найти индекс, где начинается маркер, это не сложно.
                    // он же будет совпадать с началом "зашифрованного" значения.
                    var markerStartIndex = Mask.IndexOf("<VALUE>");
                    // теперь нужно выделить из маски "окончание":
                    var postMarkerStr = Mask.Substring(markerStartIndex + 7);
                    // ну и еще, уже в самом значении найти где начинается это "окончание":
                    var valueStopIndex = Value.IndexOf(postMarkerStr);
                    // и вроде как с этими данными можно получить уже ответ:
                    value = Value.Substring(markerStartIndex, (valueStopIndex - markerStartIndex));
                }
            }
            catch { }
            if (value == "") value = null;
            ValueWithoutMask = value;
            return value;
        }
    }

    /// <summary>
    /// Условно принятый мной тип данных (для упрощения работы алгоритма)
    /// </summary>
    public enum SuppDataType
    {
        Unknown,
        String,
        Number,
        Date
    }
}
