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
using IncomeDataStorage.Data;
using System.Linq;

namespace IncomeDataStorage.Presentation
{
    public class KeyDataFinder
    {
        private List<KeysDataMapper> data;

        // конструктор
        public KeyDataFinder(List<KeysDataMapper> keysData)
        {
            data = keysData;
        }
        
        internal List<KeysDataMapper> SearchFoloorsNoStartWith(string text)
        {
            List<KeysDataMapper> result = new List<KeysDataMapper>();
            result.Clear();

            if (text == "") return null;

            try
            {
                var FloorNoQuery = from KeysDataMapper kd in data select kd;
                foreach (var kd in FloorNoQuery)
                {
                    if (kd.FloorNo.ToString().StartsWith(text))
                        result.Add(kd);
                    // ну и се..
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
