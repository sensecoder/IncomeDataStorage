using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO.IsolatedStorage;
using System.IO;
using Useful;

namespace IncomeDataStorage.Data
{
    /// <summary>
    /// Парсер данных из XML описания структуры листа экселевского файла.
    /// </summary>
    class ExcelSheetXMLParser
    {
        private string sheetPath;
        private SheetDataDimension dimension;
        private List<Cell> cells;

        // Конструкторы:
        public ExcelSheetXMLParser()
        { }
        public ExcelSheetXMLParser(string path)
        {
            this.sheetPath = path;
        }

        // Свойства:
        /// <summary>
        /// Размерность области данных.
        /// </summary>
        public SheetDataDimension Dimension
        {
            get
            {
                if (dimension == null)
                    return DetectDimension();
                else
                    return dimension;
            }
        }

        /// <summary>
        /// Возвращает коллекцию ячеек с данными.
        /// </summary>
        public List<Cell> Cells
        {
            get
            {
                if (cells == null)
                    return DetectCells();
                else
                    return cells;
            }
        }

        private List<Cell> DetectCells()
        {
            var strings = SharedStrings();
            var dataTypes = TypeFromFmtId();

            if (cells == null)
                cells = new List<Cell>();

            XmlReader reader;
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            using (var fileStream = isf.OpenFile(sheetPath, FileMode.Open))
            {
                //XDocument doc = XDocument.Load(fileStream);
                reader = XmlReader.Create(fileStream);

                var nodeName = "";
                var text = "";
                var endElement = false;
                while (reader.Name != "sheetData") reader.Read();
                while (!endElement)
                {
                    //reader.Read();
                    nodeName = reader.Name;
                    if (nodeName == "c" && reader.NodeType == XmlNodeType.Element)
                    {  // найден элемент с описанием ячейки
                        var r = reader.GetAttribute("r");
                        //if (r == "A2")
                        //    r = "A2";
                        string t = "";
                        if (reader.GetAttribute("t") != null)
                            t = reader.GetAttribute("t");

                        int s = -1;
                        if (reader.GetAttribute("s") != null)
                            s = int.Parse(reader.GetAttribute("s"));

                        var dival = new Useful.DiValue();
                        dival.DiValStrDivider(r);

                        // теперь нужно найти значение ячейки
                        // но его может и не быть.. а ячейки без значений мне не нужны
                        var cellEnd = false;
                        while (!cellEnd)
                        {
                            reader.Read();
                            if (reader.Name == "v" && reader.NodeType == XmlNodeType.Element)
                            { // найдено значение
                                while (reader.NodeType != XmlNodeType.Text) reader.Read();
                                var val = reader.Value;

                                Cell cell = new Cell()
                                {
                                    CollInd = dival.StringValue,
                                    RowInd = dival.IntValue.ToString(),
                                };

                                if (t == "s")
                                { // ячейка содержит строковое значение
                                    cell.Type = SuppDataType.String;
                                    int strInd = int.Parse(val);
                                    cell.Value = strings[strInd];
                                }
                                else
                                { // ячейка содержит значение отличное от строкового
                                    cell.Value = val;
                                    if (s >= 0) cell.Type = dataTypes[s];
                                    else
                                        cell.Type = SuppDataType.Unknown;
                                }

                                cells.Add(cell);
                            }
                            if (reader.Name == "c" || reader.Name == "row")
                                cellEnd = true;
                        }
                    }
                    if (reader.Name == "c" && reader.NodeType == XmlNodeType.Element)
                    { }
                    else reader.Read();
                    if (reader.Name == "sheetData" && reader.NodeType == XmlNodeType.EndElement)
                        endElement = true;
                }
            }

            return cells;
        }

        /// <summary>
        /// Собирает информацию о типе данных в ячейках.
        /// </summary>
        /// <returns></returns>
        private List<SuppDataType> TypeFromFmtId()
        {
            List<SuppDataType> res = new List<SuppDataType>();

            var strPath = System.IO.Path.GetDirectoryName(sheetPath);
            strPath = System.IO.Path.GetDirectoryName(strPath) + "\\styles.xml";

            XmlReader reader;
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            using (var fileStream = isf.OpenFile(strPath, FileMode.Open))
            {
                reader = XmlReader.Create(fileStream);

                var nodeName = "";
                int count = 0;
                reader.Read();
                while (!reader.EOF)
                {
                    nodeName = reader.Name;
                    if (nodeName == "cellXfs" && reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.GetAttribute("count") != null)
                            count= int.Parse(reader.GetAttribute("count"));
                    }
                    if (nodeName == "xf" && reader.NodeType == XmlNodeType.Element)
                    {
                        int numFmtId = -1;
                        if (reader.GetAttribute("numFmtId") != null)
                            numFmtId = int.Parse(reader.GetAttribute("numFmtId"));
                        if (numFmtId >= 0 && numFmtId <= 13)
                            res.Add(SuppDataType.Number);
                        else
                            if (numFmtId >= 14 && numFmtId <= 22)
                                res.Add(SuppDataType.Date);
                            else
                                res.Add(SuppDataType.Unknown);                       
                    }
                    if (count > 0 && res.Count == count) return res;
                    try { reader.Read(); }
                    catch { return res; }
                }
            }

            return res;
        }

        /// <summary>
        /// Находит и возвращает список текстовых значений ячеек листа экселевского файла
        /// </summary>
        private List<string> SharedStrings()
        {
            List<string> res = new List<string>();

            var strPath = System.IO.Path.GetDirectoryName(sheetPath);
            strPath = System.IO.Path.GetDirectoryName(strPath) + "\\sharedStrings.xml";

            XmlReader reader;
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            using (var fileStream = isf.OpenFile(strPath, FileMode.Open))
            {
                reader = XmlReader.Create(fileStream);

                var nodeName = "";
                var text = "";
                reader.Read();
                while (!reader.EOF)
                {
                    nodeName = reader.Name;
                    if (nodeName == "si" && reader.NodeType == XmlNodeType.Element)
                    {
                        reader.Read();
                        nodeName = reader.Name;
                        while (nodeName != "si") // && reader.NodeType != XmlNodeType.EndElement)
                        {
                            if (nodeName == "t" && reader.NodeType == XmlNodeType.Element)
                            {
                                reader.Read();
                                if (reader.NodeType == XmlNodeType.Text)
                                    text = text + reader.Value;
                            }
                            reader.Read();
                            nodeName = reader.Name;
                        }
                        res.Add(text);
                        text = "";
                    }
                    try { reader.Read(); }
                    catch { return res; }
                }
            }
            return res;
        }

        private SheetDataDimension DetectDimension()
        {
            XmlReader reader;
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            using (var fileStream = isf.OpenFile(sheetPath, FileMode.Open))
                reader = XmlReader.Create(fileStream);

            string nodeName = "";
            while (nodeName != "dimension")
            {
                reader.Read();
                nodeName = reader.Name;
            }
            var dimStr = reader.GetAttribute("ref");
            if (!string.IsNullOrEmpty(dimStr))
            {
                dimension = DimensionFromStr(dimStr);                
            }
            return dimension;
        }
        private SheetDataDimension DimensionFromStr(string dimStr)
        {
            // Cтрока должна быть вида "X1Y1:X2Y2",
            // где X1,X2 - строковое значение, указывающее на индекс столбца
            //     Y1,Y2 - числовое значение, указывает на индекс строки.
            // Назову конструкцию XY DiVal (типа двойное значение). С ними будут работать классы из Useful.
            // Но сначала разобъю строку по разделителю ':'.
            var vals = dimStr.Split(new char[] { ':' });
            SheetDataDimension res = new SheetDataDimension();
            DiValue dival = new DiValue();
            dival = dival.DiValStrDivider(vals[0]);
            if (dival != null)
            {
                res.TopLeftCell = new Cell()
                {
                    CollInd = dival.StringValue,
                    RowInd = dival.IntValue.ToString()
                };                
            }
            else return null;
            dival = dival.DiValStrDivider(vals[1]);
            if (dival != null)
            {
                res.BottomRightCell = new Cell()
                {
                    CollInd = dival.StringValue,
                    RowInd = dival.IntValue.ToString()
                };            
            }
            else return null;
            return res;
        }

        
    }
}
