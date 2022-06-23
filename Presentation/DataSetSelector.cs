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
using System.Collections.Generic;
using IncomeDataStorage.Data;
using System.Linq;
using System.Data.Linq;

namespace IncomeDataStorage.Presentation
{
    /// <summary>
    /// Осуществляет работу с набором данных из произвольного столбца или строки
    /// таблицы экселя (набора ячеек).
    /// </summary>
    public class DataSetSelector
    {
        private string selectedExcelInd;
        private ExcelSheetData xSheetData;
        private List<Cell> examineCellsSet;
        private PrimaryKeyDataSet primaryKeyData;

        public string TempData; // нужно для тестирования

        public bool PrimaryKeyDataSetAnalysisComplete = false;

        /// <summary>
        /// Режим выбора, строка или столбец, по-умолчанию столбец
        /// </summary>
        public SelectionMode SelMode = SelectionMode.Col;

        /// <summary>
        /// Индекс выбранного столбца или строки в таблице на экране, 
        /// отображающей экселевский лист.
        /// </summary>
        public int MaskSelectedInd = -1;

        /// <summary>
        /// Первичный набор ключевых данных.
        /// </summary>
        public PrimaryKeyDataSet PrimaryKeyData
        {
            get
            {
                return primaryKeyData;
            }
        }

        private List<SecondaryKeyDataSet> secondaryDataList;
        public List<SecondaryKeyDataSet> SecondaryDataList
        {
            get
            {
                if (secondaryDataList == null) secondaryDataList = new List<SecondaryKeyDataSet>();
                return secondaryDataList;
            }
            private set
            {
                if (secondaryDataList == null) secondaryDataList = new List<SecondaryKeyDataSet>();
                secondaryDataList = value;
            }
        }

        public KeyDataImportArgegate KeyDataAgregate
        {
            get
            {
                return new KeyDataImportArgegate()
                {
                    PrimaryDataSet = primaryKeyData,
                    SecondaryDataList = secondaryDataList
                };
            }
        }

        // Конструкторы:
        public DataSetSelector() { }
        public DataSetSelector(ExcelSheetData SheetData)
        {
            this.xSheetData = SheetData;
        }

        /// <summary>
        /// Служит для анализа первично выбранного набора ячеек.
        /// </summary>
        /// <param name="PrimaryFieldName">Имя свойства в KeysDataMapper которому, 
        /// (предположительно) принадлежат значения в наборе ячеек</param>
        internal void AnalysePrimaryKeyDataSet(string PrimaryFieldName)
        {
            var keysMap = new KeysDataMapper();
            string temp = "";
            var prop = keysMap.GetType().GetProperty(PrimaryFieldName);
            if (prop != null)
                temp = prop.PropertyType.ToString();
            var reqType = ConvertTypeToSupp(temp);
            // Для начала нужно определиться с индексом столбца(строки),
            // какой он есть в оригинальном листе экселя.
            selectedExcelInd = DetectExcelInd(xSheetData.Dimension);
            // По этому параметру можно отсортировать набор ячеек.
            // Создам вот чего:
            primaryKeyData = new PrimaryKeyDataSet();
            primaryKeyData.BDTableFieldName = PrimaryFieldName;
            primaryKeyData.BDTableDataType = reqType;
            //examineCellsSet = new List<Cell>();
            //examineCellsSet.Clear();
            try
            {
                var SetQuery = GetSortedByIndQuery(xSheetData.Cells, selectedExcelInd);
                int c = 1;
                var firstBlank = false;
                // Создам список с "отобранными" ячейками.
                foreach (var cell in SetQuery)
                {
                    //examineCellsSet.Add(cell);
                    // Для самой первой ячейки нужно сделать проверку, а не заголовок ли это?
                    if (c == 1 || firstBlank)
                    {
                        if (cell.Value.Trim() == "")
                        {
                            firstBlank = true;
                            Mask mask = new Mask()
                            {
                                AssIndex = -1,
                                HasValue = false,
                                MaskSyntax = "<SKIP>"
                            };
                            primaryKeyData.KeyMaskDic.Add(cell, mask);
                        }
                        else
                        {
                            firstBlank = false;
                            if (cell.Type == SuppDataType.String) // Т.е. улавливается первая непустая ячейка со строкой символов в виде значения
                            {
                                Mask mask = new Mask()
                                {
                                    AssIndex = -1,
                                    HasValue = false,
                                    IsHeader = true
                                };
                                primaryKeyData.KeyMaskDic.Add(cell, mask);
                            }
                        }
                    }

                    if (c > 1 && !firstBlank)
                    {
                        if (reqType == cell.Type)
                        { // Значица совпали типы поля в таблице БД с типом поля в ячейке.
                            Mask mask = new Mask()
                            {
                                HasValue = true,
                                IsHeader = false,
                                MaskSyntax = "<VALUE>",
                                AssIndex = 0,
                                АssIndexCount = 1
                            }; // Надо бы конэшна сделать проверочку на количество жопоиндексов... но вот как-то не судьба пока...
                            primaryKeyData.KeyMaskDic.Add(cell, mask);
                        }
                        else
                        { // Значица типы не совпали.
                            if (cell.Type == SuppDataType.String)
                            {
                                // Во-первых, нужно проверить, возможно ли, из значения ячейки, 
                                // которое хранится в строковом виде, вытащить необходимый тип.
                                // И как эта запись значения может быть зашифрована с помощью маски.
                                string maskStr = DetectMask(cell.Value, reqType);
                                if (maskStr != "" && maskStr != "<ERROR>")
                                { // ну типа усе упорядке, нашли масочку с выделением из строки значения)
                                    Mask mask = new Mask()
                                    {
                                        HasValue = true,
                                        IsHeader = false,
                                        IsComplexMask = true,
                                        MaskSyntax = maskStr,
                                        AssIndex = -1,
                                        АssIndexCount = -1
                                    }; 
                                    primaryKeyData.KeyMaskDic.Add(cell, mask);
                                }
                                else // что-то не так...
                                {
                                    Mask mask = new Mask()
                                    {
                                        HasValue = false,
                                        IsHeader = false,
                                        MaskSyntax = maskStr,
                                        AssIndex = -1,
                                        АssIndexCount = -1
                                    };
                                    primaryKeyData.KeyMaskDic.Add(cell, mask);
                                }
                            }
                            if (cell.Type == SuppDataType.Date)
                            {
                                // Тип данных в ячейке оказался датой. Вот тут может быть и наебка...
                                // Вообще, от экселя везде нужно ждать наебку. Почему? Да потому.
                                // Есть такой опыт. Во-первых, нужно значение ячейки чекнуть на древность даты.
                                // Вообще в экселе даты идут с 1900 года, так что думаю, что если по значению
                                // получится дата раньше 1950 года, то скорее всего кто-то кого-то наебывает.
                                // И возможно это величина, та что надо величина, но это нужно проверить еще.
                                /*if (reqType == SuppDataType.Number)
                                {
                                    var isReal = Useful.StringOperation.IsRealNumber(cell.Value);
                                    var isInteger = Useful.StringOperation.IsIntNumber(cell.Value);
                                    try
                                    {
                                        if (isReal)
                                        {
                                            var dateVal = double.Parse(cell.Value);
                                            if (dateVal < 18264) // Вот это значит что дата ранее 1950 года.
                                            { // Ну просто сомнительно, почему такая дата тут оказалсь?
                                                // Вдруг какая ошибка...
                                                Mask mask = new Mask()
                                                {
                                                    HasValue = false,
                                                    IsHeader = false,
                                                    MaskSyntax = "<NOTCONFIRMED>",
                                                    AssIndex = 0,
                                                    АssIndexCount = 1
                                                }; // Надо бы конэшна сделать проверочку на количество жопоиндексов... но вот как-то не судьба пока...
                                                primaryDataSet.MaskDic.Add(cell, mask);
                                            }
                                        }
                                    }
                                    catch
                                    {
                                    }
                                } */
                                // Не буду я мудрить особо, просто сделаю маску <ERROR>,
                                // а там уже вручную можно откорректить если чо..
                                Mask mask = new Mask()
                                {
                                    HasValue = false,
                                    IsHeader = false,
                                    MaskSyntax = "<ERROR>",
                                    AssIndex = -1,
                                    АssIndexCount = -1
                                };
                                primaryKeyData.KeyMaskDic.Add(cell, mask);
                            }
                            if (cell.Type == SuppDataType.Unknown)
                            {
                                Mask mask = new Mask()
                                {
                                    HasValue = false,
                                    IsHeader = false,
                                    MaskSyntax = "<ERROR>",
                                    AssIndex = -1,
                                    АssIndexCount = -1
                                };
                                primaryKeyData.KeyMaskDic.Add(cell, mask);
                            }
                        }
                    }
                    c++;
                }
                PrimaryKeyDataSetAnalysisComplete = true;
            }
            catch
            {
                MessageBox.Show("Невозможно обработать данные!");
                PrimaryKeyDataSetAnalysisComplete = false;
            }
            //if (examineCellsSet.Count > 0) // Тобишь получилось)
            //{
                // Сейчас нужно сделать некоторые предположения.
            //}
        }

        /// <summary>
        /// Служит для добавления вторичных ключевых данных.
        /// </summary>
        /// <param name="FieldName">Имя поля записи ключевых данных</param>
        /// <param name="Param">Дополнительный параметр</param>
        internal void AddSecondaryKeyDataSet(SecondaryKeyDataParam Param)
        {
            var secSet = new SecondaryKeyDataSet();
            secSet.FieldName = Param.FieldName;

            switch (Param.FieldName)
            {
                case "WaterCounterIsDivide":
                    if (Param.Method == ProcessingMethod.byRule)
                    {
                        foreach (var primary in primaryKeyData.KeyMaskDic)
                        {
                            if (primary.Value.HasValue)
                            {
                                if (primary.Value.AssIndex == 0)
                                {
                                    if (primary.Value.АssIndexCount == 1)
                                        secSet.DataSet.Add(false); // если всего однин ассоциативный индекс, такое правило.
                                    else
                                        secSet.DataSet.Add(true);
                                }
                                else
                                    secSet.DataSet.Add(null); // без ссылки, поскольку не нужно при ассоциативном индексе больше нуля.
                            }
                            else
                            {
                                secSet.DataSet.Add(null); // без ссылки, раз значения в примари нет.
                            }
                        }
                    }
                    SecondaryDataList.Add(secSet);
                    return;
                case "BuildAdress":
                    if (Param.Method == ProcessingMethod.byAllTheSame)
                    {
                        foreach (var primary in primaryKeyData.KeyMaskDic)
                        {
                            if (primary.Value.HasValue)
                            {
                                if (primary.Value.AssIndex == 0)
                                {
                                    secSet.DataSet.Add(Param.Addition);
                                }
                                else
                                    secSet.DataSet.Add(null); // без ссылки, поскольку не нужно при ассоциативном индексе больше нуля.
                            }
                            else
                            {
                                secSet.DataSet.Add(null); // без ссылки, раз значения в примари нет.
                            }
                        }
                    }
                    SecondaryDataList.Add(secSet);
                    return;
                case "Name":
                    if (Param.Method == ProcessingMethod.byExcelSet)
                    {
                        try
                        {
                            MaskSelectedInd = (int)Param.Addition;
                        }
                        catch
                        {
                            MessageBox.Show("Невозможно определить индекс выбранного набора данных в таблице экселя.");
                            return;
                        }
                        selectedExcelInd = DetectExcelInd(xSheetData.Dimension);
                        try
                        {
                            var SetQuery = GetSortedByIndQuery(xSheetData.Cells, selectedExcelInd);
                            foreach (var cell in SetQuery)
                            {
                                secSet.DataSet.Add(cell.Value);
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Неполучилось сформировать запрос по индексу экселя.");
                            return;
                        }
                    }
                    SecondaryDataList.Add(secSet);
                    return;
            }
        }

        /// <summary>
        /// Определяет, возможно ли применить к значению ячейки некую маску, чтобы 
        /// "выудить" из него данные необходимого типа.
        /// </summary>
        /// <param name="val">Значение ячейки в строковом виде.</param>
        /// <param name="reqType">Необходимый тип данных, значение которого может быть "зашифровано"</param>
        /// <returns></returns>
        private string DetectMask(string val, SuppDataType reqType)
        {
            var chArr = val.ToCharArray();
            var mask = "";
            if (reqType == SuppDataType.Number)
            { // Бум искать число...
                // Безусловно необходимо принять некоторые ограничения.
                // Во-первых, число должно быть одно в строке.
                // Во-вторых, маска может содержать в себе некие части строк до
                // или после числа, которое будет <VALUE>
                // В общем виде маска будет выглядеть так: "возм.строка1<VALUE>возм.строка2"
                var preStr = "";
                var postStr = "";
                var valStr = "";
                var valDetect = false;
                for (int i = 0; i < val.Length; i++)
                {
                    if (chArr[i] < '0' || chArr[i] > '9')
                        if (!valDetect) preStr = preStr + chArr[i];
                        else
                        {
                            // Тут может оказаться ситуация, когда число натуральное
                            // и в нем может оказаться запятушечка ;)
                            if (chArr[i] == ',' || chArr[i] == '.')
                            {
                                if (postStr.Length == 0) // Это значит что пока еще ни одного знака нецифрового небуло
                                    valStr = valStr + chArr[i]; // Запятушечка идет в значение.
                                else postStr = postStr + chArr[i];
                            }
                            else
                                postStr = postStr + chArr[i];
                        }
                    else // обнаружено число!
                    {
                        if (i > 0)
                            if (chArr[i - 1] == '-') // Проверка на отрицательное значение
                            {
                                if (preStr.Length > 0) preStr.Remove(preStr.Length - 1);
                                valStr = "-";
                            }
                        valStr = valStr + chArr[i];
                        valDetect = true;
                    }
                }

                // У алгоритма есть проблемка, а что если в значении окажется две запятые? вот те на...
                chArr = valStr.ToArray();
                var length = valStr.Length;
                valStr = "";
                var finded = false;
                var secondFinded = false;
                var excess = "";
                for (int i = 0; i < length; i++)
                {
                    if (chArr[i] == ',' || chArr[i] == '.')
                    {
                        if (!finded) finded = true;
                        else
                        { // Значится бяка найдена во второй раз.
                            secondFinded = true;
                            //excess = excess + chArr[i];
                        }
                    }
                    if (secondFinded)
                        excess = excess + chArr[i];
                    else
                        valStr = valStr + chArr[i];
                }
                if (excess.Length > 0)
                    postStr = excess + postStr;

                // Ну вот так вроде гладко все должно быть... лирика, йоптеть!
                // Теперь нужно проверить, найдено ли вообще значение?
                if (valStr.Length == 0) mask = "<ERROR>";
                else
                    mask = preStr + "<VALUE>" + postStr;
            }
            // Других вариантов, акромя поиска числа в маске рассматривать не буду.
            return mask;
        }

        private SuppDataType ConvertTypeToSupp(string source)
        {
            switch (source)
            {                
                case "System.Int32" :
                    return SuppDataType.Number;
                case "System.String" :
                    return SuppDataType.String;
                case "System.Boolean" :
                    return SuppDataType.Unknown;
                case "System.DateTime" :
                    return SuppDataType.Date;
                default:
                    return SuppDataType.Unknown;
            }
        }

        private IEnumerable<Cell> GetSortedByIndQuery(List<Cell> Cells, string selectedExcelInd)
        {                    
            if (SelMode == SelectionMode.Col)
            {
                var SetQuery = from Cell cd in Cells
                               where cd.CollInd == selectedExcelInd
                               select cd;
                return SetQuery;
            }

            if (SelMode == SelectionMode.Row)
            {
                var SetQuery = from Cell cd in Cells
                               where cd.RowInd == selectedExcelInd
                               select cd;
                return SetQuery;
            }

            return null;
        }

        private string DetectExcelInd(SheetDataDimension sheetDataDimension)
        {
 	        // так... известно что выбрали (строку/столбец)
            // известен выбранный индекс в маске, а также размерность поля с данными... 
            // косноязычность рулит)
            if (SelMode == SelectionMode.Col)
            {
                // для начала нужно определить сдвиг, первого столбца с данными от самого первого...
                // короче говоря - это будет номер столбца:P.
                var shift = Useful.ExcelColNoCalc.ColNo(sheetDataDimension.TopLeftCell.CollInd);
                var intInd = MaskSelectedInd + shift;
                return Useful.ExcelColNoCalc.ColSymb(intInd);
            }

            if (SelMode == SelectionMode.Row)
            {
                var shift = int.Parse(sheetDataDimension.TopLeftCell.RowInd);
                var intInd = MaskSelectedInd + shift;
                return intInd.ToString();
            }

            return null;
        }
    }

    public enum SelectionMode { Col, Row }

}
