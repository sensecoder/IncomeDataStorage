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

namespace IncomeDataStorage.Domain
{
    /// <summary>
    /// Коллекция всех ключевых идентификаторов (инфа о всех собственниках)
    /// </summary>
    public class CollectionOfKeys
    {
        public List<KeyData> Values;
        
        public void NewCollectionOfKeys(IKeysColl source)
        {
            //this.Clear;
            if (!TryToGetCollOfKeys((IKeysColl)source)) MessageBox.Show("Ошибка доступа к коллекции ключевых объектов."); 
        }

        private bool TryToGetCollOfKeys(IKeysColl source)
        {
            
            return false;
        }
    }
}
