using System;
using IncomeDataStorage.Domain;
using System.Linq;
using System.Collections.Generic;
using Useful;
using System.Windows;

namespace IncomeDataStorage.Data
{
    /// <summary>
    /// Класс работает с таблицей присоединенных данных
    /// </summary>
    public class AttachedDataWorker
    {
        IncomeDataContext db;
        private int keyID = -1;     // ID записи в таблице ключевых данных, которой 
                                    // принадлежать обрабатываемые присоединенные данные

        // пустой конструктор
        public AttachedDataWorker()
        { }
        /// <summary>
        /// Создает экземпляр, которому известен ID записи в таблице ключевых данных.
        /// </summary>
        /// <param name="id">Идентификатор (ID)</param>
        public AttachedDataWorker(int id)
        {
            keyID = id;
        }

        /// <summary>
        /// Добавляет в БД новую запись со входящими параметрами
        /// </summary>
        /// <returns>ID записи</returns>
        internal int AddNew(int hotWaterMain, int hotWaterSecondary, int coldWaterMain, int coldWaterSecondary, int electricity)
        {
            if (keyID < 0) return -1;

            int id;

            db = new IncomeDataContext(IncomeDataContext.DBSource);
            AttachedDataMapper ad = new AttachedDataMapper();

            // присваиваю входящие значения:
            if (hotWaterMain >= 0) ad.HotWaterMain = hotWaterMain;
            if (hotWaterSecondary >= 0) ad.HotWaterSecondary = hotWaterSecondary;
            if (coldWaterMain >= 0) ad.ColdWaterMain = coldWaterMain;
            if (coldWaterSecondary >= 0) ad.ColdWaterSecondary = coldWaterSecondary;
            if (electricity >= 0) ad.Electricity = electricity;
            ad.DateOfIncome = DateTime.Now;
            ad.DateOfRedaction = ad.DateOfIncome;
            ad.NoOfRedaction = 0;
            ad.Validate = true;
            ad.KeyId = keyID;

            db.AttachedTable.InsertOnSubmit(ad);
            
            db.SubmitChanges();

            id = ad.Id;
            return id;
        }

        /// <summary>
        /// Добавляет в БД отредактированную запись
        /// </summary>
        /// <returns>ID записи, либо -1 в случае ошибки</returns>
        internal int AddRedacted(AttachedDataMapper attdata)
        {          
            int id;
            
            db = new IncomeDataContext(IncomeDataContext.DBSource);
            AttachedDataMapper ad = new AttachedDataMapper();

            if (attdata != null)
            {
                ad.ColdWaterMain = attdata.ColdWaterMain;
                ad.ColdWaterSecondary = attdata.ColdWaterSecondary;
                ad.DateOfIncome = attdata.DateOfIncome;
                ad.Electricity = attdata.Electricity;
                ad.HotWaterMain = attdata.HotWaterMain;
                ad.HotWaterSecondary = attdata.HotWaterSecondary;
                ad.KeyId = attdata.KeyId;
                ad.DateOfRedaction = DateTime.Now;
                ad.NoOfRedaction = attdata.NoOfRedaction + 1;
                ad.Validate = true;
                DropValid(attdata.Id); // делаю входящую запись невалидной.
                db.AttachedTable.InsertOnSubmit(ad);

                db.SubmitChanges();

                id = ad.Id;
                return id;
            }
            else
                return -1;
            
        }

        /// <summary>
        /// необходим для сбоса валидности записи
        /// </summary>
        /// <param name="id"></param>
        private void DropValid(int id)
        {
            db = new IncomeDataContext(IncomeDataContext.DBSource);

            var query = from AttachedDataMapper data in db.AttachedTable where data.Id == id select data;

            foreach (var data in query)
            {
                data.Validate = false;
                db.SubmitChanges();
                return;
            }
        }

        /// <summary>
        /// Удаление записи... ох уж этот Руслан...
        /// </summary>
        /// <param name="id"></param>
        internal void Delete(int id)
        {
            db = new IncomeDataContext(IncomeDataContext.DBSource);

            var query = from AttachedDataMapper data in db.AttachedTable where data.Id == id select data;

            foreach (var data in query)
            {
                data.Validate = false;
                data.Deleted = true;
                db.SubmitChanges();
                return;
            }
        }

        /// <summary>
        /// Заполняет список значениями, которые хранятся в БД (дублирует таблицу в список объектов)
        /// </summary>
        /// <returns>Список</returns>
        internal List<AttachedDataMapper> FillList()
        {
            List<AttachedDataMapper> result = new List<AttachedDataMapper>();

            db = new IncomeDataContext(IncomeDataContext.DBSource);

            try
            {
                var Query = from AttachedDataMapper data in db.AttachedTable select data;
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
        /// Возвращает маппер записи в таблице присоединенных данных
        /// </summary>
        /// <param name="ID">Идентификатор записи</param>
        /// <returns></returns>
        internal AttachedDataMapper ReturnMapper(int ID)
        {
            if (ID < 0) return null;

            db = new IncomeDataContext(IncomeDataContext.DBSource);

            try
            {
                var Query = from AttachedDataMapper data in db.AttachedTable where data.Id == ID select data;
                foreach (var data in Query)
                {
                    return data;
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        /// <summary>
        /// Возвращает маппер записи в таблице ключевых данных
        /// </summary>
        /// <param name="keyID">Идентификатор записи</param>
        /// <returns></returns>
        internal KeysDataMapper ReturnKeysMapper(int keyID)
        {
            if (keyID < 0) return null;

            db = new IncomeDataContext(IncomeDataContext.DBSource);

            try
            {
                var Query = from KeysDataMapper data in db.KeysTable where data.Id == keyID select data;
                foreach (var data in Query)
                {
                    return data;
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
    }
}
