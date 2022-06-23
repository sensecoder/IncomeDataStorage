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
    public static class ExcelColNoCalc
    {
        // Словарь, содержащий латинцу в виде заглавных букв и соответствующие
        // им номера по-порядку.
        private static Dictionary<char, int> ColIndMask;

        /// <summary>
        /// Возвращает номер столбца, вычисляя его из буквенного индекса столбца,
        /// который принят в таблицах экселя. (есть ограничения).
        /// </summary>
        /// <param name="collInd">Буквенный индекс. В виде заглавных букв латинского 
        /// алфавита. Принимается не более двух букв.</param>
        /// <returns>Номер столбца в виде целого числа. -1 в случае ошибки</returns>
        public static int ColNo(string collInd)
        {
            int res = -1;
            var divMas = collInd.ToCharArray();
            if (divMas.Length > 2 && divMas.Length < 1) // поверка на количество элементов
                return res;
            if (ColIndMask == null) FillCollIndMask();

            int first, second;
            if (divMas.Length == 1)
            { first = 0; second = ColIndMask[divMas[0]]; }
            else
            { first = ColIndMask[divMas[0]]; second = ColIndMask[divMas[1]]; }

            res = 26 * first + second;

            return res;
        }

        /// <summary>
        /// Возвращает символьный индекс столбца, вычисляя его из порядкового номера этого столбца (есть ограничения).
        /// </summary>
        /// <param name="collInd">Порядковый номер столбца (начинается с 1 по 702)</param>
        /// <returns>Символьный индекс столбца, такой как принят в таблицах экселя (не более 2-х букв)</returns>
        public static string ColSymb(int collInd)
        {
            var res = "";

            if (collInd < 1 || collInd > 702) return null;

            if (ColIndMask == null) FillCollIndMask();

            Dictionary<int, char> ColSymbMask = new Dictionary<int, char>();
            foreach (var val in ColIndMask)
            {
                ColSymbMask.Add(val.Value, val.Key);
            }

            int delim, rest;
            delim = collInd / 26;
            rest = collInd % 26;

            char firstChar;
            char secondChar;
            if (rest == 0)
            {
                delim--;
                secondChar = ColSymbMask[26];
            }
            else secondChar = ColSymbMask[rest];
            if (delim > 0)
            {
                firstChar = ColSymbMask[delim];
                res = firstChar.ToString() + secondChar.ToString();
            }
            else res = secondChar.ToString();

            return res;
        }

        /// <summary>
        /// Заполняет словарь соответствия.
        /// </summary>
        private static void FillCollIndMask()
        {
            if (ColIndMask == null)
                ColIndMask = new Dictionary<char, int>();
            else
                ColIndMask.Clear();

            var j = 1;
            for (var i = 65; i <= 90; i++)
            {
                ColIndMask.Add((char)i, j);
                j++;
            }
        }
    }
}
