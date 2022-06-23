using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Collections;

namespace IncomeDataStorage.Data
{
    /// <summary>
    /// Служит для добавления импортируемых данных в БД.
    /// </summary>
    public class DBImporter
    {
        private PrimaryKeyDataSet primaryKeyData;
        private List<SecondaryKeyDataSet> secondaryKeyDataList;
        private ImporterStatus status;

        public ImporterStatus Status
        {
            get 
            {
                return status;
            }
        }
        
        // Конструкторы:
        public DBImporter() { }
        public DBImporter(KeyDataImportArgegate KeyAgr)
        {
            primaryKeyData = KeyAgr.PrimaryDataSet;
            secondaryKeyDataList = KeyAgr.SecondaryDataList;
            status = new ImporterStatus();
        }

        /// <summary>
        /// Проверяет существующие в БД записи на возможное совпадение.
        /// </summary>
        public void CheckExistedKeyRecords()
        {
            if (primaryKeyData != null)
            {
                if (primaryKeyData.BDTableFieldName == "FloorNo")
                {
                    // Создам простой список, содержащий все выбранные для занесения в БД номера квартир:
                    List<int> selectedFloorNumbers = SelectedFloorNumberList();
                    // Теперь надо составить словарь совпадающих значений. Ключем словаря будет номер квартиры
                    // А значением, будет ID записи в БД... ну и еще кой-чего;)
                    Dictionary<int,ConCurData> concurDic = CheckConcurrenceEntries(selectedFloorNumbers);
                }
            }
            else
            {
                MessageBox.Show("Нет данных. Невозможно сделать проверку. DBImporter.CheckExistedKeyRecords()");
                return;
            }
        }

        private Dictionary<int, ConCurData> CheckConcurrenceEntries(List<int> selectedFloorNumbers)
        {
            Dictionary<int, ConCurData> result = new Dictionary<int, ConCurData>();

            var db = new IncomeDataContext(IncomeDataContext.DBSource);

            try
            {
                var Query = from KeysDataMapper data in db.KeysTable orderby data.FloorNo select data;
                int lastChecked = 0;
                foreach (var data in Query)
                {
                    for (int i = lastChecked; i < selectedFloorNumbers.Count; i++)
                    {
                        if (data.FloorNo >= selectedFloorNumbers[i])
                        {
                            if (data.FloorNo == selectedFloorNumbers[i])
                            {
                                ConcurrenceType conType = CheckConType(data);
                                result.Add(data.FloorNo, new ConCurData() { ConType = conType, ID = data.Id });
                            }
                        }
                        else
                        {
                            lastChecked = i;
                            break;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }

            if (result.Count > 0)
            {
                status.ConcurrencedDataCount = result.Count;
                status.FullConcurrenced = 0;
                status.PartialConcurrenced = 0;
                foreach (var record in result)
                {
                    if (record.Value.ConType == ConcurrenceType.Full) status.FullConcurrenced++;
                    if (record.Value.ConType == ConcurrenceType.Partially) status.PartialConcurrenced++;
                }
            }
            return result;
        }

        private ConcurrenceType CheckConType(KeysDataMapper data)
        {
            // data - это запись из БД, которую нужно сверить с выбранными пользователем данными.
            // эти данные содержатся в списке secondaryKeyDataList с типами SecondaryKeyDataSet...
            // странно, тут всего одно поле... ну понятно. поле одно значений столько же сколько и в 
            // primaryKeyData. Ошибка во входящих данных. Посылается не полный список secondaryKeyDataList
            // !!!!!!!!!!!!!1
            for (int i = 0; i < primaryKeyData.KeyMaskDic.Count; i++) //foreach (var samePair in primaryKeyData.KeyMaskDic)
            {
                var samePair = primaryKeyData.KeyMaskDic.ElementAt(i);
                if (samePair.Key.GetValueByMask(samePair.Value.MaskSyntax) == data.FloorNo.ToString())
                {
                    ConcurrenceType result = ConcurrenceType.Full;
                    Queue<string> fieldsQueue = new Queue<string>();// { "BuildAdress", "Name", "WaterCounterIsDivide" };
                    fieldsQueue.Enqueue("WaterCounterIsDivide");
                    fieldsQueue.Enqueue("Name");
                    fieldsQueue.Enqueue("BuildAdress");

                    // Прошерстим выбранные пользователем вторичные ключевые данные
                    foreach (var secondData in secondaryKeyDataList)
                     {
                        string fieldName;
                        int queueCount = fieldsQueue.Count;
                        for (int j = 0; j < queueCount; j++)
                        {
                            fieldName = fieldsQueue.Dequeue();
                            if (secondData.FieldName == fieldName)
                            {
                                switch (fieldName)
                                {
                                    case "BuildAdress":
                                        if (data.BuildAdress != (string)secondData.DataSet[i])
                                            result = ConcurrenceType.Partially;
                                        break;
                                    case "Name":
                                        if (data.Name != (string)secondData.DataSet[i])
                                            result = ConcurrenceType.Partially;
                                        break;
                                    case "WaterCounterIsDivide":
                                        if (data.WaterCounterIsDivide != (bool)secondData.DataSet[i])
                                            result = ConcurrenceType.Partially;
                                        break;
                                }
                                break;
                            }
                            else
                                fieldsQueue.Enqueue(fieldName); // Возврат в очередь, для следующей проверки.
                        }                    
                    }

                /*    // Если очередь с именами полей все еще содержит значения, то делаем доп. проверку:
                    while (fieldsQueue.Count > 0)
                    {
                        var fieldName = fieldsQueue.Dequeue();
                        switch (fieldName)
                        {
                            case "BuildAdress":
                                if (data.BuildAdress != "")
                                    result = ConcurrenceType.Partially;
                                break;
                // короче... бред какойто.. надо ли делать эту проверку?
                            case "Name":
                                if (data.Name != (string)secondData.DataSet[i])
                                    result = ConcurrenceType.Partially;
                                break;
                            case "WaterCounterIsDivide":
                                if (data.WaterCounterIsDivide != (bool)secondData.DataSet[i])
                                    result = ConcurrenceType.Partially;
                                break;
                        }
                    }  */

                    return result;
                }
            }

            return ConcurrenceType.Error; // Ну это в крайнем случае.
        }

        private List<int> SelectedFloorNumberList()
        {
            List<int> result = new List<int>();
            result.Clear();

            status.SelectedKeyDataCount = primaryKeyData.UniqValueCount;

            foreach (var floorNoStr in primaryKeyData.UniqValList)
            {
                try
                {
                    result.Add(int.Parse(floorNoStr));
                }
                catch
                {
                    MessageBox.Show("Какой-то номер квартиры (" + floorNoStr +
                        "), совсем не номер. Однако... DBImporter.CheckExistedKeyRecords()");
                }
            }
            if (result.Count > 0) result.Sort(); // Отсортируем список.

            return result;
        }
    }

    public class ImporterStatus
    {
        /// <summary>
        /// Количество выбранных ключевых значений, предназначеных для занесения в БД.
        /// </summary>
        public int SelectedKeyDataCount { get; set; }
        /// <summary>
        /// Количество ключевых записей, совпавших с существующими в БД.
        /// </summary>
        public int ConcurrencedDataCount { get; set; }
        /// <summary>
        /// Количество полностью совпавших записей.
        /// </summary>
        public int FullConcurrenced { get; set; }
        /// <summary>
        /// Количество частично совпавших записей.
        /// </summary>
        public int PartialConcurrenced { get; set; }
        // ???
    }

    public class ConCurData
    {
        /// <summary>
        /// ID записи в БД.
        /// </summary>
        public int ID { get; set; }

        public ConcurrenceType ConType { get; set; } 
    }

    public enum ConcurrenceType
    {
        Full, Partially, Error
    }
}
