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
using Microsoft.Live;
using Microsoft.Live.Controls;
using IncomeDataStorage.Domain;
using IncomeDataStorage.Data;

namespace IncomeDataStorage.Presentation
{
    /// <summary>
    /// Обрабатывает запросы связанные с обновлением ключевых данных (о собственниках)
    /// </summary>
    public class KeysDataRefresh
    {
        public void CreateNewKeysColl()
        {
            KeysDataWorker kmap = new KeysDataWorker();
            CollectionOfKeys keys = new CollectionOfKeys();
            keys.NewCollectionOfKeys((IKeysColl)kmap);
        }

        /// <summary>
        /// Загрузить данные co SkyDrive.
        /// </summary>
        public void LoadFromSky()
        {
            SkyConnect scon = new SkyConnect();

            /*  try
            {
                LiveConnectClient liveClient = new LiveConnectClient();
                LiveOperationResult operationResult =
                    await liveClient.GetAsync("folder.8c8ce076ca27823f.8C8CE076CA27823F!126");
                dynamic result = operationResult.Result;
                infoTextBlock.Text = "Folder name: " + result.name + ", ID: " + result.id;
            }   
            catch (LiveConnectException exception)
            {
                infoTextBlock.Text = "Error getting folder info: " + exception.Message;
            } */
        }
    }
}
