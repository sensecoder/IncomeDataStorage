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

namespace Useful
{
    /// <summary>
    /// "Расшифровщик" строки, которая приходит в качестве результата запроса
    /// к сервису Live.
    /// </summary>
    public class LiveOperationRawResultParser
    {
        private string _parsedstr;


        /// <summary>
        /// Конструктор принимающий строку для "расшифровки"
        /// </summary>
        /// <param name="parsedstr"></param>
        public LiveOperationRawResultParser(string parsedstr)
        {
            _parsedstr = parsedstr;
        }
        
        /// <summary>
        /// Возвращает список объектов с данными о файлах, которые были описаны в 
        /// строке при создании парсера.
        /// </summary>
        /// <returns></returns>
        public List<LiveFileData> GetFilesData()
        {
            var res = new List<LiveFileData>();
            res.Clear();
            // Буду выцарапывать из строки объекты как строки окруженные фигурными скобками,
            // причем вложенные объекты будут исключены из строки. Вот такое ограничение...
            // Т.е. буду получать коллекцию из строк, которые содержат поля объектов
            // одного уровня вложенности. Причем вложенные объекты буду обозначать... както|)
            List<string> _objects = new List<string>();
            _objects.Clear();
            _objects.Add(_parsedstr);
            //var objStr = _parsedstr;
            while (_objects != null)
            {
                _objects = GetObjectsString(_objects);
                if (_objects != null)
                {
                    foreach (var objstr in _objects)
                    {
                        res.Add(SerializeLiveFileData(objstr));
                        /*if (trimInesreted(objstr).Contains("\"type\": \"file\""))
                        {
                            // ху ху йеее!
                            res.Add(SerializeLiveFileData(objstr));
                        } */
                    }
                }
            }
            if (res.Count > 0) return res;
            else return null;
        }

        /// <summary>
        /// Создает объект с данными о файле (или папке, да еще хрен знает чего что хранится на SkyDrive)
        /// сериализуя его из строки, которая содержит значения полей этого объекта.
        /// </summary>
        /// <param name="objstr">Строка для сериализации</param>
        /// <returns>Объект данных хранящихся на SkyDrive</returns>
        private LiveFileData SerializeLiveFileData(string objstr)
        {
            LiveFileData res = new LiveFileData();
            object objRes = (object)res;
            double percent = SerializeObjFromJSON(trimInesreted(objstr), ref objRes);
            res = (LiveFileData)objRes;
            /*
            System.Reflection.FieldInfo[] FieldInfo = res.GetType().GetFields();

            FieldInfo[0].FieldType;

            res.id = FindAttrValByName(objstr, "id");
            res.name = FindAttrValByName(objstr, "name");
            res.description = FindAttrValByName(objstr, "description");
            res.parent_id = FindAttrValByName(objstr, "parent_id");

            //парсить нада с умом!

            
            res.size = int.Parse(FindAttrValByName(objstr, "size"));
            res.upload_location = FindAttrValByName(objstr, "upload_location");
            res.comments_count = int.Parse(FindAttrValByName(objstr, "comments_count"));
            res.comments_enabled = bool.Parse(FindAttrValByName(objstr, "comments_enabled"));
            res.is_embeddable = bool.Parse(FindAttrValByName(objstr, "is_embeddable"));
            res.count = int.Parse(FindAttrValByName(objstr, "count"));
            res.link = FindAttrValByName(objstr, "link");
            res.type = FindAttrValByName(objstr, "type");
            res.created_time = DateTime.Parse(FindAttrValByName(objstr, "created_time"));
            res.updated_time = DateTime.Parse(FindAttrValByName(objstr, "updated_time"));
            */
            return res; 
        }

        /// <summary>
        /// Предполагается, что во входящей строке имеется описание объекта Container
        /// в формате JSON. Метод будет пытаться заполнить поля контейнера тем что 
        /// содержит эта строка. В качестве результата будет возвращен процент совпавших
        /// (сериализованных) полей, ну и объект клиента, соответсвенно изменится.
        /// </summary>
        /// <param name="ObjStr">Строка с описанием полей объекта в формате JSON</param>
        /// <param name="Container">Объект-контейнер, описание которого, предположительно находится в строке ObjStr</param>
        /// <returns>Процент совпавших полей </returns>
        private double SerializeObjFromJSON(string ObjStr, ref object Container)
        {
            // сначала сформирую словарь полей, а потом их буду перебирать и сравнивать с полями контейнера
            Dictionary<string, string> ObjDic = new Dictionary<string, string>();
            ObjDic.Clear();
            // int valStartInd = -1; // стартовый индекс, с которого начинается значение поля.
            // найдем его:

            string name, value;

            // а может ну их нафиг, эти словари...
            // метод возвращает значение, а это значение означает процент совпавших полей.
            // так, что предварительно нужно посчитать сколько полей у Container
            var containerFields = Container.GetType().GetFields();
            var containerFieldsCount = containerFields.GetLength(0);
            var consilenseFieldCount = 0;   // количество совпавших полей.
            // прошерстим строку.
            var ind = ObjStr.IndexOf('"') + 1;
            while (ind > 0)
            {
                try
                {
                    // есть ind, который типа показывает на начало предположительного имя поля,
                    // ну чтож... примем эти предположения.
                    var fieldNameStartInd = ind;
                    ind = ObjStr.IndexOf('"', ind);
                    var fieldNameStopInd = ind;
                    // имя обозначено индексами, хотя...
                    if (ind < 0) // то это ошибка...
                        { break; }
                    name = ObjStr.Substring(fieldNameStartInd, fieldNameStopInd - fieldNameStartInd); // вытаскиваю имя объекта.
                    
                    // теперь нужно определить каково значение этого поля, и поле ли это вообще?
                    ind++;
                    // Между именем поля и его значением должен быть разделитель, найду его:
                    // так... разделитель должен идти первым, после имени поля, иначе это херня какая-то...
                    var tempChar = ObjStr[ind];
                    while (tempChar != ':')
                    {
                        // просто допускаю что могут быть пробелы, но если это не так, то все... пиздец...
                        if (tempChar == ' ')
                        {
                            ind++;
                            tempChar = ObjStr[ind];
                        }
                        else
                        {
                            name = ""; // имеется ввиду, что это не было именем поля и поиск продолжится.
                            tempChar = ':';
                        }
                    }
                    // если разделитель найден, то дале может быть три варианта:
                    // либо поле имеет строковое значение и оно "обрамлено" в кавычки,
                    // либо это число или булева величина.
                    // хм... за значением поля может стоять запятая, а может и пробел, а может и вообще строка закончится.
                    // ну для начала, необходимо найти индекс с которого значение поля начинается..
                    // предположительно, после разделителя следует пробел (или, возможно, пробелЫ)
                    // короче, нужно чекать.
                    value = "";
                    if (name != "") // проверка на то, а не пустое ли это дело вообще? )
                    {
                        ind++;
                        tempChar = ObjStr[ind];
                        while (tempChar == ' ')
                        {
                            ind++;
                            tempChar = ObjStr[ind];
                        }
                        int valStartInd, valStopInd;
                        // если пришли сюда, то это значит, что был обнаружен символ отличный от пробела
                        // после разделителя. Нужно выяснить, что это за символ?
                        if (tempChar == '"')
                        { // коли так, то это значит что имеется строковое значение либо дата.
                            valStartInd = ind + 1;
                            valStopInd = ObjStr.IndexOf('"', valStartInd);
                            if (valStopInd < 0)
                            { ind = -1; break; }
                            value = ObjStr.Substring(valStartInd, valStopInd - valStartInd);
                            ind = valStopInd + 1;
                        }
                        else
                        { // тобишь имеем число или булево значение
                            // нужно найти первую запятую или первый пробел, после этого значения, ну или это будет конец строки
                            valStartInd = ind;
                            var tempStopInd = ObjStr.IndexOf(',', valStartInd);
                            valStopInd = ObjStr.IndexOf(' ', valStartInd);
                            if (tempStopInd > 0 && valStopInd > 0)
                            {
                                if (tempStopInd < valStopInd)
                                    valStopInd = tempStopInd;
                            }
                            if (valStopInd < 0)
                            { valStopInd = ObjStr.Length - 1; }
                            value = ObjStr.Substring(valStartInd, valStopInd - valStartInd);
                            ind = valStopInd + 1;
                        }
                    }
                    // после всех этих махинаций имеем две строки: name и value
                    
                    // теперь необходимо полю контейнера присвоить найденное значение
                    // нужно посмотреть, содержит ли Контейнер поле с таким именем.
                    var fieldInfo = Container.GetType().GetField(name);
                    if (fieldInfo != null && value != "")      // это значит что контейнер содержит поле с таким именем.                    
                    {
                        consilenseFieldCount++; // совпало
                        // нужно привести value к соответствующему типу и установить значение
                        var fieldTypeName = fieldInfo.FieldType.Name;                        
                        switch (fieldTypeName)
                        {                             
                            case "Int32"    : fieldInfo.SetValue(Container, int.Parse(value));
                                              break;
                            case "Double"   : fieldInfo.SetValue(Container, double.Parse(value));
                                              break;
                            case "Boolean"  : if (value == "true" || value == "True") fieldInfo.SetValue(Container, true);
                                              if (value == "false" || value == "False") fieldInfo.SetValue(Container, false);
                                              break;
                            case "String"   : fieldInfo.SetValue(Container, value);
                                              break;
                            case "DateTime" : fieldInfo.SetValue(Container, DateTime.Parse(value));
                                              break;       
                        }                    
                    }
                    ind = ObjStr.IndexOf('"', ind) + 1;
                    name = "";
                    value = "";
                    // ежели контейнер не содержит поле с таким именем, то просто переходим к другому полю в строке.
                }
                catch { return -1; }
            }
            return consilenseFieldCount/containerFieldsCount;
        }

        /// <summary>
        /// убирает из строки, описывающей объект, все описания вложенных объетов
        /// </summary>
        /// <param name="objstr">строка с описанием объекта</param>
        /// <returns>"очищенная" строка</returns>
        private string trimInesreted(string objstr)
        {
            string res = "";
            // все что нужно - это найти границы вложенного объекта.
            int begInd = 0;
            int endInd = 0;
            int index = 0;
            while (begInd >= 0 && endInd >= 0)
            {
                FindFirstObjectBorder(objstr, index, ref begInd, ref endInd);
                if (begInd > 0 && endInd > 0) 
                {
                    res = res + objstr.Substring(index, begInd - index);
                    index = endInd + 1;
                }
            }
            res = res + objstr.Substring(index);
            return res;
        }

        /// <summary>
        /// Возвращает список содержащий строки в которых описаны поля объектов верхнего уровня
        /// </summary>
        /// <param name="_objects"></param>
        /// <returns></returns>
        private List<string> GetObjectsString(List<string> _objects)
        {
            if (_objects == null) return null;
            List<string> res = new List<string>();
            res.Clear();
            // ну чо... необходимо найти объекты самого верхнего уровня...
            // для этого, для начала буду искать открывающую фигурную 
            // скобку в каждой строке списка.
            foreach ( var str in _objects )
            {
                if (str.Length == 0) break;
                int index = 0;
                index = str.IndexOf('{', index);
                if (index < 0) return null; 
                // если index будет больше нуля, значит была найдена открывающая скобка
                // и можно продолжать дальнейший парсинг.
                int begInd = index;
                int endInd = -1;
                /* var open = 1; // счетчик, который говорит о том, что найдена открывающая скобка
                // и это значит, что по крайней мере один объект в рассматриваемой строке описан. */
                var gotObj = true;
                while (gotObj) // пока есть объекты одного уровня в строке
                {
                    // теперь нужно найти закрывающую скобку объекта, исключив при этом вложенные объекты
                    /* while (open > 0)
                    {
                        var indexClose = str.IndexOf('}', index); // нахожу индекс следующей закрывающей скобки
                        var indexOpen = str.IndexOf('{', index);  // нахожу индекс следующей открывающей скобки

                        if (indexOpen > 0 && indexOpen < indexClose)
                        {
                            index = indexClose + 1;
                            open++;
                        }
                        if (indexOpen > indexClose)
                        {
                            index = indexOpen + 1;
                            open--;
                        }

                        // вложенный объект должен закрываться иначе... ошибка какая-то...
                        if (indexClose < 0) return null;
                        else
                            index = indexClose; //
                    } */
                    FindFirstObjectBorder(str, index, ref begInd, ref endInd);
                    if (endInd < 0) return null; // проверка на ошибку. 
                    // сюда приходим, если был полностью найден один объект верхнего уровня.
                    // его границы определяются начальным индексом строки begInd и заканчивается endInd
                    // теперь этот объект можно "засунуть" в список результатов метода.
                    // begInd++;
                    res.Add(str.Substring(begInd, (endInd - begInd)));
                    index = endInd;
                    // ОК. Теперь нужно еще понять, а что далее? есть ли еще объект на этом уровне?
                    try
                    {
                        index = str.IndexOf(',', index); // по правилам полагается что объекты перечисляются через запятую (если вдруг имеем дело с массивом объектов)
                        if (index > 0)
                            index = str.IndexOf('{', index);                       
                        if (index < 0) gotObj = false;
                    }
                    catch
                    {
                        gotObj = false; // так или иначе сюда выходим если не лады...
                    }        
                }
                /*  var gotObj = true; // это означает, что был получен новый объект.
                if (index > 0)
                {
                    var begInd = index; // это, типа, начальные и конечные индексы
                    var endInd = -1;    // определяющие "границы" объекта
                    while (gotObj)
                    {
                        while (endInd < 0) // изначально -1, означает что окончание объекта не найдено.
                        {
                            // необходимо проигнорить вложенные объекты.
                            var insertedInd = str.IndexOf('{', index);
                            // стэк меня спасет!!! эврика!
                            // ...пиздец... какой нахуй стек? просто счетчик.
                            while (insertedInd > 0) // это означает что внутри объекта был найден вложенный объект
                            {
                                var open = 1;
                                while (open > 0)
                                {
                                    var indexClose = str.IndexOf('}', insertedInd); // нахожу индекс следующей закрывающей скобки
                                    var indexOpen = str.IndexOf('{', insertedInd);  // нахожу индекс следующей открывающей скобки

                                    if (indexOpen > 0 && indexOpen < indexClose)
                                    {
                                        insertedInd = indexClose + 1;
                                        open++;
                                    }
                                    if (indexOpen > indexClose)
                                    {
                                        insertedInd = indexOpen + 1;
                                        open--;
                                    }
                                    
                                    // вложенный объект должен закрываться иначе... ошибка какая-то...
                                    if (indexClose < 0) return null;
                                    else 
                                        index = indexClose+1; //
                                }
                                insertedInd = str.IndexOf('{', index);
                            }

                            // все вложенные объекты найдены, теперь нужно найти фигурную скобочку, 
                            // закрывающую искомый наш объектик верхнего уровня.
                            {
                                index = str.IndexOf('}', index);
                                if (index < 0) // т.е. опять, бля, какая-то ошибка...
                                    return null;
                                endInd = index - 1; // вот где заканчивается объект.
                                // в целом, у мну уже есть объект. значит его можно уже добавить в списка.
                                res.Add(str.Substring(begInd, (endInd - begInd)));
                                // ОК. Теперь нужно еще понять, а что далее? есть ли еще объект на этом уровне?
                                index++;
                                try
                                {
                                    index = str.IndexOf(',', index);
                                    if (index < 0) gotObj = false;
                                    index = str.IndexOf('{', index) + 1;
                                    begInd = index;
                                    endInd = -1;
                                }
                                catch 
                                {
                                    gotObj = false; // так или иначе сюда выходим если не лады...
                                }                                
                            }
                        }
                    }
                } */
            }

            if (res.Count == 0)
                return null;
            else
                return res;
        }
        
        /// <summary>
        /// Находит границы первого, встречающегося в строке описания объекта.
        /// Причем, происходит исключение всех вложенных в этот объект описаний объектов.
        /// </summary>
        /// <param name="ExploredStr">Строка в которой предположительно есть описание объекта ограниченное фигурными скобками</param>
        /// <param name="StartInd">Стартовый индекс в исследуемой строке, с которого и необходимо начинать поиск границ</param>
        /// <param name="begInd">Возвращает индекс открывающей скобки, после которой идет описание объекта</param>
        /// <param name="endInd">Возвращает индекс закрывающей скобки</param>
        private void FindFirstObjectBorder(string ExploredStr, int StartInd, ref int begInd, ref int endInd)
        {
            if (ExploredStr.Length == 0)  // проверка на пустую строку, что однозначно дает ошибку
            {
                begInd = -1;
                endInd = -1;
                return;
            }
            int index = StartInd;
            index = ExploredStr.IndexOf('{', index) + 1;
            if (index < 0) // это значит что описания вложенных объектов в строке нет вообще, раз не было найдено ни одной открывающей скобки.
            {
                begInd = -1;
                endInd = -1;
                return;
            }
            // если index будет больше нуля, значит была найдена открывающая скобка
            // и можно продолжать дальнейший парсинг.
            begInd = index;
            var open = 1; // счетчик, который говорит о том, что найдена открывающая скобка
            // и это значит, что по крайней мере один объект в рассматриваемой строке описан.
            // теперь нужно найти закрывающую скобку объекта, исключив при этом вложенные объекты
            while (open > 0)
            {
                try
                {
                    var indexClose = ExploredStr.IndexOf('}', index); // нахожу индекс следующей закрывающей скобки
                    var indexOpen = ExploredStr.IndexOf('{', index);  // нахожу индекс следующей открывающей скобки

                    if (indexOpen > 0 && indexOpen < indexClose) // т.е. найдена еще одна открывающая скобка
                    {
                        index = indexOpen + 1;
                        open++;
                    }
                    if (indexOpen > indexClose)
                    {
                        index = indexClose + 1;
                        open--;
                    }
                    if (indexOpen < 0 && indexClose > 0)
                    {
                        index = indexClose + 1;
                        open--;
                    }

                    // вложенный объект должен закрываться иначе... ошибка какая-то...
                    if (indexClose < 0)
                    {
                        begInd = -1;
                        endInd = -1;
                        return;
                    }
                    else
                        if (open == 0) index = indexClose - 1;
                }
                catch
                {
                    MessageBox.Show("Ошибка парсера! Не могу найти границы объекта.");
                }
            }
            // ну вот, типа все.
            endInd = index;
        }
    }

    

    public class LiveFileData
    {
        public string id;
        public string name;
        public string description;
        public string parent_id;
        public int size;
        public string upload_location;
        public int comments_count;
        public bool comments_enabled;
        public bool is_embeddable;
        public int count;
        public string source;
        public string link;
        public string type;
        public DateTime created_time;
        public DateTime updated_time;
    }
}
