using System.Collections.Generic;


namespace IncomeDataStorage.Data
{

    /// <summary>
    /// Содержит первичный набор данных из таблицы экселя и маски ассоциаций,
    /// которые объясняют каким образом эти данные определяют порядок в таблице.
    /// </summary>
    public class PrimaryKeyDataSet
    {
        public string BDTableFieldName;
        public SuppDataType BDTableDataType;

        private List<string> uniqValList; // Список уникальных значений в наборе данных.

        /// <summary>
        /// Список уникальных значений в наборе данных (нужно проверять на null)
        /// </summary>
        public List<string> UniqValList
        {
            get
            {
                return uniqValList;
            }
        }

        /// <summary>
        /// Возвращает количество уникальных значений в первичном наборе данных.
        /// И за одно формирует список этих уникальных значений (UniqValList).
        /// </summary>
        public int UniqValueCount
        {
            get
            {
                uniqValList = new List<string>();
                uniqValList.Clear();
                foreach (var pair in keyMaskDic)
                {
                    if (pair.Value.HasValue)
                        if (!pair.Value.IsComplexMask)
                        {
                            if (!uniqValList.Contains(pair.Key.Value))
                                uniqValList.Add(pair.Key.Value);
                        }
                        else // Если маска составная
                        {
                            if (pair.Key.ValueWithoutMask == null)
                                pair.Key.GetValueByMask(pair.Value.MaskSyntax);
                            if (!uniqValList.Contains(pair.Key.ValueWithoutMask))
                                uniqValList.Add(pair.Key.ValueWithoutMask);
                        }
                }
                return uniqValList.Count;
            }
        }

        private Dictionary<Cell, Mask> keyMaskDic;
        /// <summary>
        /// Словарь, содержащий весь набор ячеек и масок, которые эту ячейку покрывают.
        /// </summary>
        public Dictionary<Cell, Mask> KeyMaskDic
        {
            get
            {
                if (keyMaskDic == null)
                    keyMaskDic = new Dictionary<Cell, Mask>();
                return keyMaskDic;
            }
            set
            {
                if (keyMaskDic == null)
                    keyMaskDic = new Dictionary<Cell, Mask>();
                keyMaskDic = value;
            }
        }

        /// <summary>
        /// Свойство, возвращающее общее количество ассоциативных индексов
        /// встречающихся в наборе данных.
        /// </summary>
        public int AllAssIndexCount
        {
            get
            {
                if (keyMaskDic != null)
                {
                    List<int> Indexes = new List<int>();
                    Indexes.Clear();
                    foreach (var pair in keyMaskDic)
                    {
                        if (pair.Value.AssIndex >= 0)
                            if (!Indexes.Contains(pair.Value.AssIndex))
                                Indexes.Add(pair.Value.AssIndex);
                    }
                    return Indexes.Count;
                }
                else return -1;
            }
        }

        /// <summary>
        /// Возвращает массив пар ключ-значение с одинаковым значением Value, "очищенным" от маски.
        /// </summary>
        /// <param name="pair">Существующая пара ключ-значение.</param>
        /// <returns>Массив пар ключ-значение с одинаковой Value, null в случае если не найдено ни одного совпадения по Value</returns>
        public KeyValuePair<Cell, Mask>[] GetSameValueArr(KeyValuePair<Data.Cell, Mask> pair)
        {
            if (pair.Key.ValueWithoutMask == null) pair.Key.GetValueByMask(pair.Value.MaskSyntax);
            var value = pair.Key.ValueWithoutMask;

            List<KeyValuePair<Data.Cell, Mask>> res = new List<KeyValuePair<Cell, Mask>>();
            res.Add(pair);

            if (value != null)
            {
                foreach (var samepair in keyMaskDic)
                {
                    if (!samepair.Equals(pair))
                    {
                        if (samepair.Value.IsComplexMask)
                        {
                            if (samepair.Key.ValueWithoutMask == null) samepair.Key.GetValueByMask(samepair.Value.MaskSyntax);
                            if (samepair.Key.ValueWithoutMask == value) res.Add(samepair);
                        }
                    }
                }
                if (res.Count > 1) return res.ToArray();
                else return null;
            }
            else
                return null; // типа изначально не правильно.
        }
    }
}