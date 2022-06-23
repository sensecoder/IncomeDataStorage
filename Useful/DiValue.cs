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

namespace Useful
{
    /// <summary>
    /// Класс представляет собой некое значение состоящее из двух подзначений, 
    /// причем одно из них строковое, а второе - целое натуральньное число.
    /// </summary>
    public class DiValue
    {
        public string StringValue = "";
        public int IntValue = 0;

        // ? ну что за хрень? спотыкания на ровном месте... Конструктор приватный, экземпляр класса возвращается одним из статических методов.
        public DiValue() { }

        /// <summary>
        /// Метод разделяет строку в которой предположительно содержится двойное значение в виде строки
        /// </summary>
        /// <param name="str">Строка с двойным значением</param>
        /// <returns>Возвращает экземпляр класса либо null если строка не валидна</returns>
        public DiValue DiValStrDivider(string str)
        {
            StringValue = "";
            IntValue = 0;
            bool strValSealed = false, strValDetected = false;
            bool intValSealed = false, intValDetected = false;
            string intVal="";
            var strArr = str.ToCharArray();
            foreach (var ch in strArr)
            {
                if ((ch < '0') || (ch > '9'))
                {
                    if (!strValDetected)
                    {
                        strValDetected = true;
                        if (intValDetected) intValSealed = true;
                    }
                    else
                        if (strValSealed) return null;
                    StringValue = StringValue + ch;
                }
                else
                {
                    if (!intValDetected)
                    {
                        intValDetected = true;
                        if (strValDetected) strValSealed = true;
                    }
                    else
                        if (intValSealed) return null;
                    intVal = intVal + ch;
                }
            }
            IntValue = int.Parse(intVal);
            return this;
        }
    }
}
