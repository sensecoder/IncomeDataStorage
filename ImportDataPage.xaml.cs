using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Live.Controls;
using Microsoft.Live;
using IncomeDataStorage.Presentation;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.IO;

namespace IncomeDataStorage
{
    public partial class KeyDataManagementPage : PhoneApplicationPage
    {
        private SkyConnect skycon;
        private ProgressIndicator progress;
        public ObservableCollection<ISFileListItem> ISFileList { get; private set; } 
        
        public KeyDataManagementPage()
        {
            InitializeComponent();
            skycon = new SkyConnect();
            ConnectPanel.DataContext = skycon;
            FileChooseArea.DataContext = skycon;
            SkyDriveLoad.DataContext = skycon;

            progress = new ProgressIndicator();
            progress.IsIndeterminate = true;
            SystemTray.SetProgressIndicator(this, progress);

            skycon.progressStart += ProgressStartHandler;
            skycon.progressStop += ProgressStopHandler;
            //FileListBox.ItemsSource = skycon.FileListItems;

           /* var col = ConnStat.Children;
            col.Add(new TextBlock() { Text = "йоу!" }); */
        }

        private void Butt_Click(object sender, RoutedEventArgs e)
        {
            // KeysDataRefresh kdr = new KeysDataRefresh();
            // kdr.LoadFromSky();
            // skycon.TestParser();
            skycon.GetFilesStructure();
        }

        private void btnLogin_SessionChanged(object sender, LiveConnectSessionChangedEventArgs e)
        {
            skycon.SessionChanged(e);
            //if (skycon.Connected) skycon.GetUserName(); 
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MessageBoxResult mesbxres = MessageBox.Show("Загрузить файл?", "Загрузка", MessageBoxButton.OKCancel);
            if (mesbxres == MessageBoxResult.OK)
            {
                TextBlock FileNameText = sender as TextBlock;
                FileListItemViewModel FileListModel = FileNameText.DataContext as FileListItemViewModel;
                skycon.DownloadFile(FileListModel);
            }
        }

        private void ProgressStartHandler() 
        {
            progress.IsIndeterminate = true;
            progress.IsVisible = true;
        }

        private void ProgressStopHandler()
        {
            progress.IsIndeterminate = false;
            progress.IsVisible = false;
        }

        // Обновляет список файлов локального хранилища
        private void TextBlock_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (ISFileList == null)
                ISFileList = new ObservableCollection<ISFileListItem>();
            else
                ISFileList.Clear();
            this.ISFileListBox.ItemsSource = ISFileList;

            // нужно заполнить список файлов.
            Stack<string> pathstack = new Stack<string>();
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
            var Mask = "*.*";
            var currDir = "";
            string[] Directories, Files;
            pathstack.Push(currDir);
            while (pathstack.Count > 0)
            {
                currDir = pathstack.Pop();
                ISFileList.Add(new ISFileListItem() { FileName = currDir });
                Directories = isf.GetDirectoryNames(currDir + "\\*");
                Files = isf.GetFileNames(currDir + "\\" + Mask);
                if (Files.Length > 0)
                    foreach (var name in Files) ISFileList.Add(new ISFileListItem() { FileName = currDir + "\\" + name });
                if (Directories.Length > 0)
                    foreach (var dirname in Directories)
                    {
                        pathstack.Push(currDir + "\\" + dirname);
                    }
            }
            
            if (ISFileList.Count == 0) ISFileList.Add(new ISFileListItem() { FileName = "Не найдено..." } );
        }

        private void ISFileListBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (ISFileListBox.SelectedItem != null)
            {
                ISFileListItem item = ISFileListBox.SelectedItem as ISFileListItem;

                if (item.IsExcelFile)
                {
                    NavigationService.Navigate(new Uri("/ExcelFileParsePage.xaml?FileName="
                                       + Uri.EscapeDataString(item.FileName), UriKind.Relative));
                }
                else
                {
                    MessageBox.Show("Файл не той породы!");
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                isf.CreateDirectory("textFiles"); 
                //Create a new StreamWriter, to write the file to the specified location. 
                StreamWriter fileWriter = new StreamWriter(new IsolatedStorageFileStream("textFiles\\newText.txt", FileMode.OpenOrCreate, isf)); 
                //Write the contents of our TextBox to the file. 
                fileWriter.WriteLine("cdcdcdcdsc"); 
                //Close the StreamWriter. 
                fileWriter.Close();
            }
            MessageBox.Show("файлик создан)");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isf.FileExists("textFiles\\newText.txt"))
                {                
                    isf.DeleteFile("textFiles\\newText.txt");
                    isf.DeleteDirectory("textFiles");
                    MessageBox.Show("Файлик удален!");
                }
                else
                    MessageBox.Show("файлика нету(");
            }
        }
    }
}