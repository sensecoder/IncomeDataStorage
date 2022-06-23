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
using System.Collections.Generic;
using System.Linq;
using IncomeDataStorage.Data;

namespace IncomeDataStorage.Presentation
{
    /// <summary>
    /// Служит для реализации работы с таблицей журнала.
    /// </summary>
    public class JournalTableData
    {
        private AttachedDataMapper attData;
        private KeysDataMapper keyData;

        public AttachedDataMapper AttData
        {
            get
            {
                return attData;
            }
        }
        public KeysDataMapper KeyData
        {
            get
            {
                return keyData;
            }
        }

        // конструктор, пустой...
        public  JournalTableData()
        {

        }
        public JournalTableData(AttachedDataMapper att, KeysDataMapper key)
        {
            this.attData = att;
            this.keyData = key;
        }

        /// <summary>
        /// возвращает список всех присоединенных данных и связанных с ним
        /// ключевых данных
        /// </summary>
        /// <returns></returns>
        public List<JournalTableData> AllData(JournalViewParam param)
        {
            List<JournalTableData> preResult = new List<JournalTableData>();
            preResult.Clear();
            List<JournalTableData> result = new List<JournalTableData>();
            result.Clear();

            // для начала нужно вытащить из БД список всех присоединных данных
            AttachedDataWorker attFiller = new AttachedDataWorker();
            List<AttachedDataMapper> attDataList = attFiller.FillList(); // - вот он.

            // также пригодится список... а точнее словарь ключевых данных
            KeysDataWorker keyFiller = new KeysDataWorker();
            Dictionary<int, KeysDataMapper> keyDataDic = keyFiller.FillDic(); // - и это, типа, тоже он

            try
            {
                var Query = from AttachedDataMapper data in attDataList
                            where data.Validate         // здесь стоит условие, по которому в запрос включаются только те записи, которые имеют валидную редакцию. 
                            orderby data.DateOfIncome   // сортировка по дате изначального занесения записи.
                            select data; 
                foreach (var data in Query)
                {
                    try
                    {
                        attData = data;
                        keyData = keyDataDic[attData.KeyId];
                        preResult.Add(new JournalTableData(attData, keyData));
                    }
                    catch
                    { }
                }
            }
            catch
            { }

            try
            {
                switch (param.SortedBy)
                {
                    case Sorting.ByFloorNo:
                        IEnumerable<JournalTableData> floorQuery = preResult.OrderBy(journal => journal.keyData.FloorNo);
                        foreach (JournalTableData journal in floorQuery)
                        {
                            result.Add(journal);
                        }
                        if (param.ReverseSort) result.Reverse();
                        break;
                    case Sorting.ByDateOfIncome:
                        IEnumerable<JournalTableData> dateQuery = preResult.OrderBy(journal => journal.AttData.DateOfIncome);
                        foreach (JournalTableData journal in dateQuery)
                        {
                            result.Add(journal);
                        }
                        if (param.ReverseSort) result.Reverse();
                        break;
                    default:
                        result = preResult;
                        break;
                }                
               /* if (param.SortedBy == Sorting.ByFloorNo)
                {
                    IEnumerable<JournalTableData> query = preResult.OrderBy(journal => journal.keyData.FloorNo);
                    foreach (JournalTableData journal in query)
                    {
                        result.Add(journal);
                    }
                    if (param.ReverseSort) result.Reverse();
                }
                else
                    result = preResult; */
            }
            catch
            {
                result = preResult;
            }

            return result;
        }

        internal AttachedDataMapper PreData(AttachedDataMapper currData, List<JournalTableData> journalList)
        {
            AttachedDataMapper result = null;
            int keyID = currData.KeyId;
            DateTime currDate = currData.DateOfIncome;
            DateTime preDate = DateTime.MinValue;

            // найду запись с прeдидущей датой
            var query = from JournalTableData data in journalList where data.AttData.DateOfIncome < currDate && data.AttData.KeyId == keyID select data; //  
            foreach (var data in query)
            {
                if (data.AttData.DateOfIncome > preDate)
                {
                    preDate = data.AttData.DateOfIncome;
                    result = data.AttData;
                }
            }

            return result;
        }
    }
}
