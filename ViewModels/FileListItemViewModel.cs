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
using System.ComponentModel;

namespace IncomeDataStorage
{
    /// <summary>
    /// Модель представления элемента списка файлов
    /// </summary>
    public class FileListItemViewModel : INotifyPropertyChanged
    {
        private FileListItemType _itemType;
        public FileListItemType ItemType  // указывает на тип элемента в списке (ненужно?)
        {
            get
            {
                return _itemType;
            }
            set
            {
                _itemType = value;
            }
        }

        private string _fileID;
        public string FileID      // идентификатор файла (оффкосе)
        {
            get
            {
                return _fileID;
            }
            set
            {
                if (value != _fileID)
                {
                    _fileID = value;
                    // NotifyPropertyChanged("FileID");
                }
            }
        }
        
        private string _fileName;
        public string FileName      // имя файла (оффкосе)
        {
            get
            {
                return _fileName;
            }
            set
            {
                if (value != _fileName)
                {
                    _fileName = value;
                    NotifyPropertyChanged("FileName");
                }
            }
        }

        private string _filePath;
        public string FilePath      // полный путь к файлу (оффкосссе)
        {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
            }
        }

        // конструктор:
        public FileListItemViewModel()  // пустой
        { } 

        // сообщатель события:
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// Означает тип элемента в списке файлов. По суте в эксплорере.
    /// </summary>
    public enum FileListItemType
        {
            Folder, Excel, Unknown
        }
}
