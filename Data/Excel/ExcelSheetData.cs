using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.IO.IsolatedStorage;

namespace IncomeDataStorage.Data
{
    /// <summary>
    /// Расшифровывает и описывает содержание таблицы в файле экселя.
    /// Пока есть ограничения. Класс работает только с первым листом в файле.
    /// </summary>
    public class ExcelSheetData
    {
        private string sourcePath;              // путь к источнику где хранятся файлы, описывающие структуру эксель-файла
        private List<Cell> cells;               // коллекция ячеек с данными находящимися на листе таблицы
        private SheetDataDimension dimension;   // размерность области с данными на листе.
        private bool canExplored = false;       // флаг, указывающий на то что есть все необходимые файлы для обработки 

        /// <summary>
        /// Возвращает размерность области с данными.
        /// </summary>
        public SheetDataDimension Dimension
        {
            get
            {
                return dimension;
            }
        }

        /// <summary>
        /// Возвращает коллекцию ячеек, что находятся на листе в файле экселя.
        /// </summary>
        public List<Cell> Cells
        {
            get
            {
                return cells;
            }
        }

        
        // конструкторы:
        public ExcelSheetData()
        { }
        /// <summary>
        /// Конструктор принимает путь к папке где хранится распаковання структура экселевского файла.
        /// </summary>
        /// <param name="source">Путь к папке со структурой.</param>
        public ExcelSheetData(string source)
        {
            this.sourcePath = source;
            CanExplored();
        }

        /// <summary>
        /// Метод определяет есть ли все необходимые для работы ресурсы.
        /// </summary>
        private void CanExplored()
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
                { canExplored = false; return; }

            using ( IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isf.DirectoryExists(sourcePath))
                    { canExplored = false; return; }
                // теперь надо бы проверить есть ли все необходимые мне файлы с ресурсами...
                if(!isf.FileExists(sourcePath + @"\xl\worksheets\sheet1.xml"))
                    { canExplored = false; return; }
                if (!isf.FileExists(sourcePath + @"\xl\sharedStrings.xml"))
                    { canExplored = false; return; }
            }

            canExplored = true;
        }
        
        /// <summary>
        /// Определяет размерность области с данными
        /// </summary>
        public void DetectData()
        {
            if (!canExplored) { ProcessException(); return; }
            // так... тут нужно будет парсить файл...
            ExcelSheetXMLParser parser = new ExcelSheetXMLParser(sourcePath + @"\xl\worksheets\sheet1.xml");
            dimension = parser.Dimension;
            cells = parser.Cells;
        }

        private void ProcessException()
        {
            MessageBox.Show("Ошибка при обработке документа!", "ExcelSheetData", MessageBoxButton.OK);
        }
    }

    /// <summary>
    /// Описывает размерность области с данными на листе экселевского файла
    /// </summary>
    public class SheetDataDimension
    {
        /// <summary>
        /// Верхняя левая ячейка
        /// </summary>
        public Cell TopLeftCell;
        /// <summary>
        /// Нижняя правая ячейка
        /// </summary>
        public Cell BottomRightCell;

        /// <summary>
        /// Возвращает количество столбцов
        /// </summary>
        public int ColsCount
        {
            get
            {
            // тут очень интересно нужно считать, поскольку столбцы обозначаются буквами...
                var startColNo = Useful.ExcelColNoCalc.ColNo(TopLeftCell.CollInd);
                var stopColNo = Useful.ExcelColNoCalc.ColNo(BottomRightCell.CollInd);
                return stopColNo - startColNo + 1;
            }
        }

        /// <summary>
        /// Возвращает количество строк
        /// </summary>
        public int RowsCount
        {
            get
            {
                var startRowNo = int.Parse(TopLeftCell.RowInd);
                var stopRowNo = int.Parse(BottomRightCell.RowInd);
                return stopRowNo - startRowNo + 1;
            }
        }
    }
}
