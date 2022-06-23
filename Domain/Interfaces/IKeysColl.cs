using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IncomeDataStorage.Domain
{
    /// <summary>
    /// Интерфейс предоставляющий коллекцию ключевых данных.
    /// </summary>
    public interface IKeysColl
    {
        CollectionOfKeys GetKeysColl();
    }
}
