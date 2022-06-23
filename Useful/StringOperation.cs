namespace Useful
{
    /// <summary>
    /// Различные действия со строками
    /// </summary>
    public class StringOperation
    {
        /// <summary>
        /// Функция определяет, является ли строка простым целым числом
        /// </summary>
        /// <param name="str">Строка для определения</param>
        /// <returns>Возвращает логическое значение</returns>
        public static bool IsIntNumber(string str)
        {
            char[] chstr = new char[str.Length];
            chstr = str.ToCharArray();
            for (int i = 0; i < str.Length; i++)
            {
                if (i == 0)
                    if (chstr[i] == '-') // Проверка на отрицательное значение
                    {
                        i++;
                        if (i >= str.Length) return false;
                    }
                if ((chstr[i] < '0') || (chstr[i] > '9')) 
                        return (false);
            }
            return (true);
        }

        /// <summary>
        /// Функция определяет, является ли строка натуральным числом
        /// и только натуральным! Есл оно простое, то результат будет false
        /// </summary>
        /// <param name="str">Строка для определения</param>
        /// <returns>Возвращает логическое значение</returns>
        public static bool IsRealNumber(string str)
        {
            char[] chstr = new char[str.Length];
            chstr = str.ToCharArray();
            var hasDelimetr = false;
            for (int i = 0; i < str.Length; i++)
            {
                if (i == 0)
                    if (chstr[i] == '-') // Проверка на отрицательное значение
                    {
                        i++;
                        if (i >= str.Length) return false;
                    }
                if (chstr[i] < '0' || chstr[i] > '9')
                    if (chstr[i] != ',' || chstr[i] != '.')
                        return (false);
                if (chstr[i] == ',' || chstr[i] == '.')
                    hasDelimetr = true;
            }
            if (hasDelimetr) return (true);
            else return false;
        }

        /// <summary>
        /// Метод определяет, является ли символ числом.
        /// </summary>
        /// <param name="ch">Символ для определения</param>
        /// <returns>фальсетруе...</returns>
        public static bool IsNumber(char ch)
        {            
            if ((ch < '0') || (ch > '9')) return (false);
            
            return (true);
        }
    }
}