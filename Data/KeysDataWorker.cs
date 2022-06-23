using IncomeDataStorage.Domain;
using System.Linq;
using System.Collections.Generic;
using Useful;
using System.Windows;

namespace IncomeDataStorage.Data
{
    /// <summary>
    /// Класс работает с таблицей ключевых значений.
    /// </summary>
    public class KeysDataWorker : IKeysColl
    {
        IncomeDataContext db;
        private string text = "";           // отрывочный текст, с которым могут работать методы класса
        private int keyID = -1;

       /* public string FloorNo
        {
            get { return floorNo; }
            private set
            {
                // тут можно еще проверочку сделать, а точно ли это "номер", в смысле, число.
                floorNo = value;
            }
        } */

        // конструкторы:
        public KeysDataWorker()
        {
           // IncomeDataContext db = new IncomeDataContext(IncomeDataContext.DBSource);
        }
        /// <summary>
        /// Создает экземпляр которому известен отрывок текста, с которым можно работать.
        /// </summary>
        /// <param name="text">отрывок текста</param>
        public KeysDataWorker(string text)
        {
            this.text = text;
        }

        /// <summary>
        /// Создает экземпляр, которому известен ID записи в таблице ключевых данных.
        /// </summary>
        /// <param name="id">Идентификатор (ID)</param>
        public KeysDataWorker(int id)
        {
            keyID = id;
        }

        /// <summary>
        /// Узнает, есть ли разделение учетов по горячей воде для уже известного ID записи
        /// </summary>
        /// <returns></returns>
        public bool IsHotWaterDivided()
        {
            if (keyID < 0) return false;
            db = new IncomeDataContext(IncomeDataContext.DBSource);
            try
            {
                var devidedQuery = from KeysDataMapper data in db.KeysTable where data.Id == keyID select data;
                foreach (var data in devidedQuery)
                {
                    return data.WaterCounterIsDivide;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// !!! НЕНУЖНО ???
        /// Формирует коллекцию ключевых данных.
        /// </summary>
        /// <returns>CollectionOfKeys</returns>
        public CollectionOfKeys GetKeysColl()
        {
            CollectionOfKeys keyscoll = new CollectionOfKeys(); // это будет коллекция ключевых значений.
            keyscoll.Values.Clear();                            // почистим элементы коллекции.
            
            var UnicFloorNoQuery = (from KeysDataMapper data in db.KeysTable select data.FloorNo).Distinct<int>();

            foreach (var floorNo in UnicFloorNoQuery)
            {
                KeyData keydata = new KeyData();
                keydata.FloorNo = floorNo;
                var UnicNameQuery = (from KeysDataMapper data in db.KeysTable where data.FloorNo == floorNo select data.Name).Distinct<string>();
                foreach (var name in UnicNameQuery)
                {
                    keydata.Name = name;
                }
                keyscoll.Values.Add(keydata);
            }

            return keyscoll;
        }

        /// <summary>
        /// Осуществляет поиск ключевых полей в которых начало совпадает с тем, что находится в известном экземпляру (я надеюсь)
        /// куском отрывочного текста (поле text)
        /// </summary>
        /// <returns>Возвращает список записей в таблице которые подходят под выборку</returns>
        internal List<KeysDataMapper> SearchFoloorsNoStartWith()
        {
            //int num;
            List<KeysDataMapper> result = new List<KeysDataMapper>();
            result.Clear();

            if (text == "") return null;

            db = new IncomeDataContext(IncomeDataContext.DBSource);
            try
            {
                // это не надо?: num = int.Parse(text);
                // ну вот, номер должнобыть известен. теперь нужно сделать выборку в БД и найти совпадения.
                // что-то мне подсказывает, что я не буду использовать доменные классы... опять ломка восприятия...
                // ну а как работать? это превый опыт) целка рвется)
                // ОК. Сначала создам дата контекст, если его нет:
                // перенесено в конструктор: 
                // это типа и есть БД, со всеми (двумя) таблицами.
                // сейчас мне нужно создать запрос, который делает выборку. стоп... я уже чета делал такое вчера...
                // ну да ладно, создаю запрос:
                var FloorNoQuery = from KeysDataMapper data in db.KeysTable select data;
                // ОК. Сейчас в запросе "как бы" существует вся БД.
                // Теперь нужно проверить есть ли совпадения номеров квартир по начальным цифрам в этом списке с num
                foreach (var data in FloorNoQuery)
                {
                    // ха! а тут нужно делать текстовое сравнение) вот бля я лошара) туду-суду парсю...
                    // ну да ладно, есть ведь номер квартиры, это экземпляр знает, я надеюсь
                    if (data.FloorNo.ToString().StartsWith(text))
                        result.Add(data);
                    // ну и се..
                }
            }
            catch
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Добавляет в БД новую запись. Нужно, чтобы экземпляр знал кусок текста (поле text).
        /// Если в тексте число, то добавляет новую запись с номером квартиры.
        /// Если там строка, то с именем собственника.
        /// </summary>
        /// <returns>ID записи</returns>
        internal int AddSomeNew()
        {
            if (text == "") return -1;

            int id;

            db = new IncomeDataContext(IncomeDataContext.DBSource);
            KeysDataMapper kd;

            if (StringOperation.IsIntNumber(text))
            { // это случай, когда определяется номер квартиры.
                var floorNo = int.Parse(text);
                kd = new KeysDataMapper() { FloorNo = floorNo };                
            }
            else
            { // ну либо иначе выбирается путь занесения с именем.
                kd = new KeysDataMapper() { Name = text };
            }

            kd.WaterCounterIsDivide = false;
            db.KeysTable.InsertOnSubmit(kd);
            db.SubmitChanges();

            id = kd.Id;
            return id;
        }

        /// <summary>
        /// Заполняет список значениями, которые хранятся в БД (дублирует таблицу в список объектов)
        /// </summary>
        /// <returns>Список</returns>
        internal List<KeysDataMapper> FillList()
        {
            List<KeysDataMapper> result = new List<KeysDataMapper>();

            db = new IncomeDataContext(IncomeDataContext.DBSource);
           
            try
            {
                var Query = from KeysDataMapper data in db.KeysTable select data;
                foreach (var data in Query)
                {
                    result.Add(data);
                }
            }
            catch
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Корректировка номера квартиры в записи с ID указанным в App.CorrectionID;
        /// </summary>
        internal void FloorNoCorrection(int floorNo)
        {
            if (App.CorrectionID <= 0) return;
            db = new IncomeDataContext(IncomeDataContext.DBSource);

            var query = from KeysDataMapper data in db.KeysTable where data.Id == App.CorrectionID select data;

            foreach (var data in query)
            {
                data.FloorNo = floorNo;
                db.SubmitChanges();
                return;
            }
        }

        /// <summary>
        /// Корректировка имени собственника в записи с ID указанным в App.CorrectionID;
        /// </summary>
        internal void NameCorrection(string name)
        {
            if (App.CorrectionID <= 0) return;
            db = new IncomeDataContext(IncomeDataContext.DBSource);

            var query = from KeysDataMapper data in db.KeysTable where data.Id == App.CorrectionID select data;

            foreach (var data in query)
            {
                data.Name = name;
                db.SubmitChanges();
                return;
            }
        }

        internal void IsHotWaterDividedSet(bool div)
        {
            if (keyID <= 0)
            {
                MessageBox.Show("Err: IsHotWaterDividedSet. keyID <= 0");
                return;
            }
            db = new IncomeDataContext(IncomeDataContext.DBSource);

            var query = from KeysDataMapper data in db.KeysTable where data.Id == keyID select data;

            foreach (var data in query)
            {
                data.WaterCounterIsDivide = div;
                db.SubmitChanges();
                return;
            }
        }

        /// <summary>
        /// Заполняет словарь значениями, которые хранятся в БД (дублирует таблицу в словарь объектов)
        /// В качестве ключа идет ID записи
        /// </summary>
        /// <returns>Словарь</returns>
        internal Dictionary<int, KeysDataMapper> FillDic()
        {
            var result = new Dictionary<int, KeysDataMapper>();

            db = new IncomeDataContext(IncomeDataContext.DBSource);

            try
            {
                var Query = from KeysDataMapper data in db.KeysTable select data;
                foreach (var data in Query)
                {
                    result.Add(data.Id, data);
                }
            }
            catch
            {
                return null;
            }

            return result;
        }
    }
}
