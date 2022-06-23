using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
//using System.Threading.Task;
using System.Net;
using Microsoft.Live;
using Microsoft.Live.Controls;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Useful;
using System.IO;
using System.IO.IsolatedStorage;

namespace IncomeDataStorage.Presentation
{
    //  delegate LiveOperationCompletedEventArgs GetCompleteEventHandler();
    public delegate void ProgressStart();
    public delegate void ProgressStop();

    public class SkyConnect : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        //public event GetCompleteEventHandler GetComplete;

        public event ProgressStart progressStart;
        public event ProgressStop progressStop;

        private LiveConnectSessionStatus conStat; //Microsoft.Live.LiveConnectSessionStatus
        private LiveConnectSession _session;
        private LiveConnectClient _client;
        private bool connected;
        private string str;
        private string fileChooseAreaCaption = "Выберите файл:";
        private ObservableCollection<FileListItemViewModel> fileListItems;
        private Queue<string> subFoldersPaths;
        //private string conStat;

        /// <summary>
        /// Коллекция объектов для списка файлов.
        /// </summary>
        public ObservableCollection<FileListItemViewModel> FileListItems
        {
            get
            {
                // Отложить создание модели представления до необходимости
                if (fileListItems == null)
                    fileListItems = new ObservableCollection<FileListItemViewModel>();

                return fileListItems;
            }
        }

        public bool Connected
        {
            get
            {
                return connected;
            }
            set
            {
                connected = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Connected"));
                } 
            }
        }
        public string Session
        {
            get
            {
                try
                {
                    string s = _session.AccessToken;
                    return "Подключение выполнено!";
                }
                catch
                {
                    return "Сессия неактивна...";
                }
            }
        }
        public LiveConnectSession session
        {
            get
            {
                return _session;
            }
            set
            {
                _session = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Session"));
                } 
            }
        }
        public string Str
        {
            set
            {
                str = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Client"));
                }
            }
        }
        public string Client
        {
            get
            {
                try
                {
                    string s = _session.RefreshToken;
                    return str;
                }
                catch
                {
                    return "Клиент не авторизован...";
                }
            }
        }
        public LiveConnectClient client
        {
            get
            {
                return _client;
            }
            set
            {
                _client = value;
               /* if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Client"));
                } */
            }
        }        
        public string ConStat
        {
            get
            {
                return conStat.ToString();
            }
            set 
            {
                //conStat = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ConStat"));
                } 
            }
        }
        public string FileChooseAreaCaption
        {
            get
            {
                return fileChooseAreaCaption;
            }
            set
            {
                fileChooseAreaCaption = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("FileChooseAreaCaption"));
                }
            }
        }

        // конструктор:
        public SkyConnect()
        {
            Connected = false;                     
        }

        /// <summary>
        /// Заполняет список файлов, данные о которых "вытаскиваются" из ответа, полученного
        /// со SkyDrive. Т.к. требуются только файлы .xlsx, то в этом методе задействован соответствующий фильтр.
        /// </summary>
        public void LoadFileList()
        {
            if (fileListItems == null)
                fileListItems = new ObservableCollection<FileListItemViewModel>();
            // fileListItems.Clear();
            if (str != "")
            {
                Useful.LiveOperationRawResultParser parser = new LiveOperationRawResultParser(str);
                List<LiveFileData> FileList;
                FileList = parser.GetFilesData();
                if (FileList == null)
                {
                    MessageBox.Show("Объектов не найдено!");
                    return;
                }
                else
                {
                    //fileListItems.Clear();
                    foreach (var data in FileList)
                    {
                        if (data.type == "file" && (data.name.EndsWith(".xlsx") || data.name.EndsWith(".XLSX")))
                        {
                            fileListItems.Add(new FileListItemViewModel()
                            {
                                FileID = data.id,
                                FileName = data.name,
                                FilePath = data.upload_location
                            });
                        }
                        if (data.count > 0 && data.type == "folder")
                            subFoldersPaths.Enqueue(data.id);
                    }
                    if (subFoldersPaths.Count > 0)
                        GetFilesStructure(subFoldersPaths.Dequeue());
                }
            }
        }

        public void SessionChanged(LiveConnectSessionChangedEventArgs e)
        {
            if (e.Status == LiveConnectSessionStatus.Connected)
            {
                session = e.Session;
                client = new LiveConnectClient(session);
                _client.GetCompleted += GetCompleteHandler;
                _client.DownloadCompleted += DownloadCompletedHandler;
                Connected = true;
            }
            else
            {
                client = null;
                session = null;
                Connected = false;
            }
        }
        
        public void DoConnect()
        {
           /* try
            {
                Microsoft.Live.LiveAuthClient auth = new LiveAuthClient("ruslanio@live.com");
                auth.InitializeAsync();
                LiveConnectSession sess = auth.Session;

                ConStat = sess.AccessToken;
                // ConStat = "Соединимсу!!!";
            }
            catch (LiveConnectException exception)
            {
                MessageBox.Show("Error: " + exception.Message);
            } */
        }

        /// <summary>
        /// Обработчик ответов с сервиса Live
        /// </summary>
       internal void GetCompleteHandler(object sender, LiveOperationCompletedEventArgs e)
        {
            progressStop();
            // Str = (string)e.Result["data"];
            Str = e.RawResult;
            LoadFileList(); // тут нужно условие поставить нужно ли вообще это делать?
        } 

        /// <summary>
        /// Делает запрос на получение структуры файлов и папок в корне SkyDrive
        /// </summary>
        internal void GetFilesStructure()
        {
            if (subFoldersPaths == null)
                subFoldersPaths = new Queue<string>();
            else subFoldersPaths.Clear();
            try
            {
                progressStart();
                _client.GetAsync("me/skydrive/files"); //LiveOperationResult operationResult =                
            }
            catch (LiveConnectException exception)
            {
            // Display error if operation is unsuccessful.
            }
            //return "";
        }

        /// <summary>
        /// Делает запрос на получение структуры файлов и папок в заданной папке на SkyDrive
        /// </summary>
        /// <param name="Path"></param>
        internal void GetFilesStructure(string Path)
        {
            try
            {
                progressStart();
                _client.GetAsync(Path + "/files"); //LiveOperationResult operationResult =                
            }
            catch (LiveConnectException exception)
            {
                // Display error if operation is unsuccessful.
            }
            //return "";
        }

        internal void TestParser()
        {
            Str = "{\r   \"data\": [\r      {\r         \"id\": \"folder.1b46e11ca598257a.1B46E11CA598257A!104\", \r         \"from\": {\r            \"name\": \"Ruslan Rakhimov\", \r            \"id\": \"1b46e11ca598257a\"\r         }, \r         \"name\": \"Документы\", \r         \"description\": \"\", \r         \"parent_id\": \"folder.1b46e11ca598257a\", \r         \"size\": 15605, \r         \"upload_location\": \"https://apis.live.net/v5.0/folder.1b46e11ca598257a.1B46E11CA598257A!104/files/\", \r         \"comments_count\": 0, \r         \"comments_enabled\": false, \r         \"is_embeddable\": true, \r         \"count\": 1, \r         \"link\": \"https://skydrive.live.com/redir.aspx?cid=1b46e11ca598257a&page=view&resid=1B46E11CA598257A!104&parid=1B46E11CA598257A!101\", \r         \"type\": \"folder\", \r         \"shared_with\": {\r            \"access\": \"Just me\"\r         }, \r         \"created_time\": \"2012-04-20T04:04:03+0000\", \r         \"updated_time\": \"2013-03-23T11:24:09+0000\"\r      }, {\r         \"id\": \"folder.1b46e11ca598257a.1B46E11CA598257A!103\", \r         \"from\": {\r            \"name\": \"Ruslan Rakhimov\", \r            \"id\": \"1b46e11ca598257a\"\r         }, \r         \"name\": \"Общая\", \r         \"description\": \"\", \r         \"parent_id\": \"folder.1b46e11ca598257a\", \r         \"size\": 0, \r         \"upload_location\": \"https://apis.live.net/v5.0/folder.1b46e11ca598257a.1B46E11CA598257A!103/files/\", \r         \"comments_count\": 0, \r         \"comments_enabled\": true, \r         \"is_embeddable\": true, \r         \"count\": 0, \r         \"link\": \"https://skydrive.live.com/redir.aspx?cid=1b46e11ca598257a&page=view&resid=1B46E11CA598257A!103&parid=1B46E11CA598257A!101\", \r         \"type\": \"folder\", \r         \"shared_with\": {\r            \"access\": \"Public\"\r         }, \r         \"created_time\": \"2012-04-20T04:04:03+0000\", \r         \"updated_time\": \"2012-04-20T04:04:03+0000\"\r      }, {\r         \"id\": \"folder.1b46e11ca598257a.1B46E11CA598257A!127\", \r         \"from\": {\r            \"name\": \"Ruslan Rakhimov\", \r            \"id\": \"1b46e11ca598257a\"\r         }, \r         \"name\": \"Показания ИПУ Автозаводская 48\", \r         \"description\": \"\", \r         \"parent_id\": \"folder.1b46e11ca598257a\", \r         \"size\": 36805, \r         \"upload_location\": \"https://apis.live.net/v5.0/folder.1b46e11ca598257a.1B46E11CA598257A!127/files/\", \r         \"comments_count\": 0, \r         \"comments_enabled\": false, \r         \"is_embeddable\": true, \r         \"count\": 2, \r         \"link\": \"https://skydrive.live.com/redir.aspx?cid=1b46e11ca598257a&page=view&resid=1B46E11CA598257A!127&parid=1B46E11CA598257A!101\", \r         \"type\": \"folder\", \r         \"shared_with\": {\r            \"access\": \"Just me\"\r         }, \r         \"created_time\": \"2013-04-22T18:03:58+0000\", \r         \"updated_time\": \"2013-04-24T05:37:29+0000\"\r      }, {\r         \"id\": \"folder.1b46e11ca598257a.1B46E11CA598257A!102\", \r         \"from\": {\r            \"name\": \"Ruslan Rakhimov\", \r            \"id\": \"1b46e11ca598257a\"\r         }, \r         \"name\": \"Фотографии\", \r         \"description\": \"\", \r         \"parent_id\": \"folder.1b46e11ca598257a\", \r         \"size\": 0, \r         \"upload_location\": \"https://apis.live.net/v5.0/folder.1b46e11ca598257a.1B46E11CA598257A!102/files/\", \r         \"comments_count\": 0, \r         \"comments_enabled\": false, \r         \"is_embeddable\": true, \r         \"count\": 0, \r         \"link\": \"https://skydrive.live.com/redir.aspx?cid=1b46e11ca598257a&page=view&resid=1B46E11CA598257A!102&parid=1B46E11CA598257A!101\", \r         \"type\": \"album\", \r         \"shared_with\": {\r            \"access\": \"Just me\"\r         }, \r         \"created_time\": \"2012-04-20T04:04:03+0000\", \r         \"updated_time\": \"2012-04-20T04:04:03+0000\"\r      }\r   ]\r}";
            LoadFileList();
        }

        internal void DownloadFile(FileListItemViewModel FileData)
        {
            // Uri location = new Uri(
            progressStart();
            _client.DownloadAsync(FileData.FileID + "/content", FileData);
            //_client.BackgroundDownloadAsync(FileID, new Uri(@"\shared\transfers"), FileName);
        }

        internal void DownloadCompletedHandler(object sender, LiveDownloadCompletedEventArgs e)
        {
            progressStop();
            if (e.Error == null)
            {
                FileListItemViewModel FileData = e.UserState as FileListItemViewModel;
                try
                {
                    using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                    using (IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream(FileData.FileName, FileMode.Create, isf))
                    {
                        e.Result.CopyTo(fileStream);
                        MessageBox.Show("Загружен файл: " + FileData.FileName);
                    }                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }

            
            
        }
    
    }
}
