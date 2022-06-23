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
using IncomeDataStorage.Data;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;

namespace IncomeDataStorage.Presentation
{
    /// <summary>
    /// Класс "знающий" как ковырять экселевские файлы.
    /// </summary>
    public class ExcelFileParser
    {
        private string parsedFilePath;
        private ExcelSheetData sheetData;
        
        /// <summary>
        /// Возвращает данные о листе в файле экселя.
        /// </summary>
        public ExcelSheetData SheetData
        {
            get
            {
                return sheetData;
            }
        }
       
        // конструкторы:
        public ExcelFileParser()
        { }
        public ExcelFileParser(string filePath)
        {
            if (filePath.ToLower().EndsWith(".xlsx"))
                parsedFilePath = filePath;
            else
                MessageBox.Show("Файл не подходит!", "ExcelFileParser", MessageBoxButton.OK);
        }

        /// <summary>
        /// Попытка разархивировать файл.
        /// </summary>
        public void TryToExtract()
        {
            ArchieveWorker extractor = new ArchieveWorker();
            extractor.ExtractZip(parsedFilePath, "temp");
        }

        /// <summary>
        /// Попытка отобразить данные из таблицы экселевского файла.
        /// </summary>
        internal void TryToCollectTableData()
        {
            // ну что... нужне файлик с размерностью таблицы, а вообще этим будет занимться отдельный класс
            sheetData = new ExcelSheetData("temp");
            sheetData.DetectData();
            // Размерность я получаю в виде ячеек, а мне то нужно в цифрах - скока на скока ячеек? 
            // SheetDataDimension может это сразу считать!
            // Следующий этап - это собрать все данные с листа и заполнить ими соответствующую коллекцию...
            // Так... это будет коллекция ячеек.

        }
    }
}
