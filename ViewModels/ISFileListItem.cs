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
    public class ISFileListItem : INotifyPropertyChanged
    {
        public bool IsExcelFile { get { return FileName.ToLower().EndsWith(".xlsx"); } }
        
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
        public ISFileListItem()  // пустой
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
}
