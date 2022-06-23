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
using System.IO.IsolatedStorage;
using System.IO;
using SharpCompress.Reader;
using System.Collections.Generic;
using System.Linq;

namespace IncomeDataStorage.Data
{
    /// <summary>
    /// Класс для работы с архивами.
    /// </summary>
    public class ArchieveWorker
    {
        /// <summary>
        /// Производит распаковку Zip архива в указанную папку.
        /// </summary>
        /// <param name="ZipArchFilePath">Путь к файлу архива.</param>
        /// <param name="ExtractionDir">Папка, куда будет размещено содержимое архива.</param>
        public void ExtractZip(string ZipArchFilePath, string ExtractionDir)
        {
            if (!string.IsNullOrEmpty(ZipArchFilePath))
            {
                using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
                using (Stream archstream = isf.OpenFile(ZipArchFilePath, FileMode.Open)) //File.OpenRead(@"C:\Code\sharpcompress.rar")
                {
                    var reader = ReaderFactory.Open(archstream);
                    var extrpath = System.IO.Path.GetDirectoryName(ZipArchFilePath) + ExtractionDir;
                    /* try
                    {
                        if (isf.DirectoryExists(extrpath)) // зачистка территории
                        {                         
                            isf.DeleteDirectory(extrpath);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    } */
                    isf.CreateDirectory(extrpath);
                    while (reader.MoveToNextEntry())
                    {
                        var dir = DetectDirPaths(reader.Entry.FilePath);
                        var filename = DetectFileName(reader.Entry.FilePath);
                        string dirpath = "";
                        if (dir != null)
                        {
                            foreach (var dirName in dir)
                            {
                                dirpath += "\\" + dirName;
                                isf.CreateDirectory(extrpath + dirpath);
                            }
                        }
                        var filePath = extrpath + dirpath + "\\" + filename;
                        Stream extrstream = new IsolatedStorageFileStream(filePath, FileMode.OpenOrCreate, isf);
                        StreamWriter fileWriter = new StreamWriter(extrstream);
                        reader.WriteEntryTo(extrstream);
                        fileWriter.Flush();
                        fileWriter.Close();
                    }
                }
            }
            else MessageBox.Show("Недопустимое имя файла!");

        }

        private string DetectFileName(string path)
        {
            if (path.Contains('/'))
            {
                var ind = path.LastIndexOf('/') + 1;
                return path.Substring(ind);
            }
            else return path;
        }

        private List<string> DetectDirPaths(string path)
        {
            if (path.Contains('/'))
            {
                List<string> dirNames = new List<string>();
                var ind = 0;
                var indStart = ind;
                ind = path.IndexOf('/', ind);
                while (ind > 0)
                {
                    dirNames.Add(path.Substring(indStart, ind - indStart));
                    ind++;
                    indStart = ind;
                    ind = path.IndexOf('/', ind);
                }
                return dirNames;
            }
            else return null;
        }
    }
}
